using UnityEngine;

namespace Buildings
{
    public class BuildingsSection : MonoBehaviour
    {
        public Transform startPosition;
        public Transform attachementPoint;

        [HideInInspector]
        public WalkingSimulator spawner;


    }
}