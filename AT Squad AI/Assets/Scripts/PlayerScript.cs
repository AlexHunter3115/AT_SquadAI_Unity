using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

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

    public Camera cam;
    private float xRot = 0f;

    public float xSens = 30f;
    public float ySens = 30f;

    public List<Transform> SquadFormations;

    public LayerMask basicCover;

    public int selectedTeamMate;
    public bool commandingTroops;

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

        controller = GetComponent<CharacterController>();

        currFormation = FormationType.SQUARE;
    }


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }




    private void Update()
    {
        isGrouded = controller.isGrounded;
        ProcessLook(playerActions.Look.ReadValue<Vector2>());


        RayCastManager();

    }



    public void RayCastManager() 
    {
        RaycastHit hit;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity,~basicCover) && commandingTroops)
        {
            GameObject newRef = hit.transform.gameObject;

            if (newRef.transform.GetComponent<SimpleObjectCover>()) 
            {
                newRef.transform.GetComponent<SimpleObjectCover>().showCubes = true;

                RaycastHit outhit;

                if (Input.GetKey(KeyCode.Mouse1)) 
                {
                    if (Physics.Raycast(cam.transform.position, cam.transform.forward, out outhit, Mathf.Infinity))
                    {
                        if (outhit.transform.tag == "BasicCover")
                        {

                            int idx = newRef.transform.GetComponent<SimpleObjectCover>().findIndexCover(outhit.transform.gameObject);
                            Debug.Log($"specific hit {idx}");


                            SquadManager.instance.teamMates[selectedTeamMate].GetComponent<TestNevMash>().movepositionTransform = newRef.transform.GetComponent<SimpleObjectCover>().listOfPossiblePos[idx];
                        }
                    }
                }
                Debug.Log($"got the shist");
            }
            else 
            {
                Debug.Log($"this didnt hit where it meant");
            
            }
        }


    }




    public void ProcessMove(Vector2 input) 
    {
        Vector3 moveDir = Vector3.zero;
        moveDir.x = input.x;
        moveDir.z = input.y;

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

        cam.transform.localRotation = Quaternion.Euler(xRot, 0, 0);
        transform.Rotate(Vector3.up * (mouseX * Time.deltaTime) * xSens);
    
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
    


    private void FixedUpdate()
    {
        ProcessMove(playerActions.Movement.ReadValue<Vector2>());
       
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
