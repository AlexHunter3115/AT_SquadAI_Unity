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

    public List<string> teamMatesNames = new List<string>();


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
        playerActions.TopViewToggle.performed += ctx => ToggleTopView();
        playerActions.moveFloorsView.performed += ctx => MoveFloors();
        playerActions.SelectAllTeamMates.performed += ctx => AllTeamSelect();
        playerActions.DeselectAllTeamMates.performed += ctx => AllTeamDeSelect();

        playerActions.ShowOptions.performed += ctx => CallOptionUIDraw();
        playerActions.ShowCoverPositions.performed += ctx => ToggleCoverPos();

        playerActions.MultiSelect.performed += ctx => MultiSelectTeamMate();

        playerActions.ForceGoToCover.performed += ctx => ForceCoverTeamMate();

        playerActions.FindCover.performed += ctx => FindCoverTeamMates();
        playerActions.HoldFire.performed += ctx => HoldFireTeamMates();
        playerActions.UseAbility.performed += ctx => UseAbilityTeamMates();
        playerActions.IntoFormation.performed += ctx => IntoFormationTeamMates();
        playerActions.DefendePoint.performed += ctx => DefendPointTeamMates();
        playerActions.PatrolPoint.performed += ctx => PatrolAroundPoint();
        playerActions.AdvanceToPoint.performed += ctx => AdvanceToPointTeamMates();


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


    private void ToggleCoverPos() => commandingTroops = !commandingTroops;

    private void Update()
    {
        isGrouded = controller.isGrounded;
        if (controlPlayer)
        ProcessLook(playerActions.Look.ReadValue<Vector2>());

        if (commandingTroops)
            SeeCoverPositions();

        //move out of here
        PatrolAroundPoint();
        // add firerate 

        if (Input.GetKeyDown(KeyCode.Mouse1) && Input.GetKey(KeyCode.LeftShift)) 
        {
            //MultiSelectTeamMate();
        }

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




    // two things happening     one draws   the other one sends command



    public Transform RetCoverPosition()
    {

        RaycastHit outHit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out outHit, Mathf.Infinity, ~basicCover) || Physics.Raycast(ray, out outHit, Mathf.Infinity, ~basicCover))
        {
            GameObject newRef = outHit.transform.gameObject;
            Debug.Log($"cvcbvbcvcbv");
            if (newRef.transform.GetComponent<SimpleObjectCover>())
            {
                newRef.transform.GetComponent<SimpleObjectCover>().showCubes = true;

                RaycastHit outhit;

                Debug.Log($"ewrwerwerw");
               
                    if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out outhit, Mathf.Infinity) || Physics.Raycast(ray, out outhit))
                    {
                        if (outhit.transform.tag == "BasicCover")
                        {
                            Debug.Log($"cvbvcbcvb");
                            int idx = newRef.transform.GetComponent<SimpleObjectCover>().findIndexCoverCubes(outhit.transform.gameObject);

                            return newRef.transform.GetComponent<SimpleObjectCover>().listOfPossiblePos[idx];
                        }
                    }
                
            }
        }

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out outHit, Mathf.Infinity, ~houseCover) || playerInHouse || Physics.Raycast(ray, out outHit, Mathf.Infinity, ~houseCover))
        {

            GameObject newRef = outHit.transform.gameObject;
            if (newRef.transform.GetComponent<HouseFloorLogic>())
            {
                newRef.transform.GetComponent<HouseFloorLogic>().showCubes = true;
                Debug.Log($"on the hosue things");
                RaycastHit outhit;
                
                    if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out outhit, Mathf.Infinity, ~houseMask) || Physics.Raycast(ray, out outhit))
                    {
                        if (outhit.transform.tag == "HouseCover")
                        {
                            int idx = newRef.transform.GetComponent<HouseFloorLogic>().findIndexCover(outhit.transform.gameObject);
                            return newRef.transform.GetComponent<HouseFloorLogic>().coverPos[idx];
                        }
                    }
                
            }
        }



        return null;



    }

    // seems to be a var about the player being in the house not too sure what to do with that
    // the mouse thing doesnt really work need a fix on that
    public void SeeCoverPositions() 
    {
        RaycastHit outHit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out outHit, Mathf.Infinity, ~basicCover) || Physics.Raycast(ray, out outHit,Mathf.Infinity, ~basicCover))
        {
            GameObject newRef = outHit.transform.gameObject;

            if (newRef.transform.GetComponent<SimpleObjectCover>())
                newRef.transform.GetComponent<SimpleObjectCover>().showCubes = true;
        }

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out outHit, Mathf.Infinity, ~houseCover) || playerInHouse  || Physics.Raycast(ray, out outHit, Mathf.Infinity, ~houseCover))
        {
            GameObject newRef = outHit.transform.gameObject;

            if (newRef.transform.GetComponent<HouseFloorLogic>())
                newRef.transform.GetComponent<HouseFloorLogic>().showCubes = true;
        }
    }





    // i want this to be in a soecific spot, wait a bit thengo
    

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

        if (controlPlayer) 
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else 
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void FormationFunction()
    {
        currFormation++;

        if (currFormation == FormationType.WORM)
        {
            currFormation = FormationType.SQUARE;
        }

        SquadManager.instance.ChangeSquadFormation(teamMatesNames);
    }



    private void MultiSelectTeamMate() 
    {

        RaycastHit outhit;

        if (Physics.Raycast(camFP.transform.position, camFP.transform.forward, out outhit, Mathf.Infinity))
        {
            if (outhit.transform.tag == "TeamMate")
            {
                var name = outhit.transform.GetComponent<TeamMateStateManager>().memberName;
               
                if (teamMatesNames.Contains(name))
                {
                    for (int i = 0; i < SquadManager.instance.uiList.Count; i++)
                    {
                        if (SquadManager.instance.uiList[i].GetComponent<TeamMateUISlot>().nameText.text == "Name: " + name)
                        {

                            SquadManager.instance.uiList[i].GetComponent<TeamMateUISlot>().imageBack.color = new Color(0.6f, 0.6f, 0.6f, 1);
                        }
                    }
                    teamMatesNames.Remove(name);

                }
                else
                {
                    teamMatesNames.Add(name);
                    for (int i = 0; i < SquadManager.instance.uiList.Count; i++)
                    {
                        Debug.Log($"{name}");
                        Debug.Log($"{SquadManager.instance.uiList[i].GetComponent<TeamMateUISlot>().nameText.text}");
                        if (SquadManager.instance.uiList[i].GetComponent<TeamMateUISlot>().nameText.text == "Name: " + name)
                        {
                            SquadManager.instance.uiList[i].GetComponent<TeamMateUISlot>().imageBack.color = Color.blue;
                        }
                    }

                }
            }
        }
    }
    /// <summary>
    /// this si for the button thing so, select with the arrow keys
    /// </summary>
    private void TeamSelectFunction()
    {
        teamMatesNames.Clear();

        float changeInput = playerActions.SelectTeamMate.ReadValue<float>();
        selectedTeamMate += (int)changeInput;

        if (SquadManager.instance.squadSize <= selectedTeamMate)
        {
            selectedTeamMate = 0;
        }
        else if (selectedTeamMate < 0)
        {
            selectedTeamMate = SquadManager.instance.squadSize - 1;
        }

        for (int i = 0; i < SquadManager.instance.uiList.Count; i++)
        {
            if (selectedTeamMate == i)
            {

                SquadManager.instance.uiList[i].GetComponent<TeamMateUISlot>().imageBack.color = Color.blue;
                teamMatesNames.Add(SquadManager.instance.teamMates[i].GetComponent<TeamMateStateManager>().memberName);
            }
            else
            {
                SquadManager.instance.uiList[i].GetComponent<TeamMateUISlot>().imageBack.color = new Color(0.6f, 0.6f, 0.6f, 1);
            }
        }
    }
    /// <summary>
    /// select the whole team with the side arrow keys
    /// </summary>
    public void AllTeamSelect() 
    {
        teamMatesNames.Clear();

        for (int i = 0; i < SquadManager.instance.uiList.Count; i++)
        {
            teamMatesNames.Add(SquadManager.instance.teamMates[i].GetComponent<TeamMateStateManager>().memberName);
            SquadManager.instance.uiList[i].GetComponent<TeamMateUISlot>().imageBack.color = Color.blue;
        }
    }
    public void AllTeamDeSelect()
    {
        teamMatesNames.Clear();

        for (int i = 0; i < SquadManager.instance.uiList.Count; i++)
        {
            //teamMatesNames.Add(SquadManager.instance.teamMates[i].GetComponent<TeamMateStateManager>().memberName);
            SquadManager.instance.uiList[i].GetComponent<TeamMateUISlot>().imageBack.color = new Color(0.6f, 0.6f, 0.6f, 1);
        }
    }






    public void MoveFloors() 
    {
        if (!controlPlayer) 
        {
            float changeInput = playerActions.moveFloorsView.ReadValue<float>();

            TopViewCamLogic.instance.changeFloorView((int)changeInput);
        }
    }


    public void PatrolAroundPoint()
    {

        //to change
        RaycastHit outhit;
        if (Input.GetKeyDown(KeyCode.M))
        {


            //Debug.Log($"dwuaihuiwdauhidwahuidwaiuh");


            if (controlPlayer)
            {
                if (Physics.Raycast(camFP.transform.position, camFP.transform.forward, out outhit, Mathf.Infinity, 13))
                {
                    Debug.Log($"{Mathf.Infinity}");
                    SquadManager.instance.teamMates[selectedTeamMate].GetComponent<TeamMateStateManager>().currCoverTransformVector3 = outhit.point;

                    SquadManager.instance.teamMates[selectedTeamMate].GetComponent<TeamMateStateManager>().ChangeState(8);
                }
            }
            else
            {
                if (Physics.Raycast(camTV.transform.position, camTV.transform.forward, out outhit, Mathf.Infinity, 13))
                {
                    //SquadManager.instance.teamMates[selectedTeamMate].GetComponent<TeamMateStateManager>().currCoverTransform = newRef.transform.GetComponent<SimpleObjectCover>().listOfPossiblePos[idx];

                    SquadManager.instance.teamMates[selectedTeamMate].GetComponent<TeamMateStateManager>().currCoverTransformVector3 = outhit.point;
                    //SquadManager.instance.teamMates[selectedTeamMate].GetComponent<TeamMateStateManager>().currCoverTransform = outhit;


                    SquadManager.instance.teamMates[selectedTeamMate].GetComponent<TeamMateStateManager>().ChangeState(8);
                }
            }



        }
    }


    private void CallOptionUIDraw() => UIManager.instance.ToggleOptions();
   





    private void test()
    {
        //commandingTroops = false;
        //selectedTeamMate = 0;
        Debug.Log($"called for the set cover");
        SquadManager.instance.teamMates[0].GetComponent<TeamMateStateManager>().ChangeState(1);
    }


    private void OnEnable()
    {
        playerActions.Enable();
    }
    private void OnDisable()
    {
        playerActions.Disable();
    }




    private void ForceCoverTeamMate() 
    {
        if (teamMatesNames.Count == 1 && commandingTroops) 
        {
            for (int x = 0; x < SquadManager.instance.teamMates.Count; x++)
            {
                if (teamMatesNames[0] == SquadManager.instance.teamMates[x].GetComponent<TeamMateStateManager>().memberName)
                {
                    var pos = RetCoverPosition();

                    if(pos != null) 
                    {
                        SquadManager.instance.teamMates[x].GetComponent<TeamMateStateManager>().currForcedCoverTransform = pos;
                        SquadManager.instance.teamMates[x].GetComponent<TeamMateStateManager>().ChangeState(12);
                    }

                    break;
                }
            }
        }

       
    }



    private void FindCoverTeamMates()
    {
        if (UIManager.instance.showOptions)
        {
            for (int i = 0; i < teamMatesNames.Count; i++)
            {
                for (int x = 0; x < SquadManager.instance.teamMates.Count; x++)
                {
                    if (teamMatesNames[i] == SquadManager.instance.teamMates[x].GetComponent<TeamMateStateManager>().memberName)
                    {
                        SquadManager.instance.teamMates[x].GetComponent<TeamMateStateManager>().ChangeState(1);
                        continue;
                    }
                }
            }
        }
    }
    private void HoldFireTeamMates() 
    {
        if (UIManager.instance.showOptions) {
            for (int i = 0; i < teamMatesNames.Count; i++)
            {
                for (int x = 0; x < SquadManager.instance.teamMates.Count; x++)
                {
                    if (teamMatesNames[i] == SquadManager.instance.teamMates[x].GetComponent<TeamMateStateManager>().memberName)
                    {
                        SquadManager.instance.teamMates[x].GetComponent<TeamMateStateManager>().holdFire = !SquadManager.instance.teamMates[x].GetComponent<TeamMateStateManager>().holdFire;
                        continue;
                    }
                }
            }
        }
    }
    private void UseAbilityTeamMates() 
    {
        if (UIManager.instance.showOptions)
        {
            for (int i = 0; i < teamMatesNames.Count; i++)
            {
                for (int x = 0; x < SquadManager.instance.teamMates.Count; x++)
                {
                    if (teamMatesNames[i] == SquadManager.instance.teamMates[x].GetComponent<TeamMateStateManager>().memberName)
                    {
                        SquadManager.instance.teamMates[x].GetComponent<TeamMateStateManager>().ChangeState(9);
                        continue;
                    }
                }
            }
        }
    }
    private void IntoFormationTeamMates() 
    {
        if (UIManager.instance.showOptions) 
        {
            SquadManager.instance.ChangeSquadFormation(teamMatesNames);
        }
    }
    private void DefendPointTeamMates()
    {
        if (UIManager.instance.showOptions)
        {
            for (int i = 0; i < teamMatesNames.Count; i++)
        {
            for (int x = 0; x < SquadManager.instance.teamMates.Count; x++)
            {
                if (teamMatesNames[i] == SquadManager.instance.teamMates[x].GetComponent<TeamMateStateManager>().memberName)
                {
                    //SquadManager.instance.teamMates[x].GetComponent<TeamMateStateManager>().ChangeState(6);
                    continue;
                }
            }
        }
    }

    }
    private void PatrolPointTeamMates()
    {

        if (UIManager.instance.showOptions)
        {
            for (int i = 0; i < teamMatesNames.Count; i++)
        {
            for (int x = 0; x < SquadManager.instance.teamMates.Count; x++)
            {
                if (teamMatesNames[i] == SquadManager.instance.teamMates[x].GetComponent<TeamMateStateManager>().memberName)
                {
                    SquadManager.instance.teamMates[x].GetComponent<TeamMateStateManager>().ChangeState(8);
                    continue;
                }
            }
        }
    }
    }
    private void AdvanceToPointTeamMates()
    {

        if (UIManager.instance.showOptions)
        {
            for (int i = 0; i < teamMatesNames.Count; i++)
        {
            for (int x = 0; x < SquadManager.instance.teamMates.Count; x++)
            {
                if (teamMatesNames[i] == SquadManager.instance.teamMates[x].GetComponent<TeamMateStateManager>().memberName)
                {
                    //SquadManager.instance.teamMates[x].GetComponent<TeamMateStateManager>().ChangeState(6);
                    continue;
                }
            }
        } }
    }












}
