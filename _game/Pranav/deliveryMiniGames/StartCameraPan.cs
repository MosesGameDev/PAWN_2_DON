using UnityEngine;
using System.Collections.Generic;
using Unity.Cinemachine;
using DG.Tweening;

public class StartCameraPan : MonoBehaviour
{
    public int maxPans = 2;

    public CinemachineCamera panCam;
    public List<GameObject> activeMembers;
    public int currentIndex = 0;
    private float panDuration = 0.6f;


    void OnEnable()
    {
        if (maxPans == 0)
        {
            SetPriorityToZero();
            return;
        }
        InvokeRepeating(nameof(PanToNextActiveMember), 0, 1.2f); // Start panning after 1 second, repeat every 1.2 seconds
    }
    int max;
    void PanToNextActiveMember()
    {
        //Camera.main.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time = .4f;
        if (currentIndex < activeMembers.Count && activeMembers[currentIndex].activeInHierarchy && max<= maxPans)
        {
            CameraPan(activeMembers[currentIndex].transform, Vector3.up * 25);
            max++;
        }
        else if(max > 2 || currentIndex >= activeMembers.Count)
        {
            SetPriorityToZero(); // Set camera priority to zero after panning to all active members
            CancelInvoke(nameof(PanToNextActiveMember));
        }
        else
        {
            CancelInvoke(nameof(PanToNextActiveMember));
            InvokeRepeating(nameof(PanToNextActiveMember), 0, 1.2f); // Start panning after 1 second, repeat every 1.2 seconds
        }
        currentIndex++;

    }

    void CameraPan(Transform targetTransform, Vector3 displacement)
    {
        if (panCam.LookAt!=null)
        {
            panCam.LookAt = null;
        }
        //panCam.Priority = 12;
        //panCam.transform.DOMove(targetTransform.position + displacement, panDuration).SetEase(Ease.Linear);
        panCam.transform.DOMove(targetTransform.position + displacement, panDuration)
    .SetEase(Ease.InOutSine)
    .OnComplete(() => panCam.LookAt = targetTransform

    );
        panCam.Priority = 12;


        //panCam.LookAt = targetTransform;
        //panCam.transform.LookAt(targetTransform);
    }

    void SetPriorityToZero()
    {
        //Camera.main.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time = 1f;
        panCam.Priority = 0;
    }
}