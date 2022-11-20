using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseFloorLogic : MonoBehaviour
{
    public List<GameObject> floorsList;
    public List<Transform> coverPos;
    public List<GameObject> coverPosCubes;

    public List<GameObject> mainParentCubes;

    public bool showCubes;


    private void Start()
    {

        for (int i = 0; i < floorsList.Count; i++)// loop through all of the floors
        {
            int childCount = floorsList[i].transform.childCount;  // get the child count of that floor
            int lastChild = childCount - 1;  // get the last index for that floor

            for (int z = 0; z < floorsList[i].transform.GetChild(lastChild).transform.childCount   ; z++)  // get the num of children in the last child
            {

                if (z == floorsList[i].transform.GetChild(lastChild).transform.childCount - 1)   // if its the last child then do the same
                {
                    for (int x = 0; x < floorsList[i].transform.GetChild(lastChild).transform.GetChild(z).transform.childCount; x++)
                    {
                        coverPosCubes.Add(floorsList[i].transform.GetChild(lastChild).transform.GetChild(z).transform.GetChild(x).gameObject);
                    }
                }
                else 
                {

                    coverPos.Add(floorsList[i].transform.GetChild(lastChild).transform.GetChild(z).transform);
                }
            }
        }
    }


    public int findIndexCover(GameObject cover)
    {
        int idx = 0;
        foreach (GameObject obj in coverPosCubes)
        {

            if (cover == obj)
            {
                return idx;
            }

            idx++;
        }

        idx = -1;

        return idx;

    }





    void Update()
    {

        for (int i = 0; i < mainParentCubes.Count; i++)
        {
            mainParentCubes[i].SetActive(showCubes);
        }



        //if (showCubes)
        //{
        //    for (int i = 0; i < mainParentCubes.Count; i++)
        //    {
        //        mainParentCubes[i].SetActive(true);
        //    }
        //}
        //else
        //{
        //    for (int i = 0; i < mainParentCubes.Count; i++)
        //    {
        //        mainParentCubes[i].SetActive(false);
        //    }
        //}

        showCubes = false;

    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerScript.instance.playerInHouse = true;
    }
    private void OnTriggerExit(Collider other)
    {
        PlayerScript.instance.playerInHouse = false;
    }

}
