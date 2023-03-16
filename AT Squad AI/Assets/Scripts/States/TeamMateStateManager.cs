using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

public class TeamMateStateManager : MonoBehaviour
{
    TeamMateBaseState currState;
    public TeamMateBaseState CurrState 
    {
        get { return CurrState; }
    }

    [SerializeField] Animator animator;


    // need to add the advance cover 
    // change the way around point works
    // add the defend point
    // add the shooting 
    // check the anims
    
    private TeamMateBaseState[] statesList = new TeamMateBaseState[17] 
    {
        new TmDead(),  //0 
        new TmFindCover(),
        new TmGoToCover(), // 2
        new TmIdleWaiting(),
        new TmbehindCoverFrontFight(), // 4
        new TmbehindCoverFrontIdle(),
        new TmInFormationFight(),    // 6
        new TmInFormationIdle(),
        new TmPatrollingAroundPoint(),    //8 
        new TmUseAbility(),
        new TmbehindCoverLateralActive(),  //10
        new TmbehindCoverLateralIdle(),
        new TmGoToForcedCover(), //12
        new TmAdvance(),
        new TmDefendPoint(),//14
        new TmMedic(),
        new TmGranedier()//16
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
    public bool holdFire;

    public string currStateText;

    public int changingToState;

    public float lastFire;
    public float fireRate = 0.2f;

    public GameObject explosive;

    public enum CoverType 
    {
        POSITIVE,
        NEGATIVE,
        FORWARD,
        NONE       
    }

    public CoverType currCoverType;
    public Vector3 currCoverTransformVector3;
    public Transform currCoverTransform;
    //public Transform currForcedCoverTransform;


    public List<GameObject> enemyList;


    public Slider healthSlider;
    public Text nameText;
    public Text abilityText;
    public Text StateText;

    public LayerMask ignoreCoverLayermask;

    public Vector3 PatrolPoint;

    public int abilityUsage = 3;
    public float lastAbilityUsed;
    public float abilityRate= 20f;

    [SerializeField] GameObject[] muzzleEffect = new GameObject[5];
    [SerializeField] GameObject hitEffect;
    [SerializeField] GameObject muzzlePoint;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();



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

            default:
                break;
        }
        allerted = false;
        alive = true;
    }

    private void Start()
    {
        currState = statesList[7];
        currState.EnterState(this);

        playerObj = GameObject.FindGameObjectWithTag("Player");

        nameText.text = "Name: " + memberName;
        abilityText.text = "Ability: " + selAbility;
    }

    private void Update()
    {
        if (alive && health <= 0) 
        {
            alive = false;
            ChangeState(0);
            AnimatorSetter(1);
        }

        healthSlider.value = health;
        StateText.text = "State: " +currStateText;

        currState.OnUpdate(this);   
    }

    public void ChangeState(int state)
    {
        changingToState = state;

        currState.OnExit(this);
        currState = statesList[state];


        currState.EnterState(this);
    }

    public void InstaStuff(RaycastHit outHit) 
    {
        var objs = Instantiate(muzzleEffect[Random.Range(0, 5)], muzzlePoint.transform.position, muzzlePoint.transform.rotation);

        objs.transform.parent = transform;

        Instantiate(hitEffect, outHit.point, Quaternion.identity);

        var obj = Instantiate(PlayerScript.instance.bulletPrefab, outHit.point, Quaternion.LookRotation(outHit.normal));
        obj.transform.parent = outHit.transform;
    }

    public void TakeDamage(int damage) 
    {
        health = health - damage;

        if (health <= 0) 
        {
            health = 0;    
            ChangeState(0);
            NavMeshAgent.isStopped = true;
        }
    }

    public void AddHealth(int medkitAmount) 
    { 
        health = health + medkitAmount;

        if (health >= 100) { health = 100; }
    }

    public void CallExplosive() 
    {
        Instantiate(explosive,new Vector3(target.transform.position.x, target.transform.position.y + 10f, target.transform.position.z), target.transform.rotation);
    }


    /// <summary>
    /// 0 shoot  --- 1 dead ---  2 idle alerted  ---   3   idle not alerted   --- 4 to cover    ---  5 on the move
    /// </summary>
    /// <param name="animationIndex"></param>
    public void AnimatorSetter(int animationIndex) 
    {
        switch (animationIndex)
        {
            case 0: //shoot
                animator.SetTrigger("Shoot");
                break;

            case 1:  //dead
                animator.SetTrigger("Die");
                break;

            case 2: //set idle alerted
                animator.SetBool("Alerted" , true);
                animator.SetBool("Moving" , false);
                animator.SetBool("Idle" , true);
                animator.SetBool("Cover" , false);
                break;

            case 3: //set idle not allerted
                animator.SetBool("Alerted", false);
                animator.SetBool("Moving", false);
                animator.SetBool("Idle", true);
                animator.SetBool("Cover", false);
                break;

            case 4:  //set to cover

                animator.SetBool("Alerted", false);
                animator.SetBool("Moving", false);
                animator.SetBool("Idle", false);
                animator.SetBool("Cover", true);

                break;

            case 5:  //on the move

                animator.SetBool("Alerted", false);
                animator.SetBool("Moving", true);
                animator.SetBool("Idle", false);
                animator.SetBool("Cover", false);
                break;

            case 6:
                break;

            default:
                break;
        }
    }

}
