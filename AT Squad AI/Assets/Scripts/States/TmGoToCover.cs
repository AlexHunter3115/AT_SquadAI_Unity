using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TmGoToCover : TeamMateBaseState
{
    // this does different things depending if its under fire   
    public override void EnterState(TeamMateStateManager teamMate)
    {
        Debug.Log(teamMate.transform.name + " is in the go to cover state ");
    }

    public override void OnUpdate(TeamMateStateManager teamMate)
    {
        if (teamMate.Allerted) 
        {
            //should look at the enemy
        }
        
        GoToPoint(teamMate.currCoverTransform, teamMate);

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
        else 
        {
        }

    }
}
//new TmDead(),
//new TmFindCover(),
//new TmGoToCover(),
//new TmIdleWaiting(),
//new TmInCoverFight(),
//new TmInCoverIdle(),
//new TmInFormationFight(),
//new TmInFormationIdle(),
//new TmPatrollingAroundPoint(),
//new TmUseAbility()