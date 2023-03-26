using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class TeamMateBaseState
{
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

    public void GoToPoint(Vector3 pos, TeamMateStateManager teamMate) => teamMate.NavMeshAgent.destination = pos;

    public Quaternion GenRandomRot()
    { return Quaternion.Euler(new Vector3(0.0f, Random.Range(0f, 359f), 0.0f)); }
    //to change
    public void ShootAt(GameObject target, TeamMateStateManager teamMate)
    {   
        teamMate.Target = target;

        Vector3 fromPosition = teamMate.transform.position;
        Vector3 toPosition = target.transform.position;
        Vector3 direction = toPosition - fromPosition;

        LookAt(target, teamMate);

        if (Time.time > teamMate.lastFire + teamMate.fireRate)
        {
            teamMate.lastFire = Time.time;
            RaycastHit hit;
            if (Physics.Raycast(teamMate.transform.position + new Vector3(0,0.5f,0), direction, out hit, Mathf.Infinity, PlayerScript.instance.Hittable))
            {
                if (hit.transform.tag == "Enemy")
                {
                    hit.transform.GetComponentInParent<EnemyScript>().TakeDamage(Random.Range(4,10));
                }
                teamMate.AnimatorSetter(0);

                teamMate.InstaStuff(hit);
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
                Debug.DrawLine(coverPos, hit.point, Color.yellow, 90);
                return true;
            }
        }

        return false; 
    }

    public bool RayCasterPoint(Vector3 coverPos, Vector3 pointPos)
    {
        RaycastHit hit;
        if (Physics.Linecast(coverPos, pointPos, out hit, ~6))
        {
            Debug.DrawLine(pointPos, hit.point,Color.green,90);
            return true;
        }
        else 
        {
            Debug.DrawLine(pointPos, coverPos, Color.blue, 90);
            return false;
        }
    }

    public void SetMessage(string text, Color color) => UIManager.instance.SendMessage(text, color);
  
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
            if (Physics.Linecast(coverPos, enemy.transform.position,out hit, SquadManager.instance.ignoreCoverLayermask))
            {
                if (hit.transform.tag == "Enemy")
                {
                    Debug.DrawLine(coverPos, hit.point, Color.red, 90);
                    return true;
                }
                else
                {
                    Debug.DrawLine(coverPos, hit.point, Color.red, 90);
                }
            }
            else 
            {
                Debug.DrawLine(coverPos, enemy.transform.position, Color.red, 90);
                return true;
            }

        }


        return false;

    }

    public List<GameObject> CheckForEnemiesAround(TeamMateStateManager teamMate) 
    {
        List<GameObject> enemiesList = new List<GameObject> ();

        Collider[] hitColliders = Physics.OverlapSphere(teamMate.transform.position, 12);
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
