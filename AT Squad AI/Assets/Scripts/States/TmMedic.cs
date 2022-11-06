using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class TmMedic : TeamMateBaseState
{


    private int idx = 0;
    private float lowestHealth = 101;


    // called when the thing dies, comesback to life when the medic heals
    public override void EnterState(TeamMateStateManager teamMate)
    {
        // get the teammate with lowest health

        Debug.Log($"in the medic ability");

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


        for (int i = 0; i < SquadManager.instance.uiList.Count; i++)
        {

            float health = SquadManager.instance.uiList[i].GetComponent<TeamMateUISlot>().healthSlider.value;

            if (health < lowestHealth)
            {
                idx = i;
                lowestHealth = health;
            }
            
        }
        if (lowestHealth == 100) {
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
    public override void OnExit(TeamMateStateManager teamMate)
    {

        // find cover around
    }
    public override void OnUpdate(TeamMateStateManager teamMate)
    {
        Debug.Log($"{Vector3.Distance(teamMate.transform.position, SquadManager.instance.teamMates[idx].transform.position)}");
        if (Vector3.Distance(teamMate.transform.position, SquadManager.instance.teamMates[idx].transform.position) < 1f) 
        {
            SquadManager.instance.teamMates[idx].GetComponent<TeamMateStateManager>().AddHealth(30);
            Debug.Log($"jfuiewhruewruweruewrwerew");
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

        //go to teamamet and do the thing
    }
}
