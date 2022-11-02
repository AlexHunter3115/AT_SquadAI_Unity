using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TmUseAbility : TeamMateBaseState
{
    //use the ability of this character, most likely need to access it somehow
    public override void EnterState(TeamMateStateManager teamMate)
    {

        UIManager.instance.SetIcon(5, teamMate.memberName);
        teamMate.currStateText = "USE ABILITY";
        Debug.Log(teamMate.transform.name + " is in the ability state ");
    }

    public override void OnUpdate(TeamMateStateManager teamMate)
    {

    }


    public override void OnExit(TeamMateStateManager teamMate)
    {

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