using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class TestNevMash : MonoBehaviour
{
    public Transform movepositionTransform;
    public float speed = 4;
    //private NavMeshAgent navMeshAgent;

    //public GameObject thing;
    private void Awake()
    {
        //navMeshAgent = GetComponent<NavMeshAgent>();
    }



    private void Update()
    {
        //navMeshAgent.destination = movepositionTransform.position;

        //Vector3 lookAtVector = new Vector3 (movepositionTransform.position.x, movepositionTransform.position.y, transform.position.z);

        //movepositionTransform
        //transform.LookAt(movepositionTransform);
        Vector3 relativePos = movepositionTransform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos);

        Quaternion current = transform.localRotation;

        transform.localRotation = Quaternion.Slerp(current, rotation, Time.deltaTime
            * speed);
    }
 }
