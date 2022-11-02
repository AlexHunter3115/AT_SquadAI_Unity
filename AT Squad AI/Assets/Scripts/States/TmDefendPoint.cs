using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TmDefendPoint : TeamMateBaseState
{

    // this all works just need to add a way for them to advance, maybe check for the parent how far it is from the goal, take them all sort array and then do the shit from there



    // for the agility 







    // prioritize the one closest to the core


    public override void EnterState(TeamMateStateManager teamMate)
    {

        UIManager.instance.SetIcon(1, teamMate.memberName);

        teamMate.currStateText = "DEFEND POINT";
        var list = CheckForEnemiesAround(teamMate);

        if (list.Count > 0) { teamMate.Allerted = true; }




        Debug.Log($"{teamMate.memberName} is trying to defend point  cover");
        

            bool found = false;

            Collider[] hitColliders = Physics.OverlapSphere(teamMate.PatrolPoint, 15);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.transform.tag == "BasicCoverPos")
                {
                //Debug.Log($"vccvcxvcvcvcvcvcvc");
                    var simpCoverScript = hitCollider.transform.GetComponentInParent<SimpleObjectCover>();

                    if (!simpCoverScript.SpotsTaken())
                    {
                        int idx = simpCoverScript.findIndexCoverTransforms(hitCollider.gameObject);

                        if (!simpCoverScript.listOfAvailability[idx])   // if the place is not taken
                        {
                            if (RayCasterPoint(hitCollider.transform.position, teamMate.PatrolPoint))
                            {

                                teamMate.currCoverTransformVector3 = hitCollider.transform.position;

                                teamMate.currCoverTransform = hitCollider.transform;
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
                        }
                    }
                }
            }

            if (found)
            {
                teamMate.ChangeState(2);
            }
            else
            {

                found = false;

                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider.transform.tag == "BasicCoverPos")
                    {
                    //Debug.Log($"fdgfgfdgfgddfgfg");
                        var simpCoverScript = hitCollider.transform.GetComponentInParent<SimpleObjectCover>();

                        int idx = simpCoverScript.findIndexCoverTransforms(hitCollider.gameObject);

                        if (!simpCoverScript.listOfAvailability[idx])   // if the place is not taken
                        {
                            //Debug.Log($"this si a free spot");
                            if (RayCasterPoint(hitCollider.transform.position, teamMate.PatrolPoint))
                            {
                            Debug.Log($"");
                                teamMate.currCoverTransformVector3 = hitCollider.transform.position;

                                teamMate.currCoverTransform = hitCollider.transform;
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
                    }
                }



                if (found)
                {
                    teamMate.ChangeState(2);
                }
                else
                {
                    teamMate.ChangeState(3);
                }
            }
        
    }
    public override void OnExit(TeamMateStateManager teamMate)
    {

    }

    // this should find a cover we dont really need the onupdate as the go to cover will deal with that

    public override void OnUpdate(TeamMateStateManager teamMate)
    {

    }
}
