using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radius : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    { 
        if (other.tag == "Enemy")
        {
            transform.parent.GetComponent<Tower>().AddEnemyToList(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            transform.parent.GetComponent<Tower>().RemoveEnemyFromList(other.gameObject);
        }
    }
}
