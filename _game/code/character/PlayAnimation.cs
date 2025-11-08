using UnityEngine;
using Sirenix.OdinInspector;

public class PlayAnimation : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    [Button]
    void PlayAnim(string animName= "root|Day_1_impact")
    {
        _animator.CrossFade(animName, 0.1f);
    }
}
