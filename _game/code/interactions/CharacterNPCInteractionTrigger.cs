using UnityEngine;

public class CharacterNPCInteractionTrigger : MonoBehaviour
{
    public GameObject playerCharacter;
    public GameObject[] NPC;
    [SerializeField] private AdvanceButton advanceButton;


    string npc_Id;
    public void PlaceInteractionTrigger(string NPC_ID)
    {
        npc_Id = NPC_ID;
        Invoke("PositionTrigger", .3f);
    }

    void PositionTrigger()
    {
        advanceButton.ToggleShowElipsis(true);
        Vector3 position = playerCharacter.transform.position + playerCharacter.transform.forward * 10;

        CrewManager.instance.CrewMembersRun();


        transform.position = new Vector3(position.x, transform.position.y, transform.position.z);
        gameObject.SetActive(true);
        GetCharacter(npc_Id).SetActive(true);
        GetComponent<Collider>().enabled = true;
    }


    public GameObject GetActiveCharacetr()
    {
        GameObject member;

        for (int i = 0; i < NPC.Length; i++)
        {
            if (NPC[i].gameObject.activeInHierarchy)
            {
                member = NPC[i].gameObject;
                return member;
            }
        }

        return null;
    }


    public GameObject GetCharacter(string id)
    {
        for (int i = 0; i < NPC.Length; i++)
        {
            if (NPC[i].GetComponent<CrewMember>())
            {
                if(NPC[i].GetComponent<CrewMember>().characterName == id)
                {
                    return NPC[i];
                }
            }
        }

        return null;
    }


    string InteractionAnimation = "isTalking";
    public void SetInteractionAnimation(string animationState = "isTalking")
    {
        InteractionAnimation = animationState;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponent<Collider>().enabled = false;
            other.GetComponent<PlayerCharacterController>().StopMoving();
            CrewManager.instance.CrewMembersStop();
            advanceButton.ToggleShowElipsis(false);
            GetCharacter(npc_Id).GetComponent<Animator>().CrossFade(InteractionAnimation, .1f);
        }
    }

    public void CompleteInteraction()
    {
        GetCharacter(npc_Id).GetComponent<Animator>().CrossFade("root|M_Idle standing", .1f);
        playerCharacter.GetComponent<PlayerCharacterController>().Move();

        CrewManager.instance.CrewMembersMove();
        Invoke("Deactivate", 2);
    }

    void Deactivate()
    {
        GetCharacter(npc_Id).SetActive(false);
    }
}
