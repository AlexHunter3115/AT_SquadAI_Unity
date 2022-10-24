using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class TeamMateBaseState
{


    // abstract should mean empty and ready to be filled
    public abstract void EnterState(TeamMateStateManager teamMate);
    public abstract void OnUpdate(TeamMateStateManager teamMate);

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
    { teamMate.Target = target; }

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
                    Debug.Log($"this should have returned true");
                    return true;
                }
            }
        }

        //or this 

        //if (teamMate.transform.GetComponent<NavMeshAgent>().pathStatus != NavMeshPathStatus.PathComplete)
        //{
        //    Debug.Log($" returning true");
        //    return true;
        //}
        //Debug.Log($"returning false");
        return false;

    }

    public bool RayCasterPlayer(Vector3 coverPos,Vector3 playerPos)
    {
        RaycastHit hit;
        if (Physics.Raycast(coverPos, (playerPos - coverPos), out hit))
        {
            if (hit.transform.tag == "Player")
            {
                return true;
            }
        }

        return false; 
    
    }

    public bool RayCasterEnemyList(Vector3 coverPos, List<Vector3> enemyPos)
    {
        RaycastHit hit;


        foreach (var enemy in enemyPos) 
        {
            if (Physics.Raycast(coverPos, (enemy - coverPos), out hit))
            {
                if (hit.transform.tag == "Player")
                {
                    return true;
                }
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
