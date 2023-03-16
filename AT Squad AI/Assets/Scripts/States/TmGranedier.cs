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
            UIManager.instance.AddNewMessageToQueue($"{teamMate.nameText.text} has no more ability usages", Color.red);

            if (teamMate.Allerted)
            {
                teamMate.ChangeState(1);
            }
            else
            {
                teamMate.ChangeState(7);
            }

            return;
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
            teamMate.abilityUsage += 1;
            SetMessage($"The Granadier {teamMate.nameText.text} has no targets to use tis ability on", new Color(1, 0.7f, 0, 1));
        }
    }
}
