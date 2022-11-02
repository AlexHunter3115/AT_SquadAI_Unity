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
            Debug.Log($"iweqiwouwqioruwqiruwqioeuwqeiowqueqwieu");
            notShowingTimer = 0;
            teamMate.transform.position = teamMate.currCoverTransform.position;
        }



        if (teamMate.Allerted) 
        {
            //should look at the enemy
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