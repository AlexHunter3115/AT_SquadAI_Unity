using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

public class PlayerScript : MonoBehaviour
{
    public static PlayerScript instance;

    private PlayerInput playerInput;
    private PlayerInput.PlayerActions playerActions;

    private CharacterController controller;
    private Vector3 playerVelocity;
    public float speed = 5f;

    public float gravity = -9.8f;
    public bool isGrouded;

    public Camera camFP;
    public Camera camTV;
    private float xRot = 0f;

    public float xSens = 30f;
    public float ySens = 30f;

    public List<Transform> SquadFormations;

    public LayerMask basicCover;
    public LayerMask houseCover;
    public LayerMask houseMask;
    public LayerMask Hittable;

    public int selectedTeamMate;
    public bool commandingTroops;
    public GameObject formationMainOBJ;

    public GameObject bulletPrefab;

    public bool controlPlayer;
    public bool playerInHouse;


    public float fireRate = 0.2f;
    public float lastFire = 0;
    public float inaccuracy = 0.1f;

    public enum FormationType
    {
        SQUARE = 0,
        DELTA,
        WORM
    }

    public FormationType currFormation;



    private void Awake()
    {
        instance = this;

        playerInput = new PlayerInput();
        playerActions = playerInput.Player;
        playerActions.FormationChange.performed += ctx => FormationFunction();
        playerActions.SelectTeamMate.performed += ctx => TeamSelectFunction();
        playerActions.DeselectTeam.performed += ctx => test();
        playerActions.TopViewToggle.performed += ctx => ToggleTopView();
        playerActions.moveFloorsView.performed += ctx => MoveFloors();

        controller = GetComponent<CharacterController>();
        currFormation = FormationType.SQUARE;
    }


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        camFP.enabled = true;
        camTV.enabled = false;
        controlPlayer = true;
        playerInHouse = false;
    }




    private void Update()
    {
        isGrouded = controller.isGrounded;
        if (controlPlayer)
        ProcessLook(playerActions.Look.ReadValue<Vector2>());

        if (commandingTroops)
            CoverRayCastManager();

        // add firerate 
        if (Input.GetKey(KeyCode.Mouse0) && !commandingTroops)
        {

            ShootingRayCastManager();
        }

    }





    public void ShootingRayCastManager() 
    {


        var x = (1 - 2 * Random.value) * inaccuracy;
        var y = (1 - 2 * Random.value) * inaccuracy;


        Vector3 newDir = camFP.transform.TransformDirection(new Vector3(x, y, 1));

        if (Time.time > lastFire + fireRate)
        {

            lastFire = Time.time;
            RaycastHit outHit;
            if (Physics.Raycast(camFP.transform.position, newDir, out outHit, Mathf.Infinity, Hittable))
            {
                GameObject newRef = Instantiate(bulletPrefab);
                newRef.transform.position = outHit.point;
            }


        }




    }


    //this function is actually makes me want to throw up
    public void CoverRayCastManager() 
    {

        if (controlPlayer)
        {


            RaycastHit hit;
            // this has something to do with the cover positions things
            // need to prob hcnage the FP to TV when ready
            if (Physics.Raycast(camFP.transform.position, camFP.transform.forward, out hit, Mathf.Infinity, ~basicCover))
            {
                GameObject newRef = hit.transform.gameObject;

                if (newRef.transform.GetComponent<SimpleObjectCover>())
                {
                    newRef.transform.GetComponent<SimpleObjectCover>().showCubes = true;

                    RaycastHit outhit;

                    if (Input.GetKeyDown(KeyCode.Mouse1))
                    {
                        if (Physics.Raycast(camFP.transform.position, camFP.transform.forward, out outhit, Mathf.Infinity))
                        {
                            if (outhit.transform.tag == "BasicCover")
                            {

                                int idx = newRef.transform.GetComponent<SimpleObjectCover>().findIndexCover(outhit.transform.gameObject);
                                //Debug.Log($"specific hit {idx}");


                                SquadManager.instance.teamMates[selectedTeamMate].GetComponent<TestNevMash>().movepositionTransform = newRef.transform.GetComponent<SimpleObjectCover>().listOfPossiblePos[idx];
                            }

                        }
                    }
                    //Debug.Log($"got the cover");
                }

            }

            if (Physics.Raycast(camFP.transform.position, camFP.transform.forward, out hit, Mathf.Infinity, ~houseCover) || playerInHouse)
            {

                GameObject newRef = hit.transform.gameObject;
                if (newRef.transform.GetComponent<HouseFloorLogic>())
                {
                    //newRef.transform.GetComponent<HouseFloorLogic>().showCubes = true;
                    Debug.Log($"on the hosue things");
                    RaycastHit outhit;

                    if (Input.GetKeyDown(KeyCode.Mouse1))
                    {
                        if (Physics.Raycast(camFP.transform.position, camFP.transform.forward, out outhit, Mathf.Infinity, ~houseMask))
                        {
                            if (outhit.transform.tag == "HouseCover")
                            {
                                int idx = newRef.transform.GetComponent<HouseFloorLogic>().findIndexCover(outhit.transform.gameObject);
                                Debug.Log($"specific hit {idx}");


                                SquadManager.instance.teamMates[selectedTeamMate].GetComponent<TestNevMash>().movepositionTransform = newRef.transform.GetComponent<HouseFloorLogic>().coverPos[idx];
                                Debug.Log($"{outhit.transform.name}");
                            }
                        }
                    }
                    //Debug.Log($"got the cover");
                }
            }


            RaycastHit outhitd;

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                if (playerInHouse && Physics.Raycast(camFP.transform.position, camFP.transform.forward, out outhitd, Mathf.Infinity, ~houseMask))
                {
                    if (outhitd.transform.tag == "HouseCover")
                    {
                        int idx = outhitd.transform.GetComponentInParent<HouseFloorLogic>().findIndexCover(outhitd.transform.gameObject);



                        SquadManager.instance.teamMates[selectedTeamMate].GetComponent<TestNevMash>().movepositionTransform = outhitd.transform.GetComponentInParent<HouseFloorLogic>().coverPos[idx];

                    }

                }
            }



        }
        else 
        {
            RaycastHit outhitd;


            if (Physics.Raycast(camTV.transform.position, camTV.transform.forward, out outhitd, Mathf.Infinity, ~basicCover))
            {
                GameObject newRef = outhitd.transform.gameObject;
                Debug.Log($"cvcbvbcvcbv");
                if (newRef.transform.GetComponent<SimpleObjectCover>())
                {
                    newRef.transform.GetComponent<SimpleObjectCover>().showCubes = true;

                    RaycastHit outhit;
                    Debug.Log($"ewrwerwerw");
                    if (Input.GetKeyDown(KeyCode.Mouse1))
                    {
                        if (Physics.Raycast(camTV.transform.position, camTV.transform.forward, out outhit, Mathf.Infinity))
                        {
                            if (outhit.transform.tag == "BasicCover")
                            {
                                Debug.Log($"cvbvcbcvb");
                                int idx = newRef.transform.GetComponent<SimpleObjectCover>().findIndexCover(outhit.transform.gameObject);

                                SquadManager.instance.teamMates[selectedTeamMate].GetComponent<TestNevMash>().movepositionTransform = newRef.transform.GetComponent<SimpleObjectCover>().listOfPossiblePos[idx];
                            }
                        }
                    }
                }
            }

            if (Physics.Raycast(camTV.transform.position, camTV.transform.forward, out outhitd, Mathf.Infinity, ~houseCover) || playerInHouse)
            {

                GameObject newRef = outhitd.transform.gameObject;
                if (newRef.transform.GetComponent<HouseFloorLogic>())
                {
                    //newRef.transform.GetComponent<HouseFloorLogic>().showCubes = true;
                    Debug.Log($"on the hosue things");
                    RaycastHit outhit;

                    if (Input.GetKeyDown(KeyCode.Mouse1))
                    {
                        if (Physics.Raycast(camTV.transform.position, camTV.transform.forward, out outhit, Mathf.Infinity, ~houseMask))
                        {
                            if (outhit.transform.tag == "HouseCover")
                            {
                                int idx = newRef.transform.GetComponent<HouseFloorLogic>().findIndexCover(outhit.transform.gameObject);
                                Debug.Log($"specific hit {idx}");


                                SquadManager.instance.teamMates[selectedTeamMate].GetComponent<TestNevMash>().movepositionTransform = newRef.transform.GetComponent<HouseFloorLogic>().coverPos[idx];
                                Debug.Log($"{outhit.transform.name}");
                            }


                        }
                    }
                    //Debug.Log($"got the cover");

                }
            }

        }
    }




    public void ProcessMove(Vector2 input) 
    {
        Vector3 moveDir = Vector3.zero;
        moveDir.x = input.x;
        moveDir.z = input.y;


        if (input.x != 0 || input.y != 0) 
        {
            formationMainOBJ.transform.position = this.transform.position;
            
            formationMainOBJ.transform.rotation = this.transform.rotation;
        }

        controller.Move(transform.TransformDirection(moveDir) * speed * Time.deltaTime);

        if (isGrouded && playerVelocity.y < 0)
            playerVelocity.y = -2f;
        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);



    }

    public void ProcessLook(Vector2 input) 
    {
        float mouseX= input.x;
        float mouseY= input.y;

        xRot -= (mouseY * Time.deltaTime) * ySens;
        xRot = Mathf.Clamp(xRot, -80f, 80f);

        camFP.transform.localRotation = Quaternion.Euler(xRot, 0, 0);
        transform.Rotate(Vector3.up * (mouseX * Time.deltaTime) * xSens);
    
    }

    private void FixedUpdate()
    {
        if (controlPlayer)
        ProcessMove(playerActions.Movement.ReadValue<Vector2>());
       
    }


    public void ToggleTopView()
    {
        camTV.enabled = !camTV.enabled;
        camFP.enabled = !camFP.enabled;
        controlPlayer = !controlPlayer;
        TopViewCamLogic.instance.changeFloorView(3);
    }

    public void FormationFunction()
    {
        currFormation++;

        if (currFormation == FormationType.WORM)
        {
            currFormation = FormationType.SQUARE;
        }

        SquadManager.instance.ChangeSquadFormation();
    }

    private void TeamSelectFunction()
    {
        commandingTroops = true;
        selectedTeamMate++;

        if (SquadManager.instance.squadSize <= selectedTeamMate) 
        {
            selectedTeamMate = 0;
        }
    }


    public void MoveFloors() 
    {
        float changeInput = playerActions.moveFloorsView.ReadValue<float>();

        TopViewCamLogic.instance.changeFloorView((int)changeInput);

        //Debug.Log(changeInput);
    }


    private void test()
    {
        commandingTroops = false;
        selectedTeamMate = 0;
    }


    private void OnEnable()
    {
        playerActions.Enable();
    }
    private void OnDisable()
    {
        playerActions.Disable();
    }

}
