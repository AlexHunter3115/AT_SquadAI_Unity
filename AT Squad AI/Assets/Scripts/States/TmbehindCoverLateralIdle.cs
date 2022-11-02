using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TmbehindCoverLateralIdle : TeamMateBaseState
{

    private bool showing;

    private float showingCooldown;
    private float showingTimer;

    private float notShowingCooldown;
    private float notShowingTimer;


    //literally does nothing, they just spawned they just look around, still have some enemy check but overall do nothing
    // also we need to have propriaterys timers here one for the cooldown and one for the reantry
    public override void EnterState(TeamMateStateManager teamMate)
    {


        UIManager.instance.SetIcon(3, teamMate.memberName);
        showing = false;
        Debug.Log(teamMate.transform.name + " is in the lateral idle state ");
        teamMate.currStateText = "BCLI";

        showingCooldown = 3f; //set the timer for how long the obj is going to be showing
        notShowingCooldown = 1f;


        ShowSelf(teamMate);
    }

    public override void OnUpdate(TeamMateStateManager teamMate)
    {


        if (showing && ReachedDestination(teamMate))
        {
            showingTimer += Time.deltaTime;

            if (showingTimer >= showingCooldown)
            {
                showingTimer = 0;
                showing = !showing;
                ShowSelf(teamMate);
            }



            //shoot


        }
        else if (!showing && ReachedDestination(teamMate))
        {
            notShowingTimer += Time.deltaTime;

            if (notShowingTimer >= notShowingCooldown)
            {
                notShowingTimer = 0;
                showing = !showing;
                ShowSelf(teamMate);
            }
        }

    }





    public override void OnExit(TeamMateStateManager teamMate)
    {
        if (teamMate.changingToState == 10)
        {

        }
        else
        {
            var simpCoverScript = teamMate.currCoverTransform.transform.GetComponentInParent<SimpleObjectCover>();
            int idx = simpCoverScript.findIndexCoverTransforms(teamMate.currCoverTransform.gameObject);
            simpCoverScript.listOfAvailability[idx] = false;
        }
    }



    // is where we see if there are any enemies left
    public void ShowSelf(TeamMateStateManager teamMate)
    {
        if (showing)
        {

            teamMate.transform.GetComponent<MeshRenderer>().material.color = Color.blue;

            var worldPos = teamMate.currCoverTransform.localPosition;

           


            if (teamMate.currCoverType == TeamMateStateManager.CoverType.NEGATIVE)
            {

                var newWorldPos = new Vector3(worldPos.x, worldPos.y, worldPos.z + 0.35f);
                newWorldPos = teamMate.currCoverTransform.TransformPoint(newWorldPos);
                GoToPoint(newWorldPos, teamMate);

            }
            else if (teamMate.currCoverType == TeamMateStateManager.CoverType.POSITIVE)
            {
                var newWorldPos = new Vector3(worldPos.x, worldPos.y, worldPos.z - 0.35f);
                newWorldPos = teamMate.currCoverTransform.TransformPoint(newWorldPos);
                GoToPoint(newWorldPos, teamMate);
            }





            var list = CheckForEnemiesAround(teamMate);
            if (list.Count > 0)
            {
                teamMate.ChangeState(4);
            }
        }
        else
        {

            teamMate.transform.GetComponent<MeshRenderer>().material.color = Color.red;


            if (teamMate.currCoverType == TeamMateStateManager.CoverType.NEGATIVE)
            {
                GoToPoint(teamMate.currCoverTransformVector3, teamMate);

            }
            else if (teamMate.currCoverType == TeamMateStateManager.CoverType.POSITIVE)
            {


                GoToPoint(teamMate.currCoverTransformVector3, teamMate);
            }



            //could make it go to the point




            //hide
        }
    }
}
