using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TmbehindCoverFrontFight : TeamMateBaseState
{

    private bool hasMoved;
    private bool showing;

    private float showingCooldown;
    private float showingTimer;

    private float notShowingCooldown;
    private float notShowingTimer;


    List<GameObject> list = new List<GameObject>();

    //literally does nothing, they just spawned they just look around, still have some enemy check but overall do nothing
    // also we need to have propriaterys timers here one for the cooldown and one for the reantry
    public override void EnterState(TeamMateStateManager teamMate)
    {
        UIManager.instance.SetIcon(2, teamMate.memberName);
        showing = false;
        Debug.Log(teamMate.transform.name + " is in the front fight state ");
        teamMate.currStateText = "BCFF";

        showingCooldown = 3f; //set the timer for how long the obj is going to be showing
        notShowingCooldown = 1f;

        teamMate.transform.GetChild(3).gameObject.SetActive(false);
        teamMate.transform.GetChild(2).gameObject.SetActive(true);

        ShowSelf(teamMate);
    }

    public override void OnExit(TeamMateStateManager teamMate)
    {


        if (teamMate.changingToState == 5)
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

    public override void OnUpdate(TeamMateStateManager teamMate)
    {


        list = CheckForEnemiesAround(teamMate);

        if (showing)
        {

            showingTimer += Time.deltaTime;

            if (list.Count > 0)
                ShootAt(list[0], teamMate);

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









    // is where we see if there are any enemies left
    public void ShowSelf(TeamMateStateManager teamMate)
    {
        if (showing)
        {

            teamMate.transform.GetChild(3).gameObject.SetActive(false);
            teamMate.transform.GetChild(2).gameObject.SetActive(true);

            //teamMate.transform.GetComponent<MeshRenderer>().material.color = Color.blue;

            List<GameObject> list = CheckForEnemiesAround(teamMate);
            if (list.Count == 0)
            {
                teamMate.ChangeState(5);
            }
        }
        else
        {

            //teamMate.transform.GetComponent<MeshRenderer>().material.color = Color.green;
            //hide
            teamMate.transform.GetChild(3).gameObject.SetActive(true);
            teamMate.transform.GetChild(2).gameObject.SetActive(false);
        }
    }


}
