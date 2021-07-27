using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public float ricochetRange;
    public float speed;
    public bool shouldRicochet = false;

    public List<GameObject> EnemiesInRicRange = new List<GameObject>();

    int ricochetAmount;
    int ricIndex = 0;

    bool readyToFly = false;
    GameObject target = null;
    Vector3 startPos;
    Tower tower;

    public void StartCannonBall(GameObject enemy, Tower t)
    {
        tower = t;
        transform.parent = null;
        startPos = transform.position;
        target = enemy;

        shouldRicochet = tower.shouldRicochet;
        ricochetAmount = tower.ricochetAmount;

        readyToFly = true;
    }

    private void Start()
    {

    }

    void Update()
    {
        if (readyToFly)
        {
            if (!shouldRicochet)
            {
                if (target != null)
                {
                    transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
                }
                else
                    Destroy(gameObject);
            }
            else
            {
                if (target != null)
                {
                    //get direction to target
                    Vector3 dir = target.transform.position - transform.position;

                    //move the bullet
                    transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);
                } 
                else if (ricIndex >= ricochetAmount)
                {
                    Destroy(gameObject);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == target)
        {
            //ENEMY HIT
            other.GetComponent<enemy_CS>().Hit(tower.damage, tower.slowMultiplier, tower.stunDuration);

            if (!shouldRicochet)
            {
                Destroy(gameObject);
            }
            else
            {
                if (EnemiesInRicRange.Count > 1)
                {
                    EnemiesInRicRange.Remove(target);
                    target = EnemiesInRicRange[Random.Range(0, EnemiesInRicRange.Count)];
                    ricIndex++;
                }   
                else
                {
                    Destroy(gameObject);
                }
            }
            
        }
    }
}
