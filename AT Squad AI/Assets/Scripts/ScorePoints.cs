using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorePoints : MonoBehaviour
{
    public static ScorePoints instance;

    public bool capturing;

    public float gainSpeed;
    public float loseSpeed;


    public Light light;



    private void Start()
    {
        instance = this;
    }

    private void OnTriggerStay(Collider other)
    {

        if (other.transform.tag == "Player" || other.transform.tag == "TeamMate")
        {

            enemySpawnerManager.instance.allowed = true;

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

        Color color = Color.Lerp(Color.red, Color.blue, UIManager.instance.pointsSlider.value / 100f);
        // Set the color of the light
        light.color = color;


        if (UIManager.instance.pointsSlider.value > 99.99f)
        {
            Debug.Log($"you win");
            Application.Quit();
        }


        capturing = false;
    }
}
