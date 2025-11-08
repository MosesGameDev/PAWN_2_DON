using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using DG.Tweening;

public class NPC_Enemy_CharacterController : MonoBehaviour
{
    [Space]
    public CinemachineCamera cam;
    public Animator npcAnimator;

    [Space]
    public ParticleSystem dizzyFx;

    [Header("Appearance")]
    public CrewMember c_crewMember;
    [SerializeField] private CrewMember[] characterBodies;

    Vector3 characterBodyPosition;

    CrewMember GetRandomMember()
    {
        return characterBodies[Random.Range(0, characterBodies.Length)];
    }

    public CrewMember GetCrewMember(string id)
    {
        for (int i = 0; i < characterBodies.Length; i++)
        {
            if (characterBodies[i].GetComponent<CrewMember>())
            {
                if (characterBodies[i].GetComponent<CrewMember>().characterName == id)
                {
                    return characterBodies[i];
                }
            }
        }

        return null;
    }


    public void Initialize(int power, int _health, string characterName)
    {
        c_crewMember = GetCrewMember(characterName);

        if(c_crewMember != null)
        {
            //dizzyFx = characterBody.dizzyPFX;
            //dizzyFx.gameObject.SetActive(false);

            npcAnimator = c_crewMember.GetComponent<Animator>();
            c_crewMember.gameObject.SetActive(true);
            ShowPower();
        }

    }

    public void ShowPower()
    {
    }

    public void UpdateHealth(int value)
    {
    }

    public void HideHelth()
    {
    }

    public void Reset()
    {
        npcAnimator.CrossFade("root|M_Idle standing", 0);
        c_crewMember.gameObject.SetActive(false);
        c_crewMember.transform.localPosition = c_crewMember.bodyPos;
    }

    public void Taunt(out Tween tween)
    {
        //cam.Prioritize();
        //npcAnimator.SetTrigger("taunt");
        //AnimatorClipInfo[] m_CurrentClipInfo = npcAnimator.GetCurrentAnimatorClipInfo(0);
        float duration = .7f;

        float v = 0;
        tween = DOTween.To(() => v, x => v = x, 1, duration);
    }


}
