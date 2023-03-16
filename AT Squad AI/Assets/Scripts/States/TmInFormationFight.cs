
using System.Collections.Generic;

using UnityEngine;

public class TmInFormationFight : TeamMateBaseState
{
    List<GameObject> list = new List<GameObject>();

    public override void EnterState(TeamMateStateManager teamMate)
    {
        UIManager.instance.SetIcon(2, teamMate.memberName);
        teamMate.currStateText = "FORM F";
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
            LookAt(list[0], teamMate);
            teamMate.NavMeshAgent.isStopped = true;
        }

        GoToPoint(teamMate.FormationTran.position, teamMate);
    }

    public override void OnExit(TeamMateStateManager teamMate)
    {
        
    }


}
