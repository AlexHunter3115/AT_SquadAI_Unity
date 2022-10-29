using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject optionsUI;

    public static UIManager instance;

    public bool showOptions;


    [SerializeField] GameObject useAbilityOBj;    // if more than 1 selected dont show

    [SerializeField] GameObject optionHoldFire;
    public bool holdFire;
    [SerializeField] GameObject optionFindCover;
    [SerializeField] GameObject optionIntoFormation;
    [SerializeField] GameObject optionDefendPoint;
    [SerializeField] GameObject optionPatrolPoint;
    [SerializeField] GameObject optionAdvance;


    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        showOptions = false;
    }

    public void ToggleOptions() 
    {
        showOptions = !showOptions;
    }



    private void Update()
    {
        if (showOptions) 
        {
            optionsUI.SetActive(true);

            if (PlayerScript.instance.teamMatesNames.Count == 1) 
            {   //check if dead

                useAbilityOBj.SetActive(true);


                var idx = PlayerScript.instance.teamMatesNames[0];

                for (int i = 0; i < SquadManager.instance.teamMates.Count; i++)
                {
                    if (idx == SquadManager.instance.teamMates[i].GetComponent<TeamMateStateManager>().memberName) 
                    {
                        if (SquadManager.instance.teamMates[i].GetComponent<TeamMateStateManager>().holdFire == true) 
                        {
                            optionHoldFire.GetComponent<RawImage>().color = Color.green;
                        }
                        else 
                        {
                            optionHoldFire.GetComponent<RawImage>().color = Color.red;
                        }
                        break;
                    }
                }
            }
            else 
            {

                useAbilityOBj.SetActive(false);
                //check for majority?

                var holding = 0;
                var firing = 0;

                for (int i = 0; i < PlayerScript.instance.teamMatesNames.Count; i++)
                {
                    if (PlayerScript.instance.teamMatesNames[i] == SquadManager.instance.teamMates[i].GetComponent<TeamMateStateManager>().memberName)
                    {
                        if (SquadManager.instance.teamMates[i].GetComponent<TeamMateStateManager>().holdFire == true)
                        {
                           firing++;
                        }
                        else
                        {
                            holding++;
                        }
                    }
                }



                if (holding > firing)
                {
                    holdFire = true;
                    optionHoldFire.GetComponent<RawImage>().color = Color.red;
                }
                else if (firing > holding) 
                {
                    holdFire = false;
                    optionHoldFire.GetComponent<RawImage>().color = Color.green;
                }
                else if (firing == holding) 
                {
                    holdFire = true;
                    optionHoldFire.GetComponent<RawImage>().color = Color.green;
                }




            }


            



        }
        else 
        {
            optionsUI.SetActive(false);
        }
    }

    public void UpdateOptionUI(List<string> teamMatesNames) 
    {
         
    
    }





}
