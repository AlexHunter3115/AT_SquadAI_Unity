using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleObjectCover : MonoBehaviour
{
    public List<Transform> listOfPossiblePos;
    public List<bool> listOfAvailability;

    public GameObject coverCubes;
    public bool showCubes;



    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < listOfPossiblePos.Count; i++)
        {
            listOfAvailability.Add(true);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (showCubes) 
        {
            coverCubes.SetActive(true);
        }
        else 
        {
            coverCubes.SetActive(false);
        }

        showCubes = false;

    }

    void OnDrawGizmosSelected()
    {
        for (int i = 0; i < listOfPossiblePos.Count; i++)
        {
            if (listOfAvailability[i])
                Gizmos.color = Color.yellow;
            else
                Gizmos.color = Color.red;

            Gizmos.DrawSphere(listOfPossiblePos[i].position, 0.3f);
        }
    }



    public int findIndexCover(GameObject cover) 
    {
        int idx = 0;
        foreach (Transform child in coverCubes.transform) 
        {

            if (cover == child.gameObject)
            {
                return idx;
            }

            idx++;
        }

        idx = -1;

        return idx;
        
    }
}
