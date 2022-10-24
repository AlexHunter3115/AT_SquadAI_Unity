using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TeamMateStateManager : MonoBehaviour
{
    TeamMateBaseState currState;
    public TeamMateBaseState CurrState 
    {
        get { return CurrState; }
    }

    private TeamMateBaseState[] statesList = new TeamMateBaseState[12] 
    {
        new TmDead(),
        new TmFindCover(),
        new TmGoToCover(),
        new TmIdleWaiting(),
        new TmbehindCoverFrontFight(),
        new TmbehindCoverFrontIdle(),
        new TmInFormationFight(),
        new TmInFormationIdle(),
        new TmPatrollingAroundPoint(),
        new TmUseAbility(),
        new TmbehindCoverLateralActive(),
        new TmbehindCoverLateralIdle()
    };


    private GameObject target;
    public GameObject Target
    {
        get { return target; }
        set { if (value.transform.tag == "Enemy") { target = value; }  }
    }


    public enum AbilityType
    {
        GRANADIER = 0,
        MEDIC = 1,
        ROPE = 2
    }

    private AbilityType selAbility;
    public AbilityType SelAbility
    {
        get { return selAbility; }
    }


    private bool alive;
    public bool Alive
    {
        get { return alive; }
        set { alive = value; }
    }


    private bool allerted;
    public bool Allerted
    {
        get { return allerted; }
        set { allerted = value; }
    }


    private int health;
    public int Health
    {
        get { return health; }
    }


    private NavMeshAgent navMeshAgent;
    public NavMeshAgent NavMeshAgent
    {
        get { return navMeshAgent; }
    }


    private int rotSpeed = 4;
    public int RotSpeed 
    {
        get { return rotSpeed;}
    }


    private GameObject playerObj;
    public GameObject PlayerObj
    {
        get { return playerObj; }
    }


    private Transform formationTran;
    public Transform FormationTran
    {
        get { return formationTran; }
        set { formationTran = value; }
    }
    public float formationTimer;
    public float formationCooldown;


    public float shootingTimer; 
    public float shootingCooldown;   // should chnage based on class



    public float coverTimer;
    public float coverCooldown;


    public string memberName;

    public enum CoverType 
    {
        POSITIVE,
        NEGATIVE,
        FORWARD,
        NONE       
    }

    public CoverType currCoverType;

    public Vector3 currCoverTransform;







    public List<GameObject> enemyList; 






    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();



        memberName =  Random.Range(0, 9999).ToString();


        health = 100;

        selAbility = (AbilityType)Random.Range(0, 2);
        switch (selAbility)
        {
            case AbilityType.GRANADIER:
                navMeshAgent.speed = 2.5f;
                break;

            case AbilityType.MEDIC:
                navMeshAgent.speed = 3.5f;
                break;

            case AbilityType.ROPE:
                navMeshAgent.speed = 4.5f;
                break;

            default:
                break;
        }
        allerted = false;
        alive = true;
    }



    private void Start()
    {
        currState = statesList[3];
        currState.EnterState(this);

        playerObj = GameObject.FindGameObjectWithTag("Player");
    }


    private void Update()
    {
        if (alive && health <= 0) 
        {
            alive = false;
            ChangeState(0);     
        }
        Debug.Log($"{allerted}");
        //if (enemyList.Count > 0) 
        //{
        //    allerted = true;
        //}
        //else 
        //{
        //    allerted = false;
        //}


        currState.OnUpdate(this);   
    }


    public void ChangeState(int state)
    {
        currState = statesList[state];

        currState.EnterState(this);
    }





    public void AddHealth(int medkitAmount) 
    { 
        health =+ medkitAmount;

        if (health >= 100) { health = 100; }
    }

}
