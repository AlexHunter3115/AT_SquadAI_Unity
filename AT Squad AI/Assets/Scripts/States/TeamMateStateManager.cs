using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamMateStateManager : MonoBehaviour
{
    TeamMateBaseState currState;

    TeamMateAlertinCover TeamMateAlertinCover = new TeamMateAlertinCover();
    TeamMateIdleCover TeamMateIdleCover = new TeamMateIdleCover();
    TeamMateIdleWalk TeamMateIdleWalk = new TeamMateIdleWalk();
    TeamMateInFormation TeamMateInFormation = new TeamMateInFormation();
    TeamMateWalkingToCover TeamMateWalkingToCover = new TeamMateWalkingToCover();



    private void Start()
    {
        currState = TeamMateIdleWalk;

        currState.EnterState(this);
    }





    private void Update()
    {
        currState.OnUpdate(this);   
    }

}
