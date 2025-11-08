using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    [SerializeField] private string teamName;
    [SerializeField] private Transform[] memberPositions; // Array of positions for team members
    [SerializeField] private CrewMember[] teamMembers; // Array of crew members

    private Dictionary<CrewMember, Vector3> originalPositions = new Dictionary<CrewMember, Vector3>();

    public void InitializeTeam()
    {
        // Make sure we have enough positions for all members
        if (teamMembers.Length > memberPositions.Length)
        {
            Debug.LogWarning($"Team {teamName} has more members than positions!");
        }

        // Position each crew member at their designated position
        for (int i = 0; i < teamMembers.Length && i < memberPositions.Length; i++)
        {
            if (teamMembers[i] != null)
            {
                teamMembers[i].transform.position = memberPositions[i].position;
                teamMembers[i].transform.rotation = memberPositions[i].rotation;

                // Store original position
                originalPositions[teamMembers[i]] = memberPositions[i].position;

                // Initialize crew member
                teamMembers[i].Initialize();
            }
        }
    }

    public void ResetPositions()
    {
        foreach (var member in teamMembers)
        {
            if (member != null && originalPositions.ContainsKey(member))
            {
                member.transform.position = originalPositions[member];
            }
        }
    }

    public CrewMember[] GetTeamMembers()
    {
        return teamMembers;
    }

    public string GetTeamName()
    {
        return teamName;
    }
}