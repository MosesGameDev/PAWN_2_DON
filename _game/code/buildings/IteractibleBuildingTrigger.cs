using UnityEngine;
using DG.Tweening;

namespace Buildings
{
    public class IteractibleBuildingTrigger : MonoBehaviour
    {
        public InteractibleBuilding building;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                building.PlayEntrySequence();

                PlayerCharacterController.instance.isMoving = false;
                CrewManager.instance.CrewMembersMove();
            }
        }
    }
}

