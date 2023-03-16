using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TmDefendPoint : TeamMateBaseState
{

    public override void EnterState(TeamMateStateManager teamMate)
    {

        UIManager.instance.SetIcon(1, teamMate.memberName);

        teamMate.currStateText = "DEFEND POINT";
        var list = CheckForEnemiesAround(teamMate);

        if (list.Count > 0) { teamMate.Allerted = true; }


        bool found = false;

        Collider[] hitColliders = Physics.OverlapSphere(teamMate.PatrolPoint, 15);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.transform.tag == "BasicCoverPos")
            {
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

    public override void OnUpdate(TeamMateStateManager teamMate)
    {

    }
}
