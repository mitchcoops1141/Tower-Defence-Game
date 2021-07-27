using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarRadius : MonoBehaviour
{
    [HideInInspector] public List<Tower> towers = new List<Tower>();
    Tower radar;

    private void Start()
    {
        radar = GetComponentInParent<Tower>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //if finds a tower
        if (other.tag == "RadarFinder")
        {
            //if its not this tower
            if (other.transform.parent.gameObject != this.transform.parent.gameObject)
            {
                //check upgrades that are unlocked and increase things as expected
                Tower tower = other.GetComponentInParent<Tower>();
                print("Found: " + tower.name);

                //increase all stats
                tower.damage *= radar.damage;
                tower.firerate /= radar.firerate;
                tower.radius *= radar.rangeIncreaseMultiplier;
                tower.UpdateRadius(tower.radius);

                towers.Add(tower);
                tower.isConnectedToRadarTower = this;

                CheckUpgradesTogether(tower);
            }
        }


        if (other.tag == "Enemy")
        {
            enemy_CS enemy = other.GetComponent<enemy_CS>();

            //mark enemy
            enemy.Mark(radar.shouldMarkEnemies, radar.markEnemiesDamageMultiplier);

            //increase enemy money
            if (radar.shouldExpensiveEnemies)
            {
                float newAmount = enemy.moneyWhenKilled;
                newAmount = newAmount * radar.enemyMoneyMultiplier;
                enemy.moneyWhenKilled = Mathf.RoundToInt(newAmount);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        
        if (other.tag == "Enemy")
        {
            enemy_CS enemy = other.GetComponent<enemy_CS>();

            //unmark enemy
            enemy.Mark(false, 1f);

            //decrease enemy money
            if (radar.shouldExpensiveEnemies)
            {
                float newAmount = enemy.moneyWhenKilled;
                newAmount = newAmount / radar.enemyMoneyMultiplier;
                enemy.moneyWhenKilled = Mathf.RoundToInt(newAmount);
            }
        }
    }

    void CheckUpgradesTogether(Tower tower)
    {
        //if unlocked upgrade Top 1
        if (radar.topPathIndex >= 1)
        {
            //increase firerate
            tower.firerate /= radar.TopPathUpgrades[0].upgradesStats[0].multiplier;

            if (radar.topPathIndex >= 2)
            {
                //decrease upgrade costs
                foreach (Upgrades upgrade in tower.TopPathUpgrades)
                {
                    float newCost = upgrade.cost;
                    newCost = newCost * radar.TopPathUpgrades[1].upgradesStats[0].multiplier;
                    upgrade.cost = Mathf.RoundToInt(newCost);
                }

                foreach (Upgrades upgrade in tower.BottomPathUpgrades)
                {
                    float newCost = upgrade.cost;
                    newCost = newCost * radar.TopPathUpgrades[1].upgradesStats[0].multiplier;
                    upgrade.cost = Mathf.RoundToInt(newCost);
                }

                if (radar.topPathIndex >= 3)
                {
                    //all stats up
                    for (int i = 0; i < radar.TopPathUpgrades[2].upgradesStats.Length; i++)
                    {
                        switch (radar.TopPathUpgrades[2].upgradesStats[i].stat)
                        {
                            case stat.Damage:
                                tower.damage *= radar.TopPathUpgrades[2].upgradesStats[i].multiplier;
                                break;
                            case stat.Firerate:
                                tower.firerate /= radar.TopPathUpgrades[2].upgradesStats[i].multiplier;
                                break;
                            case stat.Range:
                                tower.radius *= radar.TopPathUpgrades[2].upgradesStats[i].multiplier;
                                tower.UpdateRadius(tower.radius);
                                break;
                        }
                    }
                }
            }
        }
        
        
    }

    public void CheckUpgradesIndividually()
    {
        foreach(Tower tower in towers)
        {
            //if unlocked upgrade Top 1
            if (radar.topPathIndex == 1)
            {
                //increase firerate
                tower.firerate /= radar.TopPathUpgrades[0].upgradesStats[0].multiplier;
            }
            else if (radar.topPathIndex == 2)
            {
                //decrease upgrade costs
                foreach (Upgrades upgrade in tower.TopPathUpgrades)
                {
                    float newCost = upgrade.cost;
                    newCost = newCost * radar.TopPathUpgrades[1].upgradesStats[0].multiplier;
                    upgrade.cost = Mathf.RoundToInt(newCost);
                }

                foreach (Upgrades upgrade in tower.BottomPathUpgrades)
                {
                    float newCost = upgrade.cost;
                    newCost = newCost * radar.TopPathUpgrades[1].upgradesStats[0].multiplier;
                    upgrade.cost = Mathf.RoundToInt(newCost);
                }
            }
            else if (radar.topPathIndex == 3)
            {
                //all stats up
                for (int i = 0; i < radar.TopPathUpgrades[2].upgradesStats.Length; i++)
                {
                    switch (radar.TopPathUpgrades[2].upgradesStats[i].stat)
                    {
                        case stat.Damage:
                            tower.damage *= radar.TopPathUpgrades[2].upgradesStats[i].multiplier;
                            break;
                        case stat.Firerate:
                            tower.firerate /= radar.TopPathUpgrades[2].upgradesStats[i].multiplier;
                            break;
                        case stat.Range:
                            tower.radius *= radar.TopPathUpgrades[2].upgradesStats[i].multiplier;
                            tower.UpdateRadius(tower.radius);
                            break;
                    }
                }
            }
        }     
    }

    public void SoldThisTower()
    {
        foreach(Tower tower in towers)
        {
            tower.firerate = tower.originalFirerate;
            tower.damage = tower.originalDamage;
            tower.radius = tower.originalRadius;
            tower.UpdateRadius(tower.radius);

            if (radar.topPathIndex >= 2)
            {
                //decrease upgrade costs
                foreach (Upgrades upgrade in tower.TopPathUpgrades)
                {
                    float newCost = upgrade.cost;
                    newCost = newCost / radar.TopPathUpgrades[1].upgradesStats[0].multiplier;
                    upgrade.cost = Mathf.RoundToInt(newCost);
                }

                foreach (Upgrades upgrade in tower.BottomPathUpgrades)
                {
                    float newCost = upgrade.cost;
                    newCost = newCost / radar.TopPathUpgrades[1].upgradesStats[0].multiplier;
                    upgrade.cost = Mathf.RoundToInt(newCost);
                }
            }
        }
    }
}
