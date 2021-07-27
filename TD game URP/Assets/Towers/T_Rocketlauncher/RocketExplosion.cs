using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketExplosion : MonoBehaviour
{
    Tower tower;
    // Start is called before the first frame update
    public void StartExplosion(Tower t)
    {
        tower = t;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        { 
            other.GetComponent<enemy_CS>().Hit(tower.damage, tower.slowMultiplier, tower.stunDuration);
        }
    }
}
