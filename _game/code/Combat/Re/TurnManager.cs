using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private TeamManager currentTeam;
    private TeamManager waitingTeam;
    private Queue<CrewMember> currentTeamTurns = new Queue<CrewMember>();
    private CrewMember currentTurnMember;

    public void InitializeTurns(TeamManager team1, TeamManager team2, bool team1First)
    {
        if (team1First)
        {
            currentTeam = team1;
            waitingTeam = team2;
        }
        else
        {
            currentTeam = team2;
            waitingTeam = team1;
        }

        // Fill turn queue for the first team
        FillTurnQueue();
    }

    private void FillTurnQueue()
    {
        currentTeamTurns.Clear();

        // Get all alive members from the current team
        foreach (var member in currentTeam.GetTeamMembers())
        {
            if (member.IsAlive)
            {
                currentTeamTurns.Enqueue(member);
            }
        }
    }

    public CrewMember GetNextTurn()
    {
        if (currentTeamTurns.Count == 0)
        {
            // Just fill the queue with the current team's members
            // Don't swap teams here - that's handled in AdvanceTurn
            FillTurnQueue();

            // If no crew members are alive in the current team, return null
            if (currentTeamTurns.Count == 0)
            {
                return null;
            }
        }

        currentTurnMember = currentTeamTurns.Peek();
        return currentTurnMember;
    }

    public CrewMember GetCurrentTurn()
    {
        return currentTurnMember;
    }

    public bool AdvanceTurn()
    {
        if (currentTeamTurns.Count > 0)
        {
            currentTeamTurns.Dequeue(); // Remove the current member from the queue
        }

        // If queue is empty, it's time to switch teams
        if (currentTeamTurns.Count == 0)
        {
            SwapTeams();
            FillTurnQueue(); // Fill queue with members from the new team
            return true; // Indicates a new team's turn is starting
        }

        return false; // Same team continues
    }

    private void SwapTeams()
    {
        // Swap current and waiting teams
        TeamManager temp = currentTeam;
        currentTeam = waitingTeam;
        waitingTeam = temp;
    }

    public TeamManager GetCurrentTeam()
    {
        return currentTeam;
    }

    public TeamManager GetWaitingTeam()
    {
        return waitingTeam;
    }
}