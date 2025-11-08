using System.Collections.Generic;
using UnityEngine;

public class CrewManager : MonoBehaviour
{
    public static CrewManager instance;
    public CrewMember[] crewMembers;
    CrewMember hiredCrewMember;

    [Space]
    public Transform hiredCrewParentTransform;
    public Transform[] hiredCrewParentTransformPositions;
    public List<CrewMember> hiredCrewMembers = new List<CrewMember>();

    private const string HIRED_CREW_KEY = "HiredCrewMembers";

    private void OnEnable()
    {
        //CombatController.OnBattleStart += CombatController_OnBattleStart;
        //CombatController.OnBattleEnd += CombatController_OnBattleEnd;
        SaveGameManager.OnDataSaved += SaveGameManager_OnDataSaved;
    }

    private void SaveGameManager_OnDataSaved()
    {
        SaveHiredCrewMembers(); // Save after hiring a new crew member
    }

    private void CombatController_OnBattleEnd()
    {
        CrewMembersMove();

        foreach (var crewMember in hiredCrewMembers)
        {
            crewMember.transform.parent = hiredCrewParentTransform;
        }
    }

    private void CombatController_OnBattleStart()
    {
        CrewMembersFight();

        foreach (var crewMember in hiredCrewMembers)
        {
            crewMember.transform.parent = null;
        }
    }

    public CrewMember GetActiveHiredCrewMember()
    {
        return hiredCrewMember;
    }

    private void OnDisable()
    {
        //CombatController.OnBattleStart -= CombatController_OnBattleStart;
        //CombatController.OnBattleEnd -= CombatController_OnBattleEnd;
        SaveGameManager.OnDataSaved -= SaveGameManager_OnDataSaved;

    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        LoadHiredCrewMembers();
    }

    public void HireMember(string id)
    {
        for (int i = 0; i < crewMembers.Length; i++)
        {
            if (crewMembers[i].characterName == id)
            {
                hiredCrewMember = crewMembers[i];
                OnHireCrewMember();
            }
        }
    }

    void OnHireCrewMember()
    {
        hiredCrewMembers.Add(hiredCrewMember);
        hiredCrewMember.gameObject.SetActive(true);
        hiredCrewMember.Initialize();
        ScriptRegistry.Instance.textGameController.CreateTextDialogueElement($"{hiredCrewMember.characterName} is now a part of your gang ", out TextDialogueUIElement textDialogue);
        Invoke("ShowOnHireMessage", .5f);
    }

    void ShowOnHireMessage()
    {
        ScriptRegistry.Instance.textGameController.CreateTextDialogueElement($"{hiredCrewMember.characterName} and you go to do the delivery ", out TextDialogueUIElement textDialogue);
    }

    public void CrewMembersStop()
    {
        for (int i = 0; i < hiredCrewMembers.Count; i++)
        {
            hiredCrewMembers[i].Stop();
        }
    }

    public void CrewMembersRun()
    {
        for (int i = 0; i < hiredCrewMembers.Count; i++)
        {
            hiredCrewMembers[i].Run();
        }
    }

    public void CrewMembersMove()
    {
        for (int i = 0; i < hiredCrewMembers.Count; i++)
        {
            hiredCrewMembers[i].Move();
        }
    }

    public void CrewMembersFight()
    {
        for (int i = 0; i < hiredCrewMembers.Count; i++)
        {
            hiredCrewMembers[i].OnStartFight();
        }
    }

    // Save the list of hired crew members to PlayerPrefs
    public void SaveHiredCrewMembers()
    {
        string savedData = "";

        // Create a comma-separated string of crew member names
        for (int i = 0; i < hiredCrewMembers.Count; i++)
        {
            savedData += hiredCrewMembers[i].characterName;
            if (i < hiredCrewMembers.Count - 1)
            {
                savedData += ",";
            }
        }

        // Save to PlayerPrefs
        PlayerPrefs.SetString(HIRED_CREW_KEY, savedData);
        PlayerPrefs.Save();

        Debug.Log("Saved crew members: " + savedData);
    }

    // Load the list of hired crew members from PlayerPrefs
    public void LoadHiredCrewMembers()
    {
        // Clear current list
        hiredCrewMembers.Clear();

        hiredCrewMembers.Add(GetComponent<CrewMember>());

        // Get saved string from PlayerPrefs
        if (PlayerPrefs.HasKey(HIRED_CREW_KEY))
        {
            string savedData = PlayerPrefs.GetString(HIRED_CREW_KEY);
            Debug.Log("Loading crew members: " + savedData);

            if (!string.IsNullOrEmpty(savedData))
            {
                // Split the string to get individual crew member names
                string[] crewNames = savedData.Split(',');

                // Hire each saved crew member
                foreach (string crewName in crewNames)
                {
                    // Find the crew member in the available crew members array
                    for (int i = 0; i < crewMembers.Length; i++)
                    {
                        if (crewMembers[i].characterName == crewName)
                        {
                            // Add to hired list without showing messages
                            CrewMember member = crewMembers[i];
                            hiredCrewMembers.Add(member);
                            member.gameObject.SetActive(true);
                            member.Initialize();

                            // Position the crew member
                            member.transform.parent = hiredCrewParentTransform;
                        }
                    }
                }
            }
        }
    }

    // Method to clear all saved crew data (useful for new game)
    public void ClearSavedCrewData()
    {
        PlayerPrefs.DeleteKey(HIRED_CREW_KEY);
        PlayerPrefs.Save();

        // Clear current crew members
        foreach (var crewMember in hiredCrewMembers)
        {
            crewMember.gameObject.SetActive(false);
        }

        hiredCrewMembers.Clear();
        Debug.Log("Cleared all saved crew data");
    }
}