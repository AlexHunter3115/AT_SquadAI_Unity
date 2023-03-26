using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorePoints : MonoBehaviour
{
    public static ScorePoints instance;

    public bool capturing;

    public float gainSpeed;
    public float loseSpeed;

    bool captured = false;

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

    private void Update()
    {
        if (!captured) 
        {
            if (UIManager.instance.pointsSlider.value > 0)
            {
                if (!capturing)
                {
                    UIManager.instance.pointsSlider.value = UIManager.instance.pointsSlider.value - (Time.deltaTime * loseSpeed);
                }
            }

            Color color = Color.Lerp(Color.red, Color.blue, UIManager.instance.pointsSlider.value / 100f);

            light.color = color;

            if (UIManager.instance.pointsSlider.value > 99.99f)
            {
                captured = true;
                UIManager.instance.AddNewMessageToQueue("YOU WIN, YOU CAPPED THE POINT", Color.green);
                UIManager.instance.AddNewMessageToQueue("YOU WIN, YOU CAPPED THE POINT", Color.green);
            }

            capturing = false;
        }
    }
}
