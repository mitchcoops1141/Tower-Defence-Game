using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public enum EnemyType
{
    Standard,
    DemonWarrior,
    SlimeMan
}


public class enemy_CS : MonoBehaviour
{
    [Header("Enemies:")]
    [EnumToggleButtons]
    [Title("Enemy Type")]
    [HideLabel]
    public EnemyType enemyType;

    //Demon Warrior
    [Title("Demon Warrior")]
    [ShowIf("enemyType", EnemyType.DemonWarrior)] public float healthAfterRage;
    [ShowIf("enemyType", EnemyType.DemonWarrior)] public float speedAfterRage;

    //Slime Man
    [Title("Slime Man")]
    [ShowIf("enemyType", EnemyType.SlimeMan)] public int numberOfSlimes;
    [ShowIf("enemyType", EnemyType.SlimeMan)] public GameObject slime;

    [Header("STATS")]
    public float speed = 10f;
    [HideInEditorMode] public float speedMod = 1f;
    public float health = 7;
    float maxHealth;
    public int moneyWhenKilled;

    [HideInInspector] public bool isSlowed = false;
    [HideInInspector] public bool isWeak = false;
    [HideInInspector] public float weakMultiplier = 1f;

    [Header("CONNECTIONS")]
    public Material hitMat;
    public Renderer ren;

    Material startMat;
    [HideInEditorMode] public Animator ac;

    bool canSwapHitMat = true;
    bool isBurning = false;
    bool isDead = false;
    bool isStunned = false;
    bool canBeCrowdControlled = true;
    bool canbeHit = true;

    //enemy type bools
    bool demonWarriorShouldRage = true;

    [Header("HealthBar")]
    public Slider healthBar;

    [HideInInspector] public Vector3 target;
    [HideInInspector] public Transform[] targets;
    [HideInInspector] public int waypointIndex = 0;

    List<GameObject> towersInEnemiesRange = new List<GameObject>();

    private void Awake()
    {
        ac = GetComponentInChildren<Animator>();
    
        targets = LevelManager.instance.paths[LevelManager.instance.pathIndex].paths;
        target = targets[0].position;
    }

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = health;

        startMat = ren.material;

       

        //set the health bar
        healthBar.maxValue = health;
        healthBar.minValue = 0;
        healthBar.value = health;
    }

    // Update is called once per frame
    void Update()
    {
        //rotate the healthbar
        healthBar.transform.rotation = Camera.main.transform.rotation;

        //get direction to target
        Vector3 dir = target - transform.position;

        //move the enemy
        transform.Translate(dir.normalized * (speed * speedMod) * Time.deltaTime, Space.World);

        //rotate the enemy
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);

        //if we reached the target
        if (Vector3.Distance(transform.position, target) <= 0.03)
        {
            if (waypointIndex == targets.Length - 1)
            {
                LevelManager.instance.UpdateHealth(-1);

                Die(0);
            }
            else
            {
                waypointIndex++;
                target = targets[waypointIndex].position;
            }
        }
        else if (isDead)
            speed = 0;

        print(speed);
    }

    public void Stun(float time)
    {
        if (!isStunned)
        {
            if (time > 0)
            {
                ac.SetTrigger("Stun");
                StartCoroutine("StunIE", time);
            }             
        }     
    }

    IEnumerator StunIE(float time)
    {
        isStunned = true;

        float cacheSpeed = speed;
        speed = 0;

        yield return new WaitForSeconds(time);

        speed = cacheSpeed;

        isStunned = false;
    }
    public void Mark(bool weak, float multiplier)
    {
        isWeak = weak;
        weakMultiplier = multiplier;
    }

    public void Burn(float dmg, float time)
    {
        if (!isBurning)
            StartCoroutine(BurnIE(dmg, time));
    }

    IEnumerator BurnIE(float dmg, float time)
    {
        isBurning = true;

        float t = time;

        while (t > 0)
        {
            Hit(dmg,1,0);

            t -= Time.deltaTime;

            yield return null;
        }

        isBurning = false;
    }

    public void Hit(float dmg, float slowMultiplier, float stunTime)
    {
        if (canbeHit)
        {
            if (isWeak)
                dmg *= weakMultiplier;

            //lose health
            health -= dmg;

            //lose health on healthbar
            healthBar.value -= dmg;

            //if is demon warrior and less then half health
            if (enemyType == EnemyType.DemonWarrior && health < maxHealth / 2 && demonWarriorShouldRage)
            {
                StartCoroutine("EnrageDemonWarrior");
            }

            

            //if health is less the 0. DIE
            if (health <= 0)
            {
                Die(moneyWhenKilled);
            }
            else if(canBeCrowdControlled)
            {
                StartSlow(slowMultiplier);

                Stun(stunTime);
            }

            if (canSwapHitMat)
                StartCoroutine("HitColor");       
        }
    }

    IEnumerator EnrageDemonWarrior()
    {
        //speed
        speed = 0;
        
        //animations
        ac.SetTrigger("Rage");

        //bools
        canBeCrowdControlled = false;
        demonWarriorShouldRage = false;
        canbeHit = false;

        yield return new WaitForSeconds(ac.GetCurrentAnimatorClipInfo(0).Length);

        //speed
        speed = speedAfterRage;

        //healthbar
        health = healthAfterRage;
        healthBar.maxValue = healthAfterRage;
        healthBar.value = healthAfterRage;

        //bool resets
        canbeHit = true;
    }

    IEnumerator HitColor()
    {
        canSwapHitMat = false;

        //change the color
        ren.material = hitMat;

        yield return new WaitForSeconds(0.15f);

        ren.material = startMat;

        canSwapHitMat = true;
    }

    void Die(int money)
    {
        isDead = true;

        ac.SetTrigger("Death");

        //for each tower that the enmy is in their range
        foreach (GameObject tower in towersInEnemiesRange)
        {
            //remove the enemy from the list
            tower.GetComponent<Tower>().EnemyDied(gameObject);
        }

        //update stats in wavemanager
        WaveManager_CS.instance.enemyKilled();

        //update money
        LevelManager.instance.UpdateMoney(money);

        if (enemyType != EnemyType.SlimeMan)
            Destroy(gameObject, ac.GetCurrentAnimatorClipInfo(0).Length + 0.5f);
        else
        {
            StartCoroutine("SpawningSlimes");
            Destroy(gameObject, ac.GetCurrentAnimatorClipInfo(0).Length + 0.5f);
        }
    }

    IEnumerator SpawningSlimes()
    {
        int x = 0;

        while (x < numberOfSlimes)
        {   
            GameObject cacheObj = Instantiate(slime, transform);
            cacheObj.transform.parent = null;
            cacheObj.GetComponent<enemy_CS>().SlimeSpawned(waypointIndex, targets, target);
            yield return new WaitForSeconds(0.5f);
            x++;
        }
        
    }

    public void SlimeSpawned(int waypoint, Transform[] ts, Vector3 t)
    {
        waypointIndex = waypoint;
        targets = ts;
        target = t;
        StartCoroutine("SlimeSpawnedIE");
    }

    IEnumerator SlimeSpawnedIE()
    {
        canbeHit = false;
        float cacheSpeed = speed;
        speed = 0;

        //set the animation to birth slime
        ac.SetTrigger("BirthSlime");

        yield return new WaitForSeconds(ac.GetCurrentAnimatorClipInfo(0).Length);

        canbeHit = true;
        speed = cacheSpeed;
    }

    public float ClosestToCurrentWaypoint()
    {
        return Vector3.Distance(transform.position, targets[waypointIndex].position);
    }

    public void AddTowerToList(GameObject tower)
    {
        towersInEnemiesRange.Add(tower);
    }

    public void Remove(GameObject tower)
    {
        towersInEnemiesRange.Remove(tower);
    }

    public void StartSlow(float modifier)
    {
        //if not already slowed
        if (!isSlowed)
            StartCoroutine("SlowEnemy", modifier);
    }

    IEnumerator SlowEnemy(float modifier)
    {
        speedMod = modifier;
        isSlowed = true;
        yield return new WaitForSeconds(5);
        speedMod = 1f;
        isSlowed = false;
    }
}
