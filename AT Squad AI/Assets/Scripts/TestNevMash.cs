using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestNevMash : MonoBehaviour
{
    [SerializeField] private Transform movepositionTransform;

    private NavMeshAgent navMeshAgent;


    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }



    private void Update()
    {
        navMeshAgent.destination = movepositionTransform.position;
    }
}
