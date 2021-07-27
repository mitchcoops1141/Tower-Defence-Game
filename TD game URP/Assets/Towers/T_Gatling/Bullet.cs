using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 5f;
    public ParticleSystem bullet;
    public GameObject hit;

    bool readyToFly = false;
    GameObject target = null;
    Tower tower;

    public void StartBullet(GameObject enemy, Tower t)
    {
        target = enemy;
        readyToFly = true;
        tower = t;
    }

    private void Start()
    {
        bullet.Play();
    }

    void Update()
    {
        if (readyToFly)
        {
            if (target != null)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
            }
                
            else
                Destroy(gameObject);
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == target)
        {
            //ENEMY HIT
            other.GetComponent<enemy_CS>().Hit(tower.damage, tower.slowMultiplier, tower.stunDuration);
            GameObject hitMark = Instantiate(hit);
            hitMark.transform.position = transform.position;
            Destroy(hitMark, 0.3f);
            Destroy(gameObject);
        }
    }
}
