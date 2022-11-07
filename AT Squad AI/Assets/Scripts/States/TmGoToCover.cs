using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TmGoToCover : TeamMateBaseState
{



    private float notShowingCooldown;
    private float notShowingTimer;
    // this does different things depending if its under fire   
    public override void EnterState(TeamMateStateManager teamMate)
    {

        UIManager.instance.SetIcon(1, teamMate.memberName);
        teamMate.currStateText = "GO TO COVER";
        Debug.Log(teamMate.transform.name + " is in the go to cover state ");

        notShowingTimer = 0;
        notShowingCooldown = 15f;
    }
    public override void OnExit(TeamMateStateManager teamMate)
    {

    }
    public override void OnUpdate(TeamMateStateManager teamMate)
    {

        notShowingTimer += Time.deltaTime;

        if (notShowingTimer >= notShowingCooldown)
        {
            notShowingTimer = 0;
            teamMate.transform.position = teamMate.currCoverTransform.position;
        }


        var list = CheckForEnemiesAround(teamMate);
        if (list.Count > 0) 
        {
            teamMate.Allerted = true;
        }
        else 
        {
            teamMate.Allerted = false;
        }


        if (teamMate.Allerted) 
        {
            Debug.Log($"ddsdfdfdsfdssfd");
            ShootAt(list[0], teamMate);
        }
        
        GoToPoint(teamMate.currCoverTransformVector3, teamMate);

        if (ReachedDestination(teamMate)) 
        {
            Debug.Log($"called for reached ");
            if (teamMate.Allerted) 
            {
                if (teamMate.currCoverType == TeamMateStateManager.CoverType.POSITIVE || teamMate.currCoverType == TeamMateStateManager.CoverType.NEGATIVE)
                {
                    teamMate.ChangeState(10);
                }
                else
                {
                    teamMate.ChangeState(4);
                }
            }
            else 
            {
                if (teamMate.currCoverType == TeamMateStateManager.CoverType.POSITIVE || teamMate.currCoverType == TeamMateStateManager.CoverType.NEGATIVE)
                {
                    teamMate.ChangeState(11);
                }
                else
                {
                    teamMate.ChangeState(5);
                }
            }
        }
    }
}
