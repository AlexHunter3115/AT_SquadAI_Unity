using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadManager : MonoBehaviour
{

    public static SquadManager instance;

    public GameObject[] teamMates;
    public GameObject teamMatePrefab;

    private PlayerScript playerScript;
    public GameObject SquadFormations;

    public int squadSize = 4;


    public GameObject UIHolder;
    public GameObject prefabTMUi;

    public List<GameObject> uiList;


    private void Awake()
    {
        instance = this;
    }



    // Start is called before the first frame update
    void Start()
    {
        teamMates = new GameObject[squadSize];

        playerScript = PlayerScript.instance;

        for (int i = 0; i < squadSize; i++)
        {
            GameObject newRef = teamMates[i] = Instantiate(teamMatePrefab);  //adds to the enemy and instatiat
            var name = newRef.GetComponent<TeamMateStateManager>().memberName;
            var ability = newRef.GetComponent<TeamMateStateManager>().SelAbility;

            newRef.transform.position = playerScript.SquadFormations[(int)playerScript.currFormation].transform.GetChild(i).position;

            newRef = Instantiate(prefabTMUi, UIHolder.transform);
            newRef.GetComponent<TeamMateUISlot>().nameText.text = "Name: " + name;
            newRef.GetComponent<TeamMateUISlot>().abilityText.text = "Ability index: " + ability;
            uiList.Add(newRef);
        }
    }

    


    public void ChangeSquadFormation() 
    {
        for (int i = 0; i < squadSize; i++)
        {
            //meObject newRef = teamMates[i];  //adds to the enemy and instatiat

            //int formInd = (int)playerScript.currFormation;
            TeamMateStateManager TMstateMan = teamMates[i].GetComponent<TeamMateStateManager>();

            if (TMstateMan.Allerted) 
            {
                TMstateMan.ChangeState(6);
            }
            else 
            {
                TMstateMan.ChangeState(7);
            }

            TMstateMan.FormationTran = SquadFormations.transform.GetChild((int)playerScript.currFormation).transform.GetChild(i).transform;

            //teamMates[i].GetComponent<TeamMateStateManager>().ChangeState()
            //teamMates[i].GetComponent<TestNevMash>().movepositionTransform = SquadFormations.transform.GetChild((int)playerScript.currFormation).transform.GetChild(i).transform;   // sets the nevmesh
        }
    }
}
