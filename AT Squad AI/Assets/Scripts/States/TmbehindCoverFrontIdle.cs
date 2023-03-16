using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TmbehindCoverFrontIdle : TeamMateBaseState
{


    private bool showing;

    private float showingCooldown;
    private float showingTimer;

    private float notShowingCooldown;
    private float notShowingTimer;

    private bool hasMoved;


    //literally does nothing, they just spawned they just look around, still have some enemy check but overall do nothing
    // also we need to have propriaterys timers here one for the cooldown and one for the reantry
    public override void EnterState(TeamMateStateManager teamMate)
    {

        UIManager.instance.SetIcon(3, teamMate.memberName);

        showing = false;
        Debug.Log(teamMate.transform.name + " is in the front idle state ");
        teamMate.currStateText = "BCFI";

        showingCooldown = 3f; //set the timer for how long the obj is going to be showing
        notShowingCooldown = 1f;


        ShowSelf(teamMate);
    }

    public override void OnUpdate(TeamMateStateManager teamMate)
    {



        if (showing) 
        {
            showingTimer += Time.deltaTime;

            if (showingTimer >= showingCooldown)
            {
                showingTimer = 0;
                showing = !showing;
                ShowSelf(teamMate);
            }
        }
        else 
        {
            notShowingTimer += Time.deltaTime;

            if (notShowingTimer >= notShowingCooldown && !teamMate.holdFire)
            {
                notShowingTimer = 0;
                showing = !showing;
                ShowSelf(teamMate);
            }
        }
    }




    public override void OnExit(TeamMateStateManager teamMate)
    {

        if (teamMate.changingToState == 4) 
        {
            
        }
        else 
        {
            var simpCoverScript = teamMate.currCoverTransform.transform.GetComponentInParent<SimpleObjectCover>();
            int idx = simpCoverScript.findIndexCoverTransforms(teamMate.currCoverTransform.gameObject);
            simpCoverScript.listOfAvailability[idx] = false;

        }

        teamMate.transform.GetChild(3).gameObject.SetActive(false);

        teamMate.transform.GetChild(2).gameObject.SetActive(true);

    }




    // is where we see if there are any enemies left
    public void ShowSelf(TeamMateStateManager teamMate)
    {
        if (showing)
        {
            teamMate.transform.GetChild(3).gameObject.SetActive(false);
            teamMate.transform.GetChild(2).gameObject.SetActive(true);

            teamMate.AnimatorSetter(3);
            //teamMate.transform.GetComponent<MeshRenderer>().material.color = Color.blue;
            List<GameObject> list =  CheckForEnemiesAround(teamMate);
            if (list.Count > 0) 
            {
                teamMate.ChangeState(4);
            }
        }
        else
        {

            //teamMate.transform.GetComponent<MeshRenderer>().material.color = Color.green;
            //hide
            teamMate.transform.GetChild(3).gameObject.SetActive(true);
            teamMate.transform.GetChild(2).gameObject.SetActive(false);


            if (teamMate.NavMeshAgent.remainingDistance < 0.1f)
            {
                teamMate.AnimatorSetter(4);
            }

        }
    }



}
