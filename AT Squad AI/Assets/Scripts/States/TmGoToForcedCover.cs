using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TmGoToForcedCover : TeamMateBaseState
{
    // this does different things depending if its under fire   
    public override void EnterState(TeamMateStateManager teamMate)
    {
        UIManager.instance.SetIcon(1, teamMate.memberName);
        teamMate.currStateText = "GO TO FC";


        teamMate.AnimatorSetter(5);
    }


    public override void OnExit(TeamMateStateManager teamMate)
    {

    }



    public override void OnUpdate(TeamMateStateManager teamMate)
    {
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
            ShootAt(list[0], teamMate);
        }

        GoToPoint(teamMate.currCoverTransform.position, teamMate);

        if (ReachedDestination(teamMate))
        {
            if (teamMate.Allerted)
            {
                var name = teamMate.currCoverTransform.transform.name;
                if (name.Contains("Positive"))   // this two are the side ones   
                {
                    teamMate.currCoverType = TeamMateStateManager.CoverType.POSITIVE;
                }
                else if (name.Contains("Minus"))
                {
                    teamMate.currCoverType = TeamMateStateManager.CoverType.NEGATIVE;
                }
                else
                {
                    teamMate.currCoverType = TeamMateStateManager.CoverType.FORWARD;
                }
                
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
                var name = teamMate.currCoverTransform.transform.name;
                if (name.Contains("Positive"))   // this two are the side ones   
                {
                    teamMate.currCoverType = TeamMateStateManager.CoverType.POSITIVE;
                }
                else if (name.Contains("Minus"))
                {
                    teamMate.currCoverType = TeamMateStateManager.CoverType.NEGATIVE;
                }
                else
                {
                    teamMate.currCoverType = TeamMateStateManager.CoverType.FORWARD;
                }

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
