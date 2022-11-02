using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorePoints : MonoBehaviour
{


    public bool capturing;

    public float gainSpeed;
    public float loseSpeed;

    private void OnTriggerStay(Collider other)
    {

        if (other.transform.tag == "Player" || other.transform.tag == "TeamMate")
        {
            capturing = true;
            UIManager.instance.pointsSlider.value = UIManager.instance.pointsSlider.value + (Time.deltaTime * gainSpeed);
        }
    }



    // this is muhc fasetr for some reason
    private void Update()
    {
        if (UIManager.instance.pointsSlider.value > 0) 
        {
            if (!capturing) 
            {
                UIManager.instance.pointsSlider.value = UIManager.instance.pointsSlider.value - (Time.deltaTime * loseSpeed);
            }
        }

        if (UIManager.instance.pointsSlider.value > 99.99f)
        {
            Debug.Log($"you win");
        }


        capturing = false;
    }
}
