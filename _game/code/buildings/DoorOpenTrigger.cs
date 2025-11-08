using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class DoorOpenTrigger : MonoBehaviour
{
    [SerializeField] private Transform doorTransform;
    [SerializeField] private Vector3 openRoatation;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            OpenDoor();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CloseDoor();
        }
    }

    [Button]
    void OpenDoor()
    {
        doorTransform.DOLocalRotate(openRoatation, .6f).SetEase(Ease.OutBack);
    }

    [Button]
    void CloseDoor()
    {
        doorTransform.DOLocalRotate(Vector3.zero, .6f);
    }
}
