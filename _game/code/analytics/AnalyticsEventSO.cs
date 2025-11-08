using UnityEngine;

[CreateAssetMenu(fileName = "AnalyticsEvent", menuName = "Analytics/AnalyticsEventSO")]
public class AnalyticsEventSO : ScriptableObject
{
    public void LogButtonEvent()
    {
        if(AnalyticsEvents.instance)
        {
           // Debug.Log("Logging button event");
        }
    }
}
