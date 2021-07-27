using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuraAttack : MonoBehaviour
{
    List<enemy_CS> enemiesHit = new List<enemy_CS>();
    List<int> enemiesHitTimes = new List<int>();

    Tower tower;
    float dmg;

    private void Start()
    {
        tower = GetComponentInParent<Tower>();

        dmg = tower.damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            dmg = tower.damage;

            enemy_CS enemy = other.GetComponent<enemy_CS>();

            bool foundEnemy = false;

            //for each row in the dictionary
            foreach(enemy_CS e in enemiesHit)
            {
                if (e.GetInstanceID() == enemy.GetInstanceID())
                {
                    enemiesHitTimes[enemiesHit.IndexOf(e)]++;
                    foundEnemy = true;
                }
            }

            //if didnt find the enemy
            if (!foundEnemy)
            {
                enemiesHit.Add(enemy);
                enemiesHitTimes.Add(1);
            }
                
            //if should increase damage with every hit
            if (tower.shouldDamageWithEachHit)
            {
                dmg *= (tower.damageWithEachHitAmount * enemiesHitTimes[enemiesHit.IndexOf(enemy)]);
            }

            //hit the enemy
            enemy.Hit(dmg, tower.slowMultiplier, tower.stunDuration);
        }
    }
}
