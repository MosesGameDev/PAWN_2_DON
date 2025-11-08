using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using Sirenix.OdinInspector;
using System;
using UnityEngine.Events;

namespace Buildings
{
    public class InteractibleBuilding : BuildingsSection
    {
        [Header("Special: Building")]
        public string buildingId;
        [Space]
        public CinemachineCamera entrySequenceCam;
        [Space]
        [SerializeField] private Transform[] entryPathTransformPositions;
        [SerializeField] private Transform[] exitPathTransformPositions;
        [SerializeField] private float entryDuration = 7f;
        [SerializeField] private float exitDuration = 4f;
        [SerializeField] private float rotationSpeed = 5f;

        private GameObject player;
        private bool isMoving = false;
        private Coroutine currentMovementCoroutine;
        public static event Action<string> OnEnterBuilding;
        public UnityEvent onEnterBuilding;

        public void PlayEntrySequence()
        {
            if (isMoving) return;

            if (!entrySequenceCam) return;

            player = PlayerCharacterController.instance.gameObject;
            entrySequenceCam.enabled = true;

            if (currentMovementCoroutine != null)
                StopCoroutine(currentMovementCoroutine);

            currentMovementCoroutine = StartCoroutine(MoveAlongPath(entryPathTransformPositions, entryDuration, true));
        }

        void EnterBuilding()
        {
            ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("screenFade").Show();
            onEnterBuilding.Invoke();
            OnEnterBuilding?.Invoke(buildingId);
        }

        [Button]
        public void PlayExitSequence()
        {
            if (isMoving) return;

            ScriptRegistry.Instance.uiDialoguesManager.GetUIDialogue("screenFade").Hide();
            player = PlayerCharacterController.instance.gameObject;

            if (currentMovementCoroutine != null)
                StopCoroutine(currentMovementCoroutine);

            CrewManager.instance.CrewMembersMove();
            currentMovementCoroutine = StartCoroutine(MoveAlongPath(exitPathTransformPositions, exitDuration, false));
        }

        private IEnumerator MoveAlongPath(Transform[] pathPoints, float duration, bool isEntry)
        {
            isMoving = true;
            float elapsedTime = 0f;
            Vector3[] positions = GetPathPositions(pathPoints);

            float totalDistance = 0f;
            for (int i = 1; i < positions.Length; i++)
            {
                totalDistance += Vector3.Distance(positions[i - 1], positions[i]);
            }

            while (elapsedTime < duration)
            {
                float normalizedTime = elapsedTime / duration;

                Vector3 currentPosition = GetPositionAlongPath(positions, normalizedTime);
                player.transform.position = currentPosition;

                if (normalizedTime < 0.99f) // Avoid rotation calculations at the very end
                {
                    Vector3 nextPosition = GetPositionAlongPath(positions, Mathf.Min(normalizedTime + 0.1f, 1f));
                    Vector3 direction = (nextPosition - currentPosition).normalized;

                    if (direction != Vector3.zero)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(direction);
                        player.transform.rotation = Quaternion.Slerp(
                            player.transform.rotation,
                            targetRotation,
                            rotationSpeed * Time.deltaTime
                        );
                    }
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            player.transform.position = positions[positions.Length - 1];

            isMoving = false;

            if (isEntry)
            {
                EnterBuilding();
            }
            else
            {
                OnFinishExit();
            }
        }

        private void OnFinishExit()
        {
            entrySequenceCam.enabled = false;
            CrewManager.instance.CrewMembersMove();
            ScriptRegistry.Instance.textGameController.advanceButton.ToggleShowElipsis(false);
        }

        private Vector3 GetPositionAlongPath(Vector3[] positions, float normalizedTime)
        {
            if (positions.Length == 1) return positions[0];

            float scaledTime = normalizedTime * (positions.Length - 1);
            int index = Mathf.FloorToInt(scaledTime);
            float remainder = scaledTime - index;

            if (index >= positions.Length - 1)
                return positions[positions.Length - 1];

            return Vector3.Lerp(positions[index], positions[index + 1], remainder);
        }

        private Vector3[] GetPathPositions(Transform[] transformPositions)
        {
            List<Vector3> positions = new List<Vector3>();
            foreach (var entry in transformPositions)
            {
                positions.Add(entry.position);
            }
            return positions.ToArray();
        }
    }
}