using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBallRadius : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            transform.parent.GetComponent<CannonBall>().EnemiesInRicRange.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            transform.parent.GetComponent<CannonBall>().EnemiesInRicRange.Remove(other.gameObject);
        }
    }
}
