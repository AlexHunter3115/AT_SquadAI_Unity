using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TmAdvance : TeamMateBaseState
{
    // called when the thing dies, comesback to life when the medic heals
    public override void EnterState(TeamMateStateManager teamMate)
    {
        UIManager.instance.SetIcon(1, teamMate.memberName);

        teamMate.currStateText = "ADVANCING";

        Collider[] hitColliders = Physics.OverlapSphere(teamMate.transform.position, 20);

        if (hitColliders.Length > 0)
        {
            int _idx = 0;
            float distance = 9999;

            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].transform.tag == "BasicCoverPos")
                {
                    if (Vector3.Distance(hitColliders[i].transform.position, PlayerScript.instance.endPoint.transform.position) < distance)
                    {
                        var simpCoverScripts = hitColliders[i].transform.GetComponentInParent<SimpleObjectCover>();

                        if (!simpCoverScripts.listOfAvailability[simpCoverScripts.findIndexCoverTransforms(hitColliders[i].gameObject)])   // if the place is not taken
                        {
                            distance = Vector3.Distance(hitColliders[i].transform.position, PlayerScript.instance.endPoint.transform.position);
                            _idx = i;
                        }
                    }
                }
            }

            var simpCoverScript = hitColliders[_idx].transform.GetComponentInParent<SimpleObjectCover>();

            int idx = simpCoverScript.findIndexCoverTransforms(hitColliders[_idx].gameObject);

            if (!simpCoverScript.listOfAvailability[idx])   // if the place is not taken
            {
                teamMate.currCoverTransformVector3 = hitColliders[_idx].transform.position;
                teamMate.currCoverTransform = hitColliders[_idx].transform;
                simpCoverScript.listOfAvailability[idx] = true;
                var name = hitColliders[_idx].transform.name;

                //var worldPos = teamMate.currCoverTransform.localPosition;
                if (name.Contains("Positive"))   // this two are the side ones   
                {
                    teamMate.currCoverType = TeamMateStateManager.CoverType.POSITIVE;
                    //var newWorldPos = new Vector3(worldPos.x, worldPos.y, worldPos.z - 0.35f);
                    //newWorldPos = teamMate.currCoverTransform.TransformPoint(newWorldPos);
                }
                else if (name.Contains("Minus"))
                {
                    teamMate.currCoverType = TeamMateStateManager.CoverType.NEGATIVE;
                    //var newWorldPos = new Vector3(worldPos.x, worldPos.y, worldPos.z + 0.35f);
                    //newWorldPos = teamMate.currCoverTransform.TransformPoint(newWorldPos);
                }
                else
                {
                    //Vector3 adjustedPos = new Vector3(teamMate.currCoverTransform.position.x, teamMate.currCoverTransform.position.y + 1.1f, teamMate.currCoverTransform.position.z);

                    teamMate.currCoverType = TeamMateStateManager.CoverType.FORWARD;
                }
            }


            teamMate.ChangeState(2);
        }
        //teamMate.ChangeState(7);
    }
    public override void OnExit(TeamMateStateManager teamMate)
    {

    }
    public override void OnUpdate(TeamMateStateManager teamMate)
    {

    }
}
