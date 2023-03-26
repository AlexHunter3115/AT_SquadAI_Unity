using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadManager : MonoBehaviour
{

    public static SquadManager instance;

    public GameObject flagpost;

    public List<GameObject> teamMates = new List<GameObject>();
    public GameObject teamMatePrefab;

    private PlayerScript playerScript;
    public GameObject SquadFormations;

    public int squadSize = 4;


    public GameObject UIHolder;
    public GameObject prefabTMUi;

    public List<GameObject> uiList;

    public LayerMask ignoreCoverLayermask;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        playerScript = PlayerScript.instance;

        for (int i = 0; i < squadSize; i++)
        {
            GameObject newRef = Instantiate(teamMatePrefab);  //adds to the enemy and instatiat
            teamMates.Add(newRef);

            //check for similar name
            while (true) 
            {
                bool nameCheck = false;

                string placeholderName = Random.Range(0, 9999).ToString();

                    for (int x = 0; x < i; x++)
                    {
                        if (placeholderName == teamMates[x].GetComponent<TeamMateStateManager>().memberName)
                        {
                            nameCheck = true;
                            Debug.Log($"Found an equal name");
                        }
                    }
                
                if (nameCheck)  {}
                else 
                {
                    teamMates[i].GetComponent<TeamMateStateManager>().memberName = placeholderName;
                    break;
                }
                
            }
            

            var name = newRef.GetComponent<TeamMateStateManager>().memberName;
            var ability = newRef.GetComponent<TeamMateStateManager>().SelAbility;
            newRef.GetComponent<TeamMateStateManager>().FormationTran = playerScript.SquadFormations[0].transform.GetChild(i);
            newRef.transform.position = playerScript.SquadFormations[0].transform.GetChild(i).position;

            newRef = Instantiate(prefabTMUi, UIHolder.transform);
            newRef.GetComponent<TeamMateUISlot>().nameText.text = "Name: " + name;
            newRef.GetComponent<TeamMateUISlot>().abilityText.text = "Ability: " + ability;
            uiList.Add(newRef);
        }

    }

    private void Update()
    {
        for (int i = 0; i < uiList.Count; i++)
        {
            uiList[i].GetComponent<TeamMateUISlot>().stateText.text = "State: " + teamMates[i].GetComponent<TeamMateStateManager>().currStateText;
        }
    }

    public void ChangeSquadFormation(List<string> names) 
    {
        //need a check prob
        for (int i = 0; i < names.Count; i++)
        {
            for (int x = 0; x < teamMates.Count; x++)
            {
                if (names[i] == teamMates[x].GetComponent<TeamMateStateManager>().memberName)
                {

                    TeamMateStateManager TMstateMan = teamMates[x].GetComponent<TeamMateStateManager>();
                    TMstateMan.FormationTran = SquadFormations.transform.GetChild((int)playerScript.currFormation).transform.GetChild(x).transform;

                    if (TMstateMan.Allerted)
                    {
                        TMstateMan.ChangeState(6);
                    }
                    else
                    {
                        TMstateMan.ChangeState(7);
                    }

                    continue;
                }
            }
        }
    }
}
