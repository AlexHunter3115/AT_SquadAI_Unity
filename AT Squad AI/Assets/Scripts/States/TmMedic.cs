using UnityEngine;

public class TmMedic : TeamMateBaseState
{
    private int idx = 0;
    private float lowestHealth = 101;

    public override void EnterState(TeamMateStateManager teamMate)
    {
        lowestHealth = 9999;

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
        else 
        {
            for (int i = 0; i < SquadManager.instance.uiList.Count; i++)
            {
                float health = SquadManager.instance.uiList[i].GetComponent<TeamMateUISlot>().healthSlider.value;

                if (health < lowestHealth)
                { 
                    idx = i;
                    lowestHealth = health;
                }
            }

            if (lowestHealth == 100)
            {
                UIManager.instance.AddNewMessageToQueue($"{teamMate.nameText.text} didnt find anyone to heal", new Color(1, 0.7f, 0, 1));

                if (teamMate.Allerted)
                {
                    teamMate.ChangeState(1);
                }
                else
                {
                    teamMate.ChangeState(7);
                }
            }
            else
            {
                if (teamMate.name == SquadManager.instance.uiList[idx].name) //it self
                {
                    SquadManager.instance.teamMates[idx].GetComponent<TeamMateStateManager>().AddHealth(45);

                    if (teamMate.Allerted)
                    {
                        teamMate.ChangeState(1);
                    }
                    else
                    {
                        teamMate.ChangeState(7);
                    }
                }
                else
                {
                    GoToPoint(SquadManager.instance.teamMates[idx].transform.position, teamMate);
                }
            }
        }
    }

    public override void OnExit(TeamMateStateManager teamMate)
    {
        // find cover around
    }

    public override void OnUpdate(TeamMateStateManager teamMate)
    {
        if (Vector3.Distance(teamMate.transform.position, SquadManager.instance.teamMates[idx].transform.position) < 1f) 
        {
            SquadManager.instance.teamMates[idx].GetComponent<TeamMateStateManager>().AddHealth(30);
            if (teamMate.Allerted) 
            {
                teamMate.ChangeState(1);
            }
            else 
            {
                teamMate.ChangeState(7);
            }

            teamMate.abilityUsage = teamMate.abilityUsage - 1;
        }
    }
}
