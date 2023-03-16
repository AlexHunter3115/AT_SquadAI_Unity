using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.InputSystem.Controls.AxisControl;

public class EnemyScript : MonoBehaviour
{
    public int health = 100;

    public Slider healthSlider;

    public bool shotAt = false;

    public GameObject target;

    public List<GameObject> targetsList = new List<GameObject>();

    public GameObject standingBody;
    public GameObject crouchingBody;

    public float lastStanding;
    public float standingRate = 3f;

    [SerializeField] private Animator animator;

    public float lastFire;
    public float fireRate = 0.4f;

    private GameObject point;

    public bool test = false;

    public NavMeshAgent agent;

    private bool move;

    private bool dead = false;

    [SerializeField] GameObject[] muzzleEffect = new GameObject[5];
    [SerializeField] GameObject hitEffect;
    [SerializeField] GameObject muzzlePoint;

    private void Start()
    {
        SetAnim(2);

        agent = GetComponent<NavMeshAgent>();

        agent.destination = enemySpawnerManager.instance.point.position;
        move = true;
        agent.isStopped = false;
    }

    public void TakeDamage(int _damage) 
    {
        if (dead) { return; }
        health = health - _damage;

        if (health < 0) 
        {
            dead = true;
            SetAnim(1);
        }
        else { healthSlider.value = health; }

        shotAt = true;
    }

    public void CallMove() 
    {
        agent.destination = enemySpawnerManager.instance.point.position;
        SetAnim(4);
        move = true;
    }


    private void Update()
    {
        if (dead) { return; }

        List<GameObject> teamMates = new List<GameObject>();

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 10);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.transform.tag == "TeamMate" && hitCollider.transform.root.GetComponent<TeamMateStateManager>().Alive == true)
            {
                teamMates.Add(hitCollider.gameObject);
            }
        }

        if (teamMates.Count > 0)
        {
            ShootAt(teamMates);

            if (move)
                agent.isStopped = true;
        }
        else if (teamMates.Count == 0 && shotAt == false) //idle
        {
            if (move)
             agent.isStopped = false;
        }

        if(agent.velocity.magnitude > 0.4f  &&  !agent.isStopped )
        {
            SetAnim(4);
        }
        else 
        {
            SetAnim(2);
        }
    }


    private void ShootAt(List<GameObject> teamMates) 
    {
        var _direction = (teamMates[0].transform.position - transform.position).normalized;

        var x = (1 - 2 * Random.value) * 0.15f;
        var y = (1 - 2 * Random.value) * 0.15f;

        Vector3 newDir = this.transform.TransformDirection(new Vector3(x, y, 1));

        if (Time.time > lastFire + fireRate)
        {

            SetAnim(0);
            lastFire = Time.time;
            RaycastHit outHit;
            if (Physics.Raycast(this.transform.position, newDir, out outHit, Mathf.Infinity, PlayerScript.instance.Hittable))
            {

                if (outHit.transform.tag == "TeamMate")
                {
                    outHit.transform.root.GetComponent<TeamMateStateManager>().TakeDamage(5);
                }
              
                GameObject newRef = Instantiate(PlayerScript.instance.bulletPrefab);
                newRef.transform.position = outHit.point;
                newRef.transform.parent = outHit.transform;

                var objs = Instantiate(muzzleEffect[Random.Range(0, 5)], muzzlePoint.transform.position, muzzlePoint.transform.rotation);

                objs.transform.parent = transform;

                Instantiate(hitEffect, outHit.point, Quaternion.identity);

            }
        }

        _direction.y = 0;

        //create the rotation we need to be in to look at the target
        var _lookRotation = Quaternion.LookRotation(_direction);

        this.transform.rotation = _lookRotation;

    }

    /// <summary>
    /// 0 shoot on -- 1 die  --  2 idle -- 3 duck -- 4 run 
    /// </summary>
    /// <param name="animationIndex"></param>
    public void SetAnim(int animationIndex) 
    {
        switch (animationIndex)
        {
            case 0://shoot

                animator.SetTrigger("Shoot");
                break;

            case 1://die

                animator.SetTrigger("Die");
                StartCoroutine(CallDead());
                break;

            case 2:// idle
                animator.SetBool("Idle", true);
                animator.SetBool("Move", false);
                break;

            case 4://run
                animator.SetBool("Move", true);
                animator.SetBool("Idle", false);
                break;

        
            default:
                break;
        }
    }


    private IEnumerator CallDead() 
    {
        yield return new WaitForSeconds(2f);
        PlayerScript.instance.enemiesKilled++;
        Destroy(gameObject);
    }

    public bool ReachedDestination()
    {
        var navMesh = agent;

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


}
