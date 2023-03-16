using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TmInFormationIdle : TeamMateBaseState
{
    // when following the player in formation and also will be rotating

    private Quaternion lookAt;

    public override void EnterState(TeamMateStateManager teamMate)
    {
        UIManager.instance.SetIcon(1, teamMate.memberName);
        teamMate.currStateText = "FORM I";
    }

    public override void OnUpdate(TeamMateStateManager teamMate)
    {
        if (Vector3.Distance(teamMate.FormationTran.position, teamMate.transform.position) < 0.3f) 
        {
            var time = TimerCheck(teamMate.formationCooldown, teamMate.formationTimer);

            if (time != -1)
            {
                teamMate.formationTimer = time;
                lookAt = GenRandomRot();
            }

            teamMate.AnimatorSetter(3);

            SmoothRotateTowards(lookAt, teamMate);
        }
        else 
        {
            teamMate.AnimatorSetter(5);
            GoToPoint(teamMate.FormationTran.position, teamMate);
        }

        List<GameObject> list = CheckForEnemiesAround(teamMate);

        if (list.Count > 0)
        {
            teamMate.ChangeState(6);
        }
        else 
        {
            teamMate.NavMeshAgent.isStopped = false;
        }
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