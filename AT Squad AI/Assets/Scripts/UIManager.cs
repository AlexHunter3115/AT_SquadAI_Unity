using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject optionsUI;
    [SerializeField] GameObject canvas;

    public static UIManager instance;

    public bool showOptions;

    public Slider pointsSlider;

    [SerializeField] GameObject useAbilityOBj;    // if more than 1 selected dont show

    [SerializeField] GameObject optionHoldFire;
    public bool holdFire;
    [SerializeField] GameObject optionFindCover;
    [SerializeField] GameObject optionIntoFormation;
    [SerializeField] GameObject optionDefendPoint;
    [SerializeField] GameObject optionPatrolPoint;
    [SerializeField] GameObject optionAdvance;

    private Queue<Message> messageQueue = new Queue<Message>();
    [SerializeField] GameObject messagePrefab;
    private bool currentMessageDone = true;

    public List<Texture> icons = new List<Texture>();
    private GameObject currentMessage;

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
                optionPatrolPoint.SetActive(false);
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

        for (int i = 0; i < SquadManager.instance.uiList.Count; i++)
        {
            SquadManager.instance.uiList[i].GetComponent<TeamMateUISlot>().distanceText.text = "Distance: " + Vector3.Distance(PlayerScript.instance.transform.position, SquadManager.instance.teamMates[i].transform.position);
            SquadManager.instance.uiList[i].GetComponent<TeamMateUISlot>().healthSlider.value =  SquadManager.instance.teamMates[i].GetComponent<TeamMateStateManager>().Health;
        }

        if (messageQueue.Count > 0 && currentMessageDone == true) 
        {
            currentMessageDone = false;

            Message message = messageQueue.Dequeue();

            currentMessage = Instantiate(messagePrefab,canvas.transform);
            currentMessage.GetComponent<MessageUI>().SetMessage(message.text,message.color);
        }
    }

    public void SetIcon(int iconIdx, string nameTm) 
    {
        for (int i = 0; i < SquadManager.instance.uiList.Count; i++)
        {
            if (SquadManager.instance.uiList[i].GetComponent<TeamMateUISlot>().nameText.text.Contains(nameTm)) 
            {
                SquadManager.instance.uiList[i].GetComponent<TeamMateUISlot>().stateBack.texture = icons[iconIdx];
            }
        }
    }

    public class Message 
    {
        public string text = "";
        public Color color = Color.red;

        public Message(string text, Color color) 
        {
            this.text = text;
            this.color = color;
        }
    }

    public void AddNewMessageToQueue(string text, Color color) => messageQueue.Enqueue(new Message(text, color));
    public void SetCurrentMessageToDone() 
    {
        currentMessageDone = true;
        Destroy(currentMessage);
    }



}
