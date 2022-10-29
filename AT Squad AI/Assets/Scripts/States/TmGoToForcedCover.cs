using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TmGoToForcedCover : TeamMateBaseState
{
    // this does different things depending if its under fire   
    public override void EnterState(TeamMateStateManager teamMate)
    {
        teamMate.currStateText = "GO TO FC";
        Debug.Log(teamMate.transform.name + " is in the go to cover state ");
    }

    public override void OnUpdate(TeamMateStateManager teamMate)
    {
        if (teamMate.Allerted)
        {
            //should look at the enemy
        }

        GoToPoint(teamMate.currForcedCoverTransform.position, teamMate);

        if (ReachedDestination(teamMate))
        {
            Debug.Log($"called for reached ");
            if (teamMate.Allerted)
            {

                var name = teamMate.currForcedCoverTransform.transform.name;
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
                    Debug.Log($"dont call this pls");
                }
            }
            else
            {


                var name = teamMate.currForcedCoverTransform.transform.name;
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

                    Debug.Log($"dont call this pls");
                }
                else
                {
                    Debug.Log($"pls tell me this gets calls");
                    teamMate.ChangeState(5);
                }
            }
        }
        else
        {
        }

    }
}
