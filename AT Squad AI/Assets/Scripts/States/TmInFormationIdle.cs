using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TmInFormationIdle : TeamMateBaseState
{
    // when following the player in formation and also will be rotating

    private Quaternion lookAt;

    public override void EnterState(TeamMateStateManager teamMate)
    {
        Debug.Log(teamMate.transform.name + " is in the formation idle state ");
    }

    public override void OnUpdate(TeamMateStateManager teamMate)
    {
        
            var time = TimerCheck(teamMate.formationCooldown, teamMate.formationTimer);

            if (time != -1)
            {
                teamMate.formationTimer = time;
                lookAt = GenRandomRot();
            }

           SmoothRotateTowards(lookAt, teamMate);
       
           GoToPoint(teamMate.FormationTran.position, teamMate);



        List<GameObject> list = CheckForEnemiesAround(teamMate);

        if (list.Count > 0) 
        {
            teamMate.ChangeState(6);
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