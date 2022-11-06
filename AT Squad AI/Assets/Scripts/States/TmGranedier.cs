using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TmGranedier : TeamMateBaseState
{
    // called when the thing dies, comesback to life when the medic heals
    public override void EnterState(TeamMateStateManager teamMate)
    {

        Debug.Log($"in the medic granediers");
        if (teamMate.abilityUsage == 0)
        {
            if (teamMate.Allerted)
            {
                teamMate.ChangeState(1);
            }
            else
            {
                teamMate.ChangeState(7);
            }
        }

        if (teamMate.Allerted)
        {
            teamMate.abilityUsage = teamMate.abilityUsage - 1;

            teamMate.CallExplosive();

            teamMate.ChangeState(1);
        }
        else
        {
            teamMate.ChangeState(7);
        }

    }
    public override void OnExit(TeamMateStateManager teamMate)
    {

    }
    public override void OnUpdate(TeamMateStateManager teamMate)
    {

    }
}
