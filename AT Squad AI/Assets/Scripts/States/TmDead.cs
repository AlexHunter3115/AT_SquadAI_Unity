using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TmDead : TeamMateBaseState
{
    // called when the thing dies, comesback to life when the medic heals
    public override void EnterState(TeamMateStateManager teamMate)
    {
        Debug.Log($"{teamMate.SelAbility} has died....");
        teamMate.currStateText = "DEAD";
    }

    public override void OnUpdate(TeamMateStateManager teamMate)
    {
        if (teamMate.Health > 0) 
        {
            teamMate.Alive = true;
            teamMate.ChangeState(2);
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