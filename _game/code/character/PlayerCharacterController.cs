using DG.Tweening;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using static CharacterSelectionHandler;

public class PlayerCharacterController : MonoBehaviour
{
    public static PlayerCharacterController instance;

    [Header("Character Movement Settings")]
    public bool isMoving;
    [SerializeField] private bool moveOnStart;
    [SerializeField] private float movementSpeed;
    public Animator animator;

    [Header("Attachments")]
    public CharacterAttachmentHandler attachmentHandler;

    [Space]
    public GameObject africanCharacter;
    public GameObject caucasianCharacter;

    BodyType bodyType;
    public static event Action<float> onAttack;


    private void OnEnable()
    {
        //CombatController.OnBattleEnd += CombatController_OnBattleEnd;
        CharacterSelectionHandler.OnBodyTypeSelected += CharacterSelectionHandler_OnBodyTypeSelected; ;

    }

    private void CharacterSelectionHandler_OnBodyTypeSelected(BodyType obj)
    {
        bodyType = obj;
    }

    private void OnDisable()
    {
        //CombatController.OnBattleEnd -= CombatController_OnBattleEnd;
        CharacterSelectionHandler.OnBodyTypeSelected -= CharacterSelectionHandler_OnBodyTypeSelected;

    }

    private void Awake()
    {
        instance = this;
    }

    public void EnableCharacter()
    {

        if (PlayerPrefs.GetString("SELECTED_CHARACTER") == "AFRICAN")
        {
            bodyType = BodyType.AFRICAN;
        }
        else if (PlayerPrefs.GetString("SELECTED_CHARACTER") == "CAUCASIAN")
        {
            bodyType = BodyType.CAUCASIAN;
        }

        switch (bodyType)
        {
            case BodyType.AFRICAN:
                africanCharacter.SetActive(true);
                caucasianCharacter.SetActive(false);
                africanCharacter.GetComponent<CustomizableCharacter>().EquipOutfitFromSave();
                break;
            case BodyType.CAUCASIAN:
                africanCharacter.SetActive(false);
                caucasianCharacter.SetActive(true);
                caucasianCharacter.GetComponent<CustomizableCharacter>().EquipOutfitFromSave();
                break;
        }

    }


    [Button("Use Mobile")]
    public void ToggleUseMobile(bool useMobile)
    {
        if (!useMobile)
        {
            useMobile = false;
            animator.CrossFade("root|M_Walk normal", .25f);
        }
        else
        {
            useMobile = true;
            animator.CrossFade("M_Walk With Mobile", .25f);
        }
    }

    private void CombatController_OnBattleEnd()
    {
        Move();
    }

    private void Start()
    {

        if (moveOnStart)
        {
            Move();
        }

        //EnableCharacter();
    }


    public void Move()
    {
        isMoving = true;
        movementSpeed = 1;
        animator.CrossFade("root|M_Walk normal", .25f);
    }

    public void MoveToPosition(Transform targetPositionTransform, out Sequence sequence, bool faceCamera = true)
    {
        Vector3 targetPosition = new Vector3(targetPositionTransform.position.x, transform.position.y, targetPositionTransform.position.z);

        sequence = DOTween.Sequence();
        sequence
        .Append(transform.DOLookAt(targetPosition, 0.3f).OnComplete(() => animator.CrossFade("root|M_Walk normal", .25f)))
        .Append(transform.DOMove(targetPosition, 3f).SetEase(Ease.Linear));
    }

    public void StopMoving(bool faceCamera=false)
    {
        isMoving = false;

        if(faceCamera)
        {
            transform.DORotate(new Vector3(0,180,0), .5f);
        }


        animator.CrossFade("root|M_Idle standing", .25f);
    }

    public void Run()
    {
        isMoving = true;
        movementSpeed = 6f;

        animator.CrossFade("root|M_Run", .25f);
    }

    private void Update()
    {
        if (isMoving)
        {
            transform.position = new Vector3(transform.position.x + (Time.deltaTime * movementSpeed), transform.position.y, transform.position.z);
        }
    }


}


/// <summary>
/// For any game objects we would want the player to have, I.e guns, boxes, etc.
/// </summary>
[System.Serializable]
public class AttachableItem
{
    public GameObject item;
    public string itemId;
}

[System.Serializable]
public class CharacterAttachmentHandler
{
    public AttachableItem[] attachableItems;
    public AttachableItem GetAttachment(string id)
    {
        for (int i = 0; i < attachableItems.Length; i++)
        {
            if (attachableItems[i].itemId == id)
            {
                return attachableItems[i];
            }
        }

        return null;
    }
}