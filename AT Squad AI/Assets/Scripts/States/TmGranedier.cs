using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TmGranedier : TeamMateBaseState
{
    // called when the thing dies, comesback to life when the medic heals
    public override void EnterState(TeamMateStateManager teamMate)
    {
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

            CallExplosive(teamMate);

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


    public void CallExplosive(TeamMateStateManager teamMate)
    {
        var listofEnemies = CheckForEnemiesAround(teamMate);

        if (listofEnemies.Count > 0) 
        {
            teamMate.CallExplosive();
        }
        else 
        {
            SetMessage($"The Granadier {teamMate.name} has no targets to use tis ability on", new Color(1, 0.7f, 0, 1));
        }
    }
}
