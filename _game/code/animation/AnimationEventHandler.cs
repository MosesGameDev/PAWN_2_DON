using UnityEngine;
using UnityEngine.Events;

public class AnimationEventHandler : MonoBehaviour
{
    [SerializeField] private AnimationEvent[] animationEvents;

    public void ExecuteEvent(string name)
    {
        for (int i = 0; i < animationEvents.Length; i++)
        {
            if (animationEvents[i].name == name)
            {
                animationEvents[i].UnityEvent.Invoke();
                return;
            }
        }
    }
}


[System.Serializable]
public class AnimationEvent
{
    public string name;
    public UnityEvent UnityEvent;
}