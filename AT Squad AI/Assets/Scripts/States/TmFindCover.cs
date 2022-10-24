using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class TmFindCover : TeamMateBaseState
{
   

    // this all works just need to add a way for them to advance, maybe check for the parent how far it is from the goal, take them all sort array and then do the shit from there



    // for the agility 







    // prioritize the one closest to the core


    public override void EnterState(TeamMateStateManager teamMate)
    {
        Debug.Log($"{teamMate.SelAbility} is trying to find cover");
        //chekc distance in respect to the finish line
        if (teamMate.Allerted) // if in fight look for a 
        {
            bool found = false;

            Collider[] hitColliders = Physics.OverlapSphere(teamMate.transform.position, 10);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.transform.tag == "BasicCoverPos")
                {
                    Debug.Log($"found something in the tag");

                    var simpCoverScript = hitCollider.transform.GetComponentInParent<SimpleObjectCover>();

                    int idx = simpCoverScript.findIndexCoverCubes(hitCollider.gameObject);


                    if (!simpCoverScript.listOfAvailability[idx])   // if the place is not taken
                    {

                        Debug.Log($"this si a free spot");
                        if (RayCasterPlayer(hitCollider.transform.position, teamMate.PlayerObj.transform.position))
                        {

                            teamMate.currCoverTransform = hitCollider.transform.position;

                            var name = hitCollider.transform.name;
                            if (name.Contains("Positive"))   // this two are the side ones   
                            {
                                teamMate.currCoverType = TeamMateStateManager.CoverType.POSITIVE;
                            }
                            else if (name.Contains("Minus"))
                            {
                                teamMate.currCoverType = TeamMateStateManager.CoverType.NEGATIVE;
                            }
                            else
                            {
                                teamMate.currCoverType = TeamMateStateManager.CoverType.FORWARD;
                            }
                            simpCoverScript.listOfAvailability[idx] = true;
                            found = true;
                            break;
                        }
                    }
                    else
                    {
                        Debug.Log($"this spot is not free");
                    }
                }
            }

            if (found)
            {
                Debug.Log($"we found an avaialble spot to cover, it is at {teamMate.currCoverTransform}");
                
                    teamMate.ChangeState(2);
                
            }
            else
            {
                Debug.Log($"Could not find a point to take cover at");
                teamMate.ChangeState(6);
            }
        }
        else
        {

            bool found = false;

            Collider[] hitColliders = Physics.OverlapSphere(teamMate.transform.position, 10);
            foreach (var hitCollider in hitColliders)
            {
                Debug.Log($"did this even call {hitCollider.transform.tag}");
                if (hitCollider.transform.tag == "BasicCoverPos")
                {
                    Debug.Log($"found something in the tag");

                    var simpCoverScript = hitCollider.transform.GetComponentInParent<SimpleObjectCover>();

                    int idx = simpCoverScript.findIndexCoverTransforms(hitCollider.gameObject);
                    Debug.Log($"{idx}");
                    Debug.Log($"{hitCollider.name}");

                    if (!simpCoverScript.listOfAvailability[idx])   // if the place is not taken
                    {
                        Debug.Log($"this si a free spot");
                        if (RayCasterPlayer(hitCollider.transform.position, teamMate.PlayerObj.transform.position))
                        {

                            teamMate.currCoverTransform = hitCollider.transform.position;
                            simpCoverScript.listOfAvailability[idx] = true;
                            var name = hitCollider.transform.name;
                            if (name.Contains("Positive"))   // this two are the side ones   
                            {
                                teamMate.currCoverType = TeamMateStateManager.CoverType.POSITIVE;
                            }
                            else if (name.Contains("Minus"))
                            {
                                teamMate.currCoverType = TeamMateStateManager.CoverType.NEGATIVE;
                            }
                            else
                            {
                                teamMate.currCoverType = TeamMateStateManager.CoverType.FORWARD;
                            }

                            found = true;
                            break;
                        }
                    }
                    else 
                    {
                        Debug.Log($"this spot is not free");
                    }
                }
            }

            if (found) 
            {
                Debug.Log($"we found an avaialble spot to cover, it is at {teamMate.currCoverTransform}");
               
                   teamMate.ChangeState(2);
                
              
            }
            else 
            {
                Debug.Log($"Could not find a point to take cover at");
                teamMate.ChangeState(3);
            }
        }

    }


    // this should find a cover we dont really need the onupdate as the go to cover will deal with that

    public override void OnUpdate(TeamMateStateManager teamMate)
    {

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