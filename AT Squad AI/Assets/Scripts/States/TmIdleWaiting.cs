using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TmIdleWaiting : TeamMateBaseState
{

    public override void EnterState(TeamMateStateManager teamMate)
    {
        teamMate.transform.GetChild(3).gameObject.SetActive(true);
        teamMate.transform.GetChild(2).gameObject.SetActive(false) ;

        UIManager.instance.SetIcon(0, teamMate.memberName);
        teamMate.currStateText = "WAITING";

        teamMate.AnimatorSetter(3);
    }

    public override void OnUpdate(TeamMateStateManager teamMate)
    {
        
    }


    public override void OnExit(TeamMateStateManager teamMate)
    {
        teamMate.transform.GetChild(3).gameObject.SetActive(false);

        teamMate.transform.GetChild(2).gameObject.SetActive(true);
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