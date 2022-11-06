using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TmDead : TeamMateBaseState
{
    // called when the thing dies, comesback to life when the medic heals
    public override void EnterState(TeamMateStateManager teamMate)
    {
        teamMate.transform.GetChild(3).gameObject.SetActive(true);
        teamMate.transform.GetChild(2).gameObject.SetActive(false);

        UIManager.instance.SetIcon(4, teamMate.memberName);
        Debug.Log($"{teamMate.SelAbility} has died....");
        teamMate.currStateText = "DEAD";


    }
    public override void OnExit(TeamMateStateManager teamMate)
    {

        teamMate.transform.GetChild(3).gameObject.SetActive(false);
        teamMate.transform.GetChild(2).gameObject.SetActive(true);
    }
    public override void OnUpdate(TeamMateStateManager teamMate)
    {
        if (teamMate.Health > 0) 
        {
            teamMate.Alive = true;


            var list = CheckForEnemiesAround(teamMate);

            if (list.Count > 0) { teamMate.Allerted = true; }
            else { teamMate.Allerted = false; }

            if (teamMate.Allerted) 
            {

                teamMate.ChangeState(2);
            }
            else 
            {
                teamMate.ChangeState(5);
            }


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