using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destory : MonoBehaviour
{
    public float time;

    private void Start()
    {
        Destroy(this.gameObject, time);
    }
}
