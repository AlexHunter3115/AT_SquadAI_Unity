using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TmInCoverFight : TeamMateBaseState
{
    //deal with the name 
    public override void EnterState(TeamMateStateManager teamMate)
        //THIS WILL BE CHANGED
    {
        Debug.Log(teamMate.transform.name + " is in the cover fight ");
    }

    public override void OnUpdate(TeamMateStateManager teamMate)
    {
        if (!teamMate.Allerted) { teamMate.ChangeState(5); }
    }
}





//new TmDead(), 0
//new TmFindCover(),
//new TmGoToCover(),2
//new TmIdleWaiting(),
//new TmInCoverFight(),4
//new TmInCoverIdle(),
//new TmInFormationFight(),6
//new TmInFormationIdle(),
//new TmPatrollingAroundPoint(),
//new TmUseAbility()