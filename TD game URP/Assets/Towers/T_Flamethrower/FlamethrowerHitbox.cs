using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamethrowerHitbox : MonoBehaviour
{
    Tower tower;
    private void Start()
    {
        tower = GetComponentInParent<Tower>();
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Enemy")
        {
            //SHOOTING
            other.GetComponent<enemy_CS>().Hit(tower.damage, tower.slowMultiplier, tower.stunDuration);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            if (tower.shouldburnEnemies)
            {
                other.GetComponent<enemy_CS>().Burn(tower.burnDamage, 3f);
            }
        }
    }
}
