using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TmPatrollingAroundPoint : TeamMateBaseState
{
     // given a point, choose a random position around that point dont fuck with buildings
    public override void EnterState(TeamMateStateManager teamMate)
    {
        teamMate.currStateText = "PATROLLING AROUND P";
        Debug.Log(teamMate.transform.name + " is in the patrol around state ");
    }

    public override void OnUpdate(TeamMateStateManager teamMate)
    {
        GoToPoint(teamMate.currCoverTransformVector3,teamMate);



        if (ReachedDestination(teamMate)) 
        {
            Vector3 newPos = new Vector3(teamMate.transform.position.x + Random.Range(-7,7) , teamMate.transform.position.y, teamMate.transform.position.z + Random.Range(-7, 7));
            teamMate.currCoverTransformVector3 = newPos;
        
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