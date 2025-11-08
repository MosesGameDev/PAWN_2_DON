using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CharacterSelectionHandler;

namespace Buildings
{
    public class WalkingSimulator : MonoBehaviour
    {
        [ReadOnly][SerializeField] float distanceToEndPoint;
        [SerializeField] float distanceSpawnThreshold = 10;

        [Header("Spawn Settings")]
        [SerializeField] private BuildingsSection[] buildingSectionPrefabs;
        [SerializeField] private int initialSectionCount = 5;
        [SerializeField] private int poolSizePerSection = 3;

        [Header("References")]
        [SerializeField] private PlayerCharacterController player;
        [SerializeField] private Transform endPointTransform;
        [SerializeField] private Transform playerStartPosition;

        [Space]
        [SerializeField] List<BuildingsSection> activeSections = new List<BuildingsSection>();
        private Dictionary<int, ObjectPool<BuildingsSection>> sectionPools = new Dictionary<int, ObjectPool<BuildingsSection>>();

        bool isInitialized = false;
        Vector3 lastPos;

        public Action OnInitialized;

        private void Awake()
        {
            // Initialize object pools for each building section type
            for (int i = 0; i < buildingSectionPrefabs.Length; i++)
            {
                sectionPools[i] = new ObjectPool<BuildingsSection>(
                    buildingSectionPrefabs[i],
                    poolSizePerSection,
                    transform
                );
            }
        }

        [Button]
        public void Initialize()
        {
            ScriptRegistry.Instance.screenFade.FadeInFast(1);

            if (isInitialized)
            {
                Debug.LogWarning("BuildingSpawner is already initialized.");
                return;
            }


            for (int i = 0; i < initialSectionCount; i++)
            {
                SpawnSection();
            }

            UpdatedSpawnedSectionPositions();

            player.transform.position = new Vector3(0, player.transform.position.y, player.transform.position.z);
            player.gameObject.SetActive(true);
            player.EnableCharacter();
            player.GetComponent<CrewManager>().CrewMembersMove();
            ScriptRegistry.Instance.gameCam.Prioritize();
            ScriptRegistry.Instance.textGameController.SetHalfScreenCameraRect();

            OnInitialized?.Invoke();


           // print("BuildingSpawner initialized with " + initialSectionCount + " sections.");
        }


        [Button]
        public void Exit()
        {
            ScriptRegistry.Instance.screenFade.FadeInFast(1);
            isInitialized = false;

            foreach (var section in new List<BuildingsSection>(activeSections))
            {
                ReturnSectionToPool(section);
            }


            player.gameObject.SetActive(false);
        }

        private void SpawnSection()
        {

            int prefabIndex = UnityEngine.Random.Range(0, buildingSectionPrefabs.Length);
            Vector3 position = Vector3.zero;

            BuildingsSection newSection = sectionPools[prefabIndex].Get(position, Quaternion.identity);
            newSection.spawner = this;
            activeSections.Add(newSection);
        }

        [Button]
        public void UpdatedSpawnedSectionPositions()
        {
            for (int i = 0; i < activeSections.Count; i++)
            {
                if((i>0) && (i+1) < activeSections.Count)
                {
                    activeSections[i].transform.position = activeSections[i - 1].attachementPoint.position;
                }
            }

            activeSections[activeSections.Count -1].transform.position = activeSections[activeSections.Count - 2].attachementPoint.position;
            endPointTransform.position = activeSections[activeSections.Count - 1].attachementPoint.position;
            StartCoroutine(InitializeUpdate());
        }


        public bool playerPosSet;
        IEnumerator InitializeUpdate()
        {
            yield return new WaitForSeconds(1f);
            isInitialized = true;
            if (!playerPosSet)
            {
                player.transform.position = activeSections[0].startPosition.position;
                playerPosSet = true;
            }
        }

        public void ReturnSectionToPool(BuildingsSection section)
        {
            if (section == null) return;

            for (int i = 0; i < buildingSectionPrefabs.Length; i++)
            {
                if (section.CompareTag(buildingSectionPrefabs[i].tag))
                {
                    sectionPools[i].ReturnToPool(section);
                    activeSections.Remove(section);
                    return;
                }
            }

            // If we couldn't find the right pool (shouldn't happen),
            // just deactivate the object
            section.gameObject.SetActive(false);
            activeSections.Remove(section);
        }

        private void Update()
        {
            if (!isInitialized)
            {
                return;
            }

            lastPos = player.transform.position;
            distanceToEndPoint = Vector3.Distance(player.transform.position, endPointTransform.position);

            // Check if the player is close enough to the end point to spawn a new section
            if (distanceToEndPoint < distanceSpawnThreshold)
            {
                isInitialized = false;
                //print("Spawning new section due to player proximity to end point.");
                DeactivateSections();
                SpawnSection();
                UpdatedSpawnedSectionPositions();
                isInitialized = true;
            }
        }

        public void DeactivateSections()
        {
            for (int i = 0; i < activeSections.Count-1; i++)
            {
                ReturnSectionToPool(activeSections[i]);
            }
        }


        private void OnDestroy()
        {
            foreach (var pool in sectionPools.Values)
            {
                pool.Clear();
            }
        }
    }
}