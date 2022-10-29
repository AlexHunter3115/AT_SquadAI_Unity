using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TopViewCamLogic : MonoBehaviour
{

    public static TopViewCamLogic instance;

    public List<GameObject> buildingsList;
    public int currentFloor = 0;

    public float speed = 1;


    private void Start()
    {
        instance = this;
    }

    private void FixedUpdate()
    {
        if (PlayerScript.instance.controlPlayer == false)
        {
            Vector3 movement = Vector3.zero;

            if (Input.GetKey(KeyCode.Q))
            {
                movement += Vector3.forward * speed;
            }
            if (Input.GetKey(KeyCode.E))
            {
                movement += Vector3.back * speed;
            }
            if (Input.GetKey(KeyCode.A))
            {
                movement += Vector3.left * speed;
            }
            if (Input.GetKey(KeyCode.D))
            {
                movement += Vector3.right * speed;
            }
            if (Input.GetKey(KeyCode.W))
            {
                movement += Vector3.up * speed;
            }
            if (Input.GetKey(KeyCode.S))
            {
                movement += Vector3.down * speed;
            }


            transform.Translate(movement);
        }
    }



    public void changeFloorView(int floor) 
    {
        currentFloor += floor;    // this is going to be 1 


        if (currentFloor < 0) currentFloor = 0;
        if (currentFloor > 2) currentFloor = 2;


        for (int i = 0; i < buildingsList.Count; i++)   // going to go through all of the saved buildings in the array
        {
            var childCoutn = buildingsList[i].transform.childCount - 1; // get the numebr of floors the current building has in index form
            
            for (int z = 0; z < buildingsList[i].transform.childCount; z++)  // loop through the floors
            {
                if (currentFloor < z) 
                {
                    buildingsList[i].transform.GetChild(z).transform.GameObject().SetActive(false);
                }
                else 
                {
                    buildingsList[i].transform.GetChild(z).transform.GameObject().SetActive(true);
                }
            }
        }
    }
}
