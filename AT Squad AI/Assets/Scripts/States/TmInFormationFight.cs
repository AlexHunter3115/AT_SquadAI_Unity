using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TmInFormationFight : TeamMateBaseState
{
    // when following the player in formation and will be firing at someone

    List<GameObject> list = new List<GameObject>();

    public override void EnterState(TeamMateStateManager teamMate)
    {

        UIManager.instance.SetIcon(2, teamMate.memberName);
        teamMate.currStateText = "FORM F";
        Debug.Log(teamMate.transform.name + " is in the formation fight state ");
    }

    public override void OnUpdate(TeamMateStateManager teamMate)
    {
        list = CheckForEnemiesAround(teamMate);

        if (list.Count == 0)
        {
            teamMate.ChangeState(7);
        }


        if (list.Count > 0) 
        {

            ShootAt(list[0], teamMate);
        }

        GoToPoint(teamMate.FormationTran.position, teamMate);

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
//new forcegotocover()