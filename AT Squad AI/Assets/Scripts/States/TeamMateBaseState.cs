using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static Unity.VisualScripting.Member;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.InputSystem.Controls.AxisControl;

public abstract class TeamMateBaseState
{


    // abstract should mean empty and ready to be filled
    public abstract void EnterState(TeamMateStateManager teamMate);
    public abstract void OnUpdate(TeamMateStateManager teamMate);
    public abstract void OnExit(TeamMateStateManager teamMate);

    public void SmoothRotateTowards(Quaternion lookAt, TeamMateStateManager teamMate)
    {
        teamMate.transform.rotation = Quaternion.Slerp(teamMate.transform.rotation, lookAt, Time.deltaTime * teamMate.RotSpeed);
    }

    public float TimerCheck(float coolDown, float timer) 
    { 
        if ( Time.time >= coolDown + timer  ) 
        {
            return Time.time; }
        else 
        {
            return -1; 
        } 
    }

    public void GoToPoint(Vector3 pos, TeamMateStateManager teamMate)
    { teamMate.NavMeshAgent.destination = pos; }

    public Quaternion GenRandomRot()
    { return Quaternion.Euler(new Vector3(0.0f, Random.Range(0f, 359f), 0.0f)); }
    //to change
    public void ShootAt(GameObject target, TeamMateStateManager teamMate)
    {   teamMate.Target = target;


        Vector3 fromPosition = teamMate.transform.position;
        Vector3 toPosition = target.transform.position;
        Vector3 direction = toPosition - fromPosition;


        if (Time.time > teamMate.lastFire + teamMate.fireRate)
        {
            teamMate.lastFire = Time.time;
            RaycastHit outHit;
            if (Physics.Raycast(teamMate.transform.position + new Vector3(0,0.5f,0), direction, out outHit, Mathf.Infinity, PlayerScript.instance.Hittable))
            {

                //Debug.Log($"{outHit.transform.name}");


                if (outHit.transform.tag == "Enemy")
                {
                    outHit.transform.GetComponentInParent<EnemyScript>().TakeDamage(10);
                }
            }
        }

    }

    public void LookAt(GameObject target, TeamMateStateManager teamMate)
    {
        var enemyPos = target.transform.position;
        enemyPos.y = teamMate.transform.position.y;
        teamMate.transform.LookAt(enemyPos);
    }

    public bool ReachedDestination(TeamMateStateManager teamMate) 
    {
        var navMesh = teamMate.transform.GetComponent<NavMeshAgent>();

        if (!navMesh.pathPending)
        {
            if (navMesh.remainingDistance <= navMesh.stoppingDistance)
            {
                if (!navMesh.hasPath || navMesh.velocity.sqrMagnitude == 0f)
                {
                    //Debug.Log($"this should have returned true");
                    return true;
                }
            }
        }

        return false;

    }

    public bool RayCasterPlayer(Vector3 coverPos,Vector3 playerPos)
    {
        RaycastHit hit;
        if (Physics.Raycast(coverPos, (playerPos - coverPos), out hit, Mathf.Infinity, ~6))
        {
            if (hit.transform.tag == "Player")
            {
                return true;
            }
        }

        return false; 
    
    }

    public bool RayCasterPoint(Vector3 coverPos, Vector3 pointPos)
    {
        RaycastHit hit;
        if (Physics.Raycast(coverPos, (pointPos - coverPos), out hit, Vector3.Distance(coverPos, pointPos) , ~6))
        {
            Debug.Log($"{hit.transform.name}");
                return true;
            
        }
        else 
        {
            return false;
        }

    }



    /// <summary>
    /// false if there is no detected enemy
    /// true if there is an enemy in sight
    /// </summary>
    /// <param name="coverPos"></param>
    /// <param name="enemyPos"></param>
    /// <returns></returns>
    public bool RayCasterEnemyList(Vector3 coverPos, List<GameObject> enemyPos)
    {
        RaycastHit hit;

        foreach (var enemy in enemyPos)
        {

            if (Physics.Linecast(coverPos, enemy.transform.position,out hit, SquadManager.instance.teamMates[0].GetComponent<TeamMateStateManager>().ignoreCoverLayermask))
            {
                if (hit.transform.tag == "Enemy")
                {
                    return true;
                }
                else
                {
                }
            }
            else 
            {
                return true;
            }

        }


        return false;

    }

    public List<GameObject> CheckForEnemiesAround(TeamMateStateManager teamMate) 
    {

        List<GameObject> enemiesList = new List<GameObject> ();

        Collider[] hitColliders = Physics.OverlapSphere(teamMate.transform.position, 10);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.transform.tag == "Enemy")
            {
                enemiesList.Add(hitCollider.gameObject);
            }
        }


        return enemiesList;

    }
 
    


}
