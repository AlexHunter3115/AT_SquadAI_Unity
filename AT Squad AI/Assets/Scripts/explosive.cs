using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosive : MonoBehaviour
{

    [SerializeField] GameObject effect;



    private void OnCollisionEnter(Collision collision)
    {
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 20);

        foreach (var item in hitColliders)
        {
            if (item.transform.tag == "TeamMate")
            {
                item.transform.root.GetComponent<TeamMateStateManager>().TakeDamage(10);
            }
            else if (item.transform.tag == "Enemy")
            {
                item.transform.root.GetComponent<EnemyScript>().TakeDamage(40);
            }
        }

        Instantiate(effect, this.transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
