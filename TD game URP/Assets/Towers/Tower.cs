using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

[System.Serializable]
public class Upgrades
{
    public string name;
    public string description;
    public int cost;
    public UpgradeStat[] upgradesStats;
    public Tower tower;

    public void CheckTopUpgrades()
    {
        //check if allowed to upgrade this
        //if the top path is has been bought once and the bot path more then once
        if (tower.topPathIndex == 1 && tower.botPathIndex > 1)
        {
            PathMaxed(tower.TopPathUpgrades, tower.topPathIndex);
        }
        //if the top path has hit the max level
        else if (tower.topPathMaxed)
        {
            PathMaxed(tower.TopPathUpgrades, tower.topPathIndex - 1);
        }
    }

    public void CheckBotUpgrades()
    {
        //check if allowed to upgrade this
        //if the top path is has been bought once and the bot path more then once
        if (tower.botPathIndex == 1 && tower.topPathIndex > 1)
        {
            PathMaxed(tower.BottomPathUpgrades, tower.botPathIndex);
        }
        //if the top path has hit the max level
        else if (tower.botPathMaxed)
        {
            PathMaxed(tower.BottomPathUpgrades, tower.botPathIndex - 1);
        }
    }
    public void ApplyUpgradeTop()
    {
        //apply upgrade if can afford it
        if (ApplyUpgrade())
        {
            tower.topPathIndex++;

            if (tower.topPathIndex == tower.TopPathUpgrades.Length)
            {
                tower.topPathMaxed = true;
            }

            if (tower.towerType == towerType.Radar)
            {
                tower.GetComponentInChildren<RadarRadius>().CheckUpgradesIndividually();
            }
        }
    }

    public void ApplyUpgradeBottom()
    {
        if (ApplyUpgrade())
        {
            tower.botPathIndex++;

            if (tower.botPathIndex == tower.BottomPathUpgrades.Length)
            {
                tower.botPathMaxed = true;
            }
        }
            
            
    }

    void PathMaxed(Upgrades[] upg, int index)
    {
        upg[index].cost = 0;
        upg[index].description = "No More Upgrades";
        upg[index].name = "Path Maxed";

        for (int i = 0; i < upg[index].upgradesStats.Length; i++)
        {
            upg[index].upgradesStats[i].multiplier = 1;
        }

    }

    bool ApplyUpgrade()
    {
        //if can afford upgrade
        if (cost <= LevelManager.instance.GetMoney())
        {
            for (int i = 0; i < upgradesStats.Length; i++)
            {
                switch (upgradesStats[i].stat)
                {
                    case stat.Damage:
                        if (tower.towerType != towerType.Radar)
                            tower.damage *= upgradesStats[i].multiplier;
                        break;
                    case stat.Firerate:
                        if (tower.towerType != towerType.Radar)
                            tower.firerate /= upgradesStats[i].multiplier;
                        break;
                    case stat.Range:
                        if (tower.towerType != towerType.Radar)
                        {
                            tower.radius *= upgradesStats[i].multiplier;
                            tower.UpdateRadius(tower.radius);
                        }   
                        break;
                    case stat.Stun:
                        tower.stunDuration = upgradesStats[i].multiplier;
                        break;
                    case stat.DamageWithDistance:
                        tower.shouldDamageWithDistance = true;
                        tower.damageWithDistanceDamage = upgradesStats[i].multiplier;
                        break;
                    case stat.Ricochet:
                        tower.shouldRicochet = true;
                        tower.ricochetAmount = (int)upgradesStats[i].multiplier;
                        break;
                    case stat.Slow:
                        tower.slowMultiplier = upgradesStats[i].multiplier;
                        break;
                    case stat.DamageWithEachHit:
                        tower.shouldDamageWithEachHit = true;
                        tower.damageWithEachHitAmount = upgradesStats[i].multiplier;
                        break;
                    case stat.DamageOverTime:
                        tower.shouldburnEnemies = true;
                        tower.burnDamage = upgradesStats[i].multiplier;
                        break;
                    case stat.BlueFire:
                        ParticleSystem.MainModule flames = tower.flames.GetComponent<ParticleSystem>().main;
                        flames.startColor = tower.flameColor;
                        break;
                    case stat.SingleShot:
                        tower.shouldSingleShot = true;
                        tower.DestroyFlameHitbox();
                        tower.damage = tower.singleShotDamage;
                        break;
                    case stat.HugeExplosions:
                        tower.shouldHugeExplosions = true;
                        break;
                    case stat.MarkEnemies:
                        tower.shouldMarkEnemies = true;
                        tower.markEnemiesDamageMultiplier = upgradesStats[i].multiplier;
                        break;
                    case stat.ExpensiveEnemies:
                        tower.shouldExpensiveEnemies = true;
                        tower.enemyMoneyMultiplier = upgradesStats[i].multiplier;
                        break;
                }
            }

            tower.TowerUpgraded();

            //subtract from cost 
            LevelManager.instance.UpdateMoney(-cost);
            tower.cost += (cost / 2);

            LevelManager.instance.UI.HideUpgradesMenu();
            LevelManager.instance.UI.ShowUpgradesMenu(tower.TopPathUpgrades, tower.BottomPathUpgrades, tower);

            return true;
        }
        else
            return false;
    }
}

public enum stat
{
    Damage,
    Firerate,
    Range,
    Stun,
    DamageWithDistance,
    Ricochet,
    Slow,
    DamageWithEachHit,
    DamageOverTime,
    BlueFire,
    SingleShot,
    HugeExplosions,
    ReduceTowersUpgradeCosts,
    MarkEnemies,
    ExpensiveEnemies
}

[System.Serializable]
public class UpgradeStat
{
    [EnumToggleButtons] public stat stat;
    public float multiplier;
}

public enum towerType
{
    Gatling,
    Sniper,
    Cannon,
    Aura,
    Radar,
    Flamethrower,
    Rocketlauncher,
};


public class Tower : MonoBehaviour
{
    [Header("Towers:")]
    [EnumToggleButtons]
    [Title("Tower Type")]
    [HideLabel]
    public towerType towerType;

    //GATLING TOWER
    [Title("Gatling Tower")]
    [ShowIf("towerType", towerType.Gatling)] public Transform[] gatlingBarrels;
    [ShowIf("towerType", towerType.Gatling)] public float barrelSpinSpeed;
    [ShowIf("towerType", towerType.Gatling)] public GameObject gatlingBullet;
    [ShowIf("towerType", towerType.Gatling)] public GameObject gatlingBulletFlash;

    //SNIPER TOWER
    [Title("Sniper Tower")]
    [ShowIf("towerType", towerType.Sniper)] public GameObject[] sniperBarrels;
    [ShowIf("towerType", towerType.Sniper)] public GameObject[] sniperBarrelMoveToPos;
    [ShowIf("towerType", towerType.Sniper)] public ParticleSystem sniperMuzzleFlash;

    //CANNON TOWER
    [Title("Cannon Tower")]
    [ShowIf("towerType", towerType.Cannon)] public int ricochetAmount;
    [ShowIf("towerType", towerType.Cannon)] public GameObject cannonBullet;

    //AURA TOWER
    [Title("Aura Tower")]
    [ShowIf("towerType", towerType.Aura)] public Transform auraRadius;
    [ShowIf("towerType", towerType.Aura)] public Transform attackRadius;

    //RADAR TOWER
    [Title("Radar Tower")]
    [ShowIf("towerType", towerType.Radar)] public float rangeIncreaseMultiplier;

    //FLAMETHROWER TOWER
    [Title("Flamethrower Tower")]
    [ShowIf("towerType", towerType.Flamethrower)] public GameObject flames;
    [ShowIf("towerType", towerType.Flamethrower)] public Color flameColor;
    [ShowIf("towerType", towerType.Flamethrower)] public GameObject flameHitBox;
    [ShowIf("towerType", towerType.Flamethrower)] public GameObject flameBullet;
    [ShowIf("towerType", towerType.Flamethrower)] public GameObject flameBulletFlash;
    [ShowIf("towerType", towerType.Flamethrower)] public float singleShotDamage;

    //ROCKETLAUNCHER TOWER
    [Title("RocketLauncher Tower")]
    [ShowIf("towerType", towerType.Rocketlauncher)] public GameObject rocketBullet;
    [ShowIf("towerType", towerType.Rocketlauncher)] public Transform rocketSpawnLocations1;
    [ShowIf("towerType", towerType.Rocketlauncher)] public Transform rocketSpawnLocations2;
    [ShowIf("towerType", towerType.Rocketlauncher)] public Transform[] rocketSpawnLocations3;
    [ShowIf("towerType", towerType.Rocketlauncher)] public GameObject Explosion;
    [ShowIf("towerType", towerType.Rocketlauncher)] public GameObject HugeExplosion;
    int rocketSpawnIndex = 0;

    [Header("STATS")]
    public float firerate;
    public float radius;
    public float damage;
    public int cost;

    [HideInEditorMode] public float originalFirerate;
    [HideInEditorMode] public float originalRadius;
    [HideInEditorMode] public float originalDamage;


    [HideInEditorMode] public float slowMultiplier = 1f;
    [HideInEditorMode] public float stunDuration = 0f;

    [Header("SHOOTING")]
    
    public Transform[] bulletSpawnLocations;

    [Header("UPGRADES")]
    public Upgrades[] TopPathUpgrades;
    public Upgrades[] BottomPathUpgrades;
    [HideInInspector] public int topPathIndex = 0;
    [HideInInspector] public int botPathIndex = 0;
    [HideInInspector] public bool topPathMaxed = false;
    [HideInInspector] public bool botPathMaxed = false;
    public GameObject[] towerMeshes;

    [Header("Layermask")]
    public LayerMask layers;

    [Header("Radius")]
    public Transform radiusSphere;

    [HideInEditorMode] public RadarRadius isConnectedToRadarTower = null;

    List<GameObject> enemiesInRange = new List<GameObject>();
    Transform currentTarget = null;
    bool canShoot = true;
    int upgradeIndex = 0;

    //Upgrade variables
    //damage with distance
    [HideInEditorMode] public bool shouldDamageWithDistance = false;
    [HideInEditorMode] public float damageWithDistanceDamage = 0f;

    //ricochet
    [HideInEditorMode] public bool shouldRicochet = false;

    //damage with each hit
    [HideInEditorMode] public bool shouldDamageWithEachHit = false;
    [HideInEditorMode] public float damageWithEachHitAmount = 0f;

    //mark enemies
    [HideInEditorMode] public bool shouldMarkEnemies = false;
    [HideInEditorMode] public float markEnemiesDamageMultiplier = 0f;

    //expensive enemies
    [HideInEditorMode] public bool shouldExpensiveEnemies = false;
    [HideInEditorMode] public float enemyMoneyMultiplier = 0f;

    //burn enemies
    [HideInEditorMode] public bool shouldburnEnemies = false;
    [HideInEditorMode] public float burnDamage = 0f;

    //single shot
    [HideInEditorMode] public bool shouldSingleShot = false;

    //huge explosions
    [HideInEditorMode] public bool shouldHugeExplosions = false;

    // Start is called before the first frame update
    void Start()
    {
        UpdateRadius(radius);
        radiusSphere.gameObject.GetComponent<Renderer>().enabled = false;

        originalDamage = damage;
        originalFirerate = firerate;
        originalRadius = radius;
    }

    // Update is called once per frame
    void Update()
    {
        //look
        if (currentTarget)
        {
            if (towerType != towerType.Aura && towerType != towerType.Radar)
                transform.LookAt(new Vector3(currentTarget.position.x, 0, currentTarget.position.z));

            //if its a gating tower
            if (towerType == towerType.Gatling)
            {
                //spin the barrels
                gatlingBarrels[upgradeIndex].transform.Rotate(0, 0, barrelSpinSpeed, Space.Self);
            }

            //spawn the flames
            if (towerType == towerType.Flamethrower && !shouldSingleShot)
            {
                flames.SetActive(true);
            }
        }
        else
        {
            if (towerType == towerType.Flamethrower && !shouldSingleShot)
            {
                flames.SetActive(false);
            }   
        }


        if (canShoot && currentTarget)
        {
            switch (towerType)
            {
                case towerType.Gatling:
                    StartCoroutine("GatlingShoot");
                    break;
                case towerType.Sniper:
                    StartCoroutine("SniperShoot");
                    break;
                case towerType.Cannon:
                    StartCoroutine("CannonShoot");
                    break;
                case towerType.Aura:
                    StartCoroutine("AuraShoot");
                    break;
                case towerType.Flamethrower:
                    if (shouldSingleShot)
                        StartCoroutine("FlamethrowerShoot");
                    break;
                case towerType.Rocketlauncher:
                    StartCoroutine("RocketlauncherShoot");
                    break;
            }
        }
    }

    public void ShowRadius(bool b)
    {
        radiusSphere.gameObject.GetComponent<Renderer>().enabled = b;
    }

    IEnumerator GatlingShoot()
    {
        canShoot = false;

        Destroy(Instantiate(gatlingBulletFlash, bulletSpawnLocations[upgradeIndex]), 0.3f);

        //shoot
        GameObject cacheObj = Instantiate(gatlingBullet, transform);
        cacheObj.transform.position = bulletSpawnLocations[upgradeIndex].position;
        cacheObj.transform.parent = null;
        cacheObj.GetComponent<Bullet>().StartBullet(currentTarget.gameObject, this);

        yield return new WaitForSeconds(firerate);

        canShoot = true;
    }

    IEnumerator SniperShoot()
    {
        canShoot = false;

        //shoot
        //instantiate muzzle flash
        sniperMuzzleFlash.transform.position = bulletSpawnLocations[upgradeIndex].position;
        sniperMuzzleFlash.Play();

        //move barrel back and forth
        sniperBarrels[upgradeIndex].SetActive(false);
        sniperBarrelMoveToPos[upgradeIndex].SetActive(true);

        yield return new WaitForSeconds(0.075f);

        sniperBarrels[upgradeIndex].SetActive(true);
        sniperBarrelMoveToPos[upgradeIndex].SetActive(false);

        //hit enemy
        if (!shouldDamageWithDistance)
        {
            currentTarget.GetComponent<enemy_CS>().Hit(damage, slowMultiplier, stunDuration);    
        }
        //hit enemy with additionaly damage from distance
        else
        {
            float dmg = damage;
            float distance = Vector3.Distance(transform.position, currentTarget.position);
            dmg += distance * damageWithDistanceDamage;
            currentTarget.GetComponent<enemy_CS>().Hit(dmg, slowMultiplier, stunDuration);
            
        }
        

        yield return new WaitForSeconds(firerate - 0.075f);

        canShoot = true;
    }

    IEnumerator CannonShoot()
    {
        canShoot = false;

        //shoot
        GameObject cacheObj = Instantiate(cannonBullet, transform);
        cacheObj.transform.position = bulletSpawnLocations[upgradeIndex].position;
        cacheObj.transform.parent = null;
        cacheObj.GetComponent<CannonBall>().StartCannonBall(currentTarget.gameObject, this);

        yield return new WaitForSeconds(firerate);

        canShoot = true;
    }

    IEnumerator AuraShoot()
    {
        canShoot = false;

        float t = 0.1f;

        while (t < auraRadius.localScale.x)
        {
            attackRadius.localScale = new Vector3(t, 0.01f, t);
            t += 0.2f;
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        while (t > 0.1)
        {
            attackRadius.localScale = new Vector3(t, 0.01f, t);
            t -= 0.4f;
            yield return null;
        }

        yield return new WaitForSeconds(firerate - 0.1f);

        canShoot = true;
    }

    IEnumerator FlamethrowerShoot()
    {
        canShoot = false;

        Destroy(Instantiate(flameBulletFlash, bulletSpawnLocations[upgradeIndex]), 0.3f);

        //shoot
        GameObject cacheObj = Instantiate(flameBullet, transform);
        cacheObj.transform.position = bulletSpawnLocations[upgradeIndex].position;
        cacheObj.transform.parent = null;
        cacheObj.GetComponent<FlameBullet>().StartBullet(currentTarget.gameObject, this);

        yield return new WaitForSeconds(firerate);

        canShoot = true;
    }

    IEnumerator RocketlauncherShoot()
    {
        canShoot = false;

        GameObject cacheObj = Instantiate(rocketBullet, transform);

        //shoot
        if (upgradeIndex == 0)
        {         
            cacheObj.transform.position = rocketSpawnLocations1.position; 
        }
        else if (upgradeIndex == 1)
        {
            cacheObj.transform.position = rocketSpawnLocations2.position;
        }
        else if (upgradeIndex > 1)
        {
            cacheObj.transform.position = rocketSpawnLocations3[rocketSpawnIndex].position;

            rocketSpawnIndex++;

            if (rocketSpawnIndex >= rocketSpawnLocations3.Length)
            {
                rocketSpawnIndex = 0;
            }
        }

        print(shouldHugeExplosions);

        //if not huge explosions
        if (!shouldHugeExplosions)
            cacheObj.GetComponent<Rocket>().StartBullet(currentTarget.gameObject, Explosion);
        else
            cacheObj.GetComponent<Rocket>().StartBullet(currentTarget.gameObject, HugeExplosion);

        yield return new WaitForSeconds(firerate);

        canShoot = true;
    }

    public void DestroyFlameHitbox()
    {
        Destroy(flames.gameObject);
        Destroy(flameHitBox);
    }

    public void UpdateRadius(float radius)
    {
        radiusSphere.localScale = new Vector3(radius, 0.01f, radius);
    }

    public void TowerUpgraded()
    {
        if (upgradeIndex < towerMeshes.Length - 1)
        {
            towerMeshes[upgradeIndex].SetActive(false);
            upgradeIndex++;
            towerMeshes[upgradeIndex].SetActive(true);
        }
        
    }

    void GetNewTarget()
    {
        if (enemiesInRange.Count > 0)
        {
            int index = 0;
            float fIndex = 0;
            List<GameObject> furthestEnemies = new List<GameObject>();

            //loop through all enemies in range and create a list with all enemies on the highest way point index
            foreach (GameObject enemy in enemiesInRange)
            {
                //get the enemy componant
                enemy_CS e = enemy.GetComponent<enemy_CS>();

                //if the waypoint index is higher then the current index
                if (e.waypointIndex > index)
                {
                    //Reset the list
                    furthestEnemies.Clear();

                    //set the current index to this index
                    index = e.waypointIndex;

                    //add this enemy to that list
                    furthestEnemies.Add(enemy);
                }
                else if (e.waypointIndex == index)
                {
                    //add to list
                    furthestEnemies.Add(enemy);
                }
            }

            //loop through all enemies in the furthest enemies list and get the smallest distance to the end of their current waypoint
            foreach (GameObject enemy in furthestEnemies)
            {
                float distance = enemy.GetComponent<enemy_CS>().ClosestToCurrentWaypoint();

                //if the index is 0 (first iteration)
                if (fIndex == 0)
                    //set the index
                    fIndex = distance;
                //if the index is not 0
                else
                {
                    //if the distance is smaller then current index (closer)
                    if (distance < fIndex)
                    {
                        //set the idex to the distance
                        fIndex = distance;
                    }
                }
            }

            //loop through all enemies in the furthest enemeis list and if the smallest distance matches that enemy, make it the target
            foreach(GameObject enemy in furthestEnemies)
            {
                if (enemy.GetComponent<enemy_CS>().ClosestToCurrentWaypoint() == fIndex)
                {
                    currentTarget = enemy.transform;
                }
            }
        }
    }

    //reference function for the radius to update the list
    public void AddEnemyToList(GameObject enemy)
    {
        //if no enemies in range at the moment
        if (enemiesInRange.Count == 0)
        {
            //set this enemy to be the target and add it to range list
            enemiesInRange.Add(enemy);
            currentTarget = enemy.transform;
        }
        //if enemies already in range
        else
        {
            //add enemies to range list
            enemiesInRange.Add(enemy);
        }


        enemy.GetComponent<enemy_CS>().AddTowerToList(gameObject);
    }

    public void RemoveEnemyFromList(GameObject enemy)

    {
        //if the enemy leaving the range is the current target
        if (currentTarget == enemy.transform)
        {
            //set target to null
            currentTarget = null;
            //remove enemy from range
            enemiesInRange.Remove(enemy);
            //get a new target
            GetNewTarget();
        }
        //if the enemy is not the target
        else
            //remove from list
            enemiesInRange.Remove(enemy);

        enemy.GetComponent<enemy_CS>().AddTowerToList(gameObject);
    }

    public void EnemyDied(GameObject enemy)
    {
        //if the enemy leaving the range is the current target
        if (currentTarget == enemy.transform)
        {
            //set target to null
            currentTarget = null;
            //remove enemy from range
            enemiesInRange.Remove(enemy);
            //get a new target
            GetNewTarget();
        }
        //if the enemy is not the target
        else
            //remove from list
            enemiesInRange.Remove(enemy);
    }

}
