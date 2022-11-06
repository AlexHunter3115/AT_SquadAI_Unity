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


    public float lastFire;
    public float fireRate = 0.4f;

    private GameObject point;

    public NavMeshAgent agent;

    private bool move;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();



        agent.destination = enemySpawnerManager.instance.point.position;
        move = true;
        agent.isStopped = false;
    }

    public void TakeDamage(int _damage) 
    {
        health = health - _damage;

        if (health < 0) { Destroy(this.gameObject); }
        else { healthSlider.value = health; }

        shotAt = true;
    }

    public void CallMove() 
    {

        agent.destination = enemySpawnerManager.instance.point.position;
        move = true;
    }


    private void Update()
    {

        List<GameObject> teamMates = new List<GameObject>();

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 10);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.transform.tag == "TeamMate" && hitCollider.transform.root.GetComponent<TeamMateStateManager>().Health != 0)
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
        else if (teamMates.Count == 0 && shotAt == false)
        {
            if (move)
             agent.isStopped = false;
        }


        if (shotAt) 
        {
            if (standingBody.activeSelf) 
            {
                standingBody.SetActive(false);
                crouchingBody.SetActive(true);
                lastStanding = Time.time;
                if (move)
                    agent.isStopped = true;
            }


            if (Time.time > lastStanding + standingRate)
            {
                lastStanding = Time.time;
                standingBody.SetActive(true);
                crouchingBody.SetActive(false);
                shotAt = false;
            }
        }
    }





    private void ShootAt(List<GameObject> teamMates) 
    {
        var _direction = (teamMates[0].transform.position - transform.position).normalized;

        var x = (1 - 2 * Random.value) * 0.05f;
        var y = (1 - 2 * Random.value) * 0.05f;


        Vector3 newDir = this.transform.TransformDirection(new Vector3(x, y, 1));

        if (Time.time > lastFire + fireRate)
        {

            lastFire = Time.time;
            RaycastHit outHit;
            if (Physics.Raycast(this.transform.position, newDir, out outHit, Mathf.Infinity, PlayerScript.instance.Hittable))
            {

                if (outHit.transform.tag == "TeamMate")
                {
                    outHit.transform.root.GetComponent<TeamMateStateManager>().TakeDamage(0);
                }
              

                GameObject newRef = Instantiate(PlayerScript.instance.bulletPrefab);
                newRef.transform.position = outHit.point;
                newRef.transform.parent = outHit.transform;
            }
        }





        _direction.y = 0;

        //create the rotation we need to be in to look at the target
        var _lookRotation = Quaternion.LookRotation(_direction);

        this.transform.rotation = _lookRotation;








    }
}
