using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

public class Day02PackageInteractionController : MonoBehaviour
{

    [SerializeField] private CharacterNPCInteractionTrigger interactionTrigger;


    [Button]
    public void StartInteraction()
    {
        PlayerCharacterController.instance.attachmentHandler.GetAttachment("senderbox").item.SetActive(true);
        interactionTrigger.GetActiveCharacetr().GetComponent<Animator>().CrossFade("root|M_Box_With_Idle", 0.1f);
    }


    [Button]
    public void GivePlayerPackage()
    {
        ScriptRegistry.Instance.textGameController.advanceButton.ToggleShowElipsis(true);
        interactionTrigger.GetActiveCharacetr().GetComponent<Animator>().CrossFade("root|M_Box_Give", 0.1f);

    }

    public void OnHandOverPackage()
    {
        PlayerCharacterController.instance.attachmentHandler.GetAttachment("senderbox").item.SetActive(false);
        PlayerCharacterController.instance.attachmentHandler.GetAttachment("box").item.SetActive(true);

        ScriptRegistry.Instance.textGameController.advanceButton.ToggleShowElipsis(false);

    }

    public void DeliverPackage()
    {
        Transform playerTransform = PlayerCharacterController.instance.transform;

        Vector3 pos = new Vector3(playerTransform.position.x + .4f, playerTransform.position.y, playerTransform.position.z);
        playerTransform.DOMove(pos, .8f)
        .OnComplete
        (
            () =>
            {
                ScriptRegistry.Instance.textGameController.advanceButton.ToggleShowElipsis(true);
                interactionTrigger.GetActiveCharacetr().GetComponent<Animator>().CrossFade("root|Recive_Box", 0.1f);
                //PlayerCharacterController.instance.animator.CrossFade("root|M_Box_Give", 0.1f);
            }
        );


    }

    public void OnDeliverPackage()
    {
        ScriptRegistry.Instance.textGameController.advanceButton.ToggleShowElipsis(false);
        PlayerCharacterController.instance.attachmentHandler.GetAttachment("recieverbox").item.SetActive(true);
        PlayerCharacterController.instance.attachmentHandler.GetAttachment("box").item.SetActive(false);
    }


}
