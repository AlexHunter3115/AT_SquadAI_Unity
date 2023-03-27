
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

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

    private int selectedTeamMate = -1;
    public bool commandingTroops;
    public GameObject formationMainOBJ;

    public GameObject bulletPrefab;

    public bool controlPlayer;
    public bool playerInHouse;

    public float fireRate = 0.2f;
    public float lastFire = 0;
    public float inaccuracy = 0.1f;

    public int enemiesKilled = 0;

    public GameObject endPoint;
    [SerializeField] GameObject[] muzzleEffect = new GameObject[5];
    [SerializeField] GameObject hitEffect;
    [SerializeField] GameObject muzzlePoint;

    [SerializeField] Animator animator;
    [SerializeField] CharacterController charContr;

    private bool showMenu = false;
    public enum FormationType
    {
        SQUARE = 0,
        DELTA
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
        playerActions.Menu.performed += ctx => ToggleMenu();

        playerActions.ShowOptions.performed += ctx => CallOptionUIDraw();
        playerActions.ShowCoverPositions.performed += ctx => ToggleCoverPos();

        playerActions.MultiSelect.performed += ctx => MultiSelectTeamMate();

        playerActions.ForceGoToCover.performed += ctx => ForceCoverTeamMate();
        playerActions.ForceGoToCover.performed += ctx => OneTeamSelectTeamMate();

        playerActions.FindCover.performed += ctx => FindCoverTeamMates();
        playerActions.HoldFire.performed += ctx => HoldFireTeamMates();
        playerActions.UseAbility.performed += ctx => UseAbilityTeamMates();
        playerActions.IntoFormation.performed += ctx => IntoFormationTeamMates();
        playerActions.DefendePoint.performed += ctx => DefendPointTeamMates();
        playerActions.PatrolPoint.performed += ctx => PatrolPointTeamMates();
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

        UIManager.instance.AddNewMessageToQueue("Keep your troops alive and capture the flag.\nUse Mouse scroll, right mouse click to select your troops.\nTo select more troops use shift right mouse button.", Color.blue);
        UIManager.instance.AddNewMessageToQueue("You can also use Right Arrow to select all or Left Arrow to deselect all.\nOnce a troop is selected hold the middle mouse button for the different options.", Color.blue);
        UIManager.instance.AddNewMessageToQueue("Press T to opne the cover menu, when this menu is open you can force the selected teamMate to go to the cover of your choice.", Color.blue);
        UIManager.instance.AddNewMessageToQueue("Press X to toggle the top down view, use the [ and ] to hide floors so its easier to see inside buildings.", Color.blue);
    }


    private void ToggleMenu() 
    {
        showMenu = !showMenu;

        Time.timeScale = showMenu ? 0 : 1;
        Cursor.lockState = showMenu ? CursorLockMode.None : CursorLockMode.Locked;
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
        // add firerate 



        if (Input.GetKey(KeyCode.Mouse0) && !commandingTroops)
        {
            ShootingRayCastManager();
        }

    }


    public void ShootingRayCastManager()
    {
        var x = (1 - 2 * Random.value) * 0.005f;
        var y = (1 - 2 * Random.value) * 0.005f;

        Vector3 newDir = camFP.transform.TransformDirection(new Vector3(x, y, 1));

        if (Time.time > lastFire + fireRate)
        {
            lastFire = Time.time;
            RaycastHit outHit;
            if (Physics.Raycast(camFP.transform.position, newDir, out outHit, Mathf.Infinity, Hittable))
            {
                if (outHit.transform.CompareTag("TeamMate"))
                {
                    outHit.transform.root.GetComponent<TeamMateStateManager>().TakeDamage(5);
                }
                if (outHit.transform.CompareTag("Enemy"))
                {
                    outHit.transform.GetComponentInParent<EnemyScript>().TakeDamage(10);
                }

                var objs=  Instantiate(muzzleEffect[Random.Range(0, 5)], muzzlePoint.transform.position, muzzlePoint.transform.rotation);

                objs.transform.parent = transform;

                Instantiate(hitEffect, outHit.point, Quaternion.LookRotation(outHit.normal));

                var obj = Instantiate(bulletPrefab, outHit.point, Quaternion.identity);
                obj.transform.parent = outHit.transform;
            }
        }
    }

    // two things happening     one draws   the other one sends command

    public Transform RetCoverPosition(int x)
    {

        RaycastHit outHit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out outHit, Mathf.Infinity, ~basicCover) || Physics.Raycast(ray, out outHit, Mathf.Infinity, ~basicCover))
        {
            GameObject newRef = outHit.transform.gameObject;
            if (newRef.transform.GetComponent<SimpleObjectCover>())
            {
                newRef.transform.GetComponent<SimpleObjectCover>().showCubes = true;

                RaycastHit outhit;


                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out outhit, Mathf.Infinity) || Physics.Raycast(ray, out outhit))
                {
                    if (outhit.transform.tag == "BasicCover")
                    {
                        int idx = newRef.transform.GetComponent<SimpleObjectCover>().findIndexCoverCubes(outhit.transform.gameObject);


                        if (!newRef.transform.GetComponent<SimpleObjectCover>().listOfAvailability[idx])   // if the place is not taken
                        {
                            var teaMate = SquadManager.instance.teamMates[x].GetComponent<TeamMateStateManager>();

                            teaMate.currCoverTransformVector3 = outhit.transform.position;
                            teaMate.currCoverTransform = outhit.transform;
                            var name = outhit.transform.name;

                            var worldPos = teaMate.currCoverTransform.localPosition;
                            

                            if (name.Contains("Positive"))   // this two are the side ones   
                            {
                                teaMate.currCoverType = TeamMateStateManager.CoverType.POSITIVE;
                                var newWorldPos = new Vector3(worldPos.x, worldPos.y, worldPos.z - 0.35f);
                                newWorldPos = teaMate.currCoverTransform.TransformPoint(newWorldPos);

                                


                            }
                            else if (name.Contains("Minus"))
                            {
                                teaMate.currCoverType = TeamMateStateManager.CoverType.NEGATIVE;
                                var newWorldPos = new Vector3(worldPos.x, worldPos.y, worldPos.z + 0.35f);
                                newWorldPos = teaMate.currCoverTransform.TransformPoint(newWorldPos);



                            }
                            else
                            {

                                Vector3 adjustedPos = new Vector3(teaMate.currCoverTransform.position.x, teaMate.currCoverTransform.position.y + 1.1f, teaMate.currCoverTransform.position.z);

                                SquadManager.instance.teamMates[x].GetComponent<TeamMateStateManager>().currCoverType = TeamMateStateManager.CoverType.FORWARD;
                               

                            }


                            newRef.transform.GetComponent<SimpleObjectCover>().listOfAvailability[idx] = true;
                            return newRef.transform.GetComponent<SimpleObjectCover>().listOfPossiblePos[idx];
                        }


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
    private void ForceCoverTeamMate()
    {
        if (teamMatesNames.Count == 1 && commandingTroops)
        {
            for (int x = 0; x < SquadManager.instance.teamMates.Count; x++)
            {
                if (teamMatesNames[0] == SquadManager.instance.teamMates[x].GetComponent<TeamMateStateManager>().memberName)
                {
                    var pos = RetCoverPosition(x);

                    if (pos != null)
                    {
                        SquadManager.instance.teamMates[x].GetComponent<TeamMateStateManager>().currCoverTransform = pos;
                        SquadManager.instance.teamMates[x].GetComponent<TeamMateStateManager>().ChangeState(12);
                    }

                    break;
                }
            }
        }
    }

    // seems to be a var about the player being in the house not too sure what to do with that
    // the mouse thing doesnt really work need a fix on that
    public void SeeCoverPositions()
    {
        RaycastHit outHit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out outHit, Mathf.Infinity, ~basicCover) || Physics.Raycast(ray, out outHit, Mathf.Infinity, ~basicCover))
        {
            GameObject newRef = outHit.transform.gameObject;

            if (newRef.transform.GetComponent<SimpleObjectCover>())
                newRef.transform.GetComponent<SimpleObjectCover>().showCubes = true;
        }

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out outHit, Mathf.Infinity, ~houseCover) || playerInHouse || Physics.Raycast(ray, out outHit, Mathf.Infinity, ~houseCover))
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

        animator.SetFloat("Speed", input.magnitude);
    }


    public void ProcessLook(Vector2 input)
    {
        float mouseX = input.x;
        float mouseY = input.y;

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

        if (currFormation == FormationType.DELTA)
        {
            currFormation = FormationType.SQUARE;
        }
        else if (currFormation == FormationType.SQUARE) 
        {
            currFormation = FormationType.DELTA;
        }

        SquadManager.instance.ChangeSquadFormation(teamMatesNames);
    }


    private void MultiSelectTeamMate()
    {
        RaycastHit outhit;
        if (Physics.Raycast(camFP.transform.position, camFP.transform.forward, out outhit, Mathf.Infinity, Hittable))
        {

            if (outhit.transform.tag == "TeamMate")
            {
                var name = outhit.transform.root.GetComponent<TeamMateStateManager>().memberName;

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

        if (changeInput < 0)
        {
            changeInput = -1;
        }
        else
        {
            changeInput = 1;
        }


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
                if (SquadManager.instance.teamMates[i].GetComponent<TeamMateStateManager>().Health != 0) 
                {
                    teamMatesNames.Add(SquadManager.instance.teamMates[i].GetComponent<TeamMateStateManager>().memberName);
                    SquadManager.instance.uiList[i].GetComponent<TeamMateUISlot>().imageBack.color = Color.blue;
                }
                else 
                {
                    SquadManager.instance.uiList[i].GetComponent<TeamMateUISlot>().imageBack.color = Color.red;
                }
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

            if (SquadManager.instance.teamMates[i].GetComponent<TeamMateStateManager>().Health != 0) 
            {
                teamMatesNames.Add(SquadManager.instance.teamMates[i].GetComponent<TeamMateStateManager>().memberName);
                SquadManager.instance.uiList[i].GetComponent<TeamMateUISlot>().imageBack.color = Color.blue;
            }
         
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
    private void OneTeamSelectTeamMate()
    {
        RaycastHit outhit;
        if (Physics.Raycast(camFP.transform.position, camFP.transform.forward, out outhit, Mathf.Infinity, Hittable))
        {
            if (outhit.transform.tag == "TeamMate")
            {

                teamMatesNames.Clear();
                if (outhit.transform.root.GetComponent<TeamMateStateManager>().Health != 0)
                {
                    var name = outhit.transform.root.GetComponent<TeamMateStateManager>().memberName;

                    for (int i = 0; i < SquadManager.instance.uiList.Count; i++)
                    {

                        if (SquadManager.instance.uiList[i].GetComponent<TeamMateUISlot>().nameText.text.Contains(name))
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
            }
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
    private void CallOptionUIDraw() => UIManager.instance.ToggleOptions();


    private void OnEnable()
    {
        playerActions.Enable();
    }
    private void OnDisable()
    {
        playerActions.Disable();
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
        if (UIManager.instance.showOptions)
        {
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
        RaycastHit outhit;



        if (UIManager.instance.showOptions)
        {
            for (int i = 0; i < teamMatesNames.Count; i++)
            {
                for (int x = 0; x < SquadManager.instance.teamMates.Count; x++)
                {
                    if (teamMatesNames[i] == SquadManager.instance.teamMates[x].GetComponent<TeamMateStateManager>().memberName)
                    {
                        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out outhit, Mathf.Infinity, Hittable))
                        {
                            SquadManager.instance.teamMates[x].GetComponent<TeamMateStateManager>().PatrolPoint = outhit.point;
                            SquadManager.instance.teamMates[x].GetComponent<TeamMateStateManager>().ChangeState(14);
                        }
                    }
                }
            }
        }

    }
    private void PatrolPointTeamMates()
    {

        RaycastHit outhit;



        if (UIManager.instance.showOptions)
        {
            for (int i = 0; i < teamMatesNames.Count; i++)
            {
                for (int x = 0; x < SquadManager.instance.teamMates.Count; x++)
                {
                    if (teamMatesNames[i] == SquadManager.instance.teamMates[x].GetComponent<TeamMateStateManager>().memberName)
                    {
                        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out outhit, Mathf.Infinity, Hittable))
                        {
                            SquadManager.instance.teamMates[x].GetComponent<TeamMateStateManager>().PatrolPoint = outhit.point;
                            SquadManager.instance.teamMates[x].GetComponent<TeamMateStateManager>().ChangeState(8);
                        }
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
                        SquadManager.instance.teamMates[x].GetComponent<TeamMateStateManager>().ChangeState(13);
                     
                    }
                }
            }
        }
    }


    private void OnGUI()
    {
        GUIStyle style = GUI.skin.GetStyle("label"); 
        style.alignment = TextAnchor.UpperRight; 
       
        if (showMenu) 
        {
            Rect buttonRect = new Rect(Screen.width / 2 - 50, Screen.height / 2 - 25, 100, 50); 
            if (GUI.Button(buttonRect, "Restart Game"))
            { // Draw the button with the specified text
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);

                Time.timeScale =1;
            }
        }
        else 
        {
            Rect labelRect = new Rect(Screen.width - 125, 0, 100, 50); 
            GUI.Label(labelRect, $"Enemies killed {enemiesKilled}", style); 
        }
    }

}
