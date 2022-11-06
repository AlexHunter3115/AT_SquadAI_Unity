using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class TmFindCover : TeamMateBaseState
{

    public override void EnterState(TeamMateStateManager teamMate)
    {
        UIManager.instance.SetIcon(1, teamMate.memberName);

        teamMate.currStateText = "FIND COVER";
        var list = CheckForEnemiesAround(teamMate);

        if (list.Count > 0) { teamMate.Allerted = true; }


        Debug.Log($"{teamMate.memberName} is trying to find cover");
        //chekc distance in respect to the finish line
        if (teamMate.Allerted) // if in fight look for a 
        {
            bool found = false;

            Collider[] hitColliders = Physics.OverlapSphere(teamMate.transform.position, 15);
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
                            if (!RayCasterEnemyList(hitCollider.transform.position, list))
                            {

                                teamMate.currCoverTransformVector3 = hitCollider.transform.position;
                                teamMate.currCoverTransform = hitCollider.transform;
                                simpCoverScript.listOfAvailability[idx] = true;
                                var name = hitCollider.transform.name;

                                var worldPos = teamMate.currCoverTransform.localPosition;
                                if (name.Contains("Positive"))   // this two are the side ones   
                                {
                                    teamMate.currCoverType = TeamMateStateManager.CoverType.POSITIVE;
                                    var newWorldPos = new Vector3(worldPos.x, worldPos.y, worldPos.z - 0.35f);
                                    newWorldPos = teamMate.currCoverTransform.TransformPoint(newWorldPos);

                                    if (!RayCasterEnemyList(newWorldPos, list))
                                    {
                                        found = true;
                                        break;
                                    }

                                }
                                else if (name.Contains("Minus"))
                                {
                                    teamMate.currCoverType = TeamMateStateManager.CoverType.NEGATIVE;
                                    var newWorldPos = new Vector3(worldPos.x, worldPos.y, worldPos.z + 0.35f);
                                    newWorldPos = teamMate.currCoverTransform.TransformPoint(newWorldPos);

                                    if (!RayCasterEnemyList(newWorldPos, list))
                                    {
                                        found = true;
                                        break;
                                    }
                                }
                                else
                                {

                                    Vector3 adjustedPos = new Vector3(teamMate.currCoverTransform.position.x, teamMate.currCoverTransform.position.y + 1.1f, teamMate.currCoverTransform.position.z);

                                    teamMate.currCoverType = TeamMateStateManager.CoverType.FORWARD;
                                    if (!RayCasterEnemyList(adjustedPos, list))
                                    {
                                        found = true;
                                        break;
                                    }
                                }
                            }
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
                        var simpCoverScript = hitCollider.transform.GetComponentInParent<SimpleObjectCover>();

                        int idx = simpCoverScript.findIndexCoverTransforms(hitCollider.gameObject);

                        if (!simpCoverScript.listOfAvailability[idx])   // if the place is not taken
                        {
                            if (!RayCasterEnemyList(hitCollider.transform.position, list))
                            {

                                teamMate.currCoverTransformVector3 = hitCollider.transform.position;
                                teamMate.currCoverTransform = hitCollider.transform;
                                simpCoverScript.listOfAvailability[idx] = true;
                                var name = hitCollider.transform.name;

                                var worldPos = teamMate.currCoverTransform.localPosition;
                                if (name.Contains("Positive"))   // this two are the side ones   
                                {
                                    teamMate.currCoverType = TeamMateStateManager.CoverType.POSITIVE;
                                    var newWorldPos = new Vector3(worldPos.x, worldPos.y, worldPos.z - 0.35f);
                                    newWorldPos = teamMate.currCoverTransform.TransformPoint(newWorldPos);

                                    if (!RayCasterEnemyList(newWorldPos, list))
                                    {
                                        found = true;
                                        break;
                                    }

                                }
                                else if (name.Contains("Minus"))
                                {
                                    teamMate.currCoverType = TeamMateStateManager.CoverType.NEGATIVE;
                                    var newWorldPos = new Vector3(worldPos.x, worldPos.y, worldPos.z + 0.35f);
                                    newWorldPos = teamMate.currCoverTransform.TransformPoint(newWorldPos);

                                    if (!RayCasterEnemyList(newWorldPos, list))
                                    {
                                        found = true;
                                        break;
                                    }

                                }
                                else
                                {
                                    Vector3 adjustedPos = new Vector3(teamMate.currCoverTransform.position.x, teamMate.currCoverTransform.position.y + 1.1f, teamMate.currCoverTransform.position.z);

                                    teamMate.currCoverType = TeamMateStateManager.CoverType.FORWARD;
                                    if (!RayCasterEnemyList(adjustedPos, list))
                                    {
                                        found = true;
                                        break;
                                    }
                                }
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
                    teamMate.ChangeState(7);
                    Debug.Log($"sitting on my hands");
                }

            }
        }
        else
        {

            bool found = false;

            Collider[] hitColliders = Physics.OverlapSphere(teamMate.transform.position, 15);
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
                            if (RayCasterPlayer(hitCollider.transform.position, teamMate.PlayerObj.transform.position))
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

                        var simpCoverScript = hitCollider.transform.GetComponentInParent<SimpleObjectCover>();

                        int idx = simpCoverScript.findIndexCoverTransforms(hitCollider.gameObject);

                        if (!simpCoverScript.listOfAvailability[idx])   // if the place is not taken
                        {
                            //Debug.Log($"this si a free spot");
                            if (RayCasterPlayer(hitCollider.transform.position, teamMate.PlayerObj.transform.position))
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
    }
    public override void OnExit(TeamMateStateManager teamMate)
    {

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