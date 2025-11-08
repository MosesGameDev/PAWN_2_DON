using DG.Tweening;
using UnityEngine;
namespace Buildings
{
    public class InteractiveBuildingEntryTrigger : MonoBehaviour
    {
        [SerializeField] private InteractibleBuilding building;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                building.PlayEntrySequence();
            }
        }

    }
}

