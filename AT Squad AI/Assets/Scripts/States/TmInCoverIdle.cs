using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TmInCoverIdle : TeamMateBaseState
{


    //run checks
    //this WILL BE CHANGED
    public override void EnterState(TeamMateStateManager teamMate)
    {
        Debug.Log(teamMate.transform.name + " is in the cover idle state ");
    }

    public override void OnUpdate(TeamMateStateManager teamMate)
    {
        if (teamMate.Allerted) { teamMate.ChangeState(4); }
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