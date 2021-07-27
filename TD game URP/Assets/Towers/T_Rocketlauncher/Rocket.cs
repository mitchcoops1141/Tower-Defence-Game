using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public float speed = 50f;

    GameObject explosion;

    public Transform spawnExplosionLocation;

    public SphereCollider hitbox;

    bool readyToFly = false;
    GameObject target = null;

    public void StartBullet(GameObject enemy, GameObject ex)
    {
        target = enemy;
        readyToFly = true;
        explosion = ex;
        //StartCoroutine("DestroyHitbox");
    }

    IEnumerator DestroyHitbox()
    {
        yield return new WaitForSeconds(0.2f);

        hitbox.enabled = false;
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
            //spawn explosion
            GameObject cacheObj = Instantiate(explosion, spawnExplosionLocation);
            cacheObj.transform.parent = null;
            cacheObj.GetComponent<RocketExplosion>().StartExplosion(GetComponentInParent<Tower>());
            Destroy(cacheObj, 0.5f);

            Destroy(gameObject);
        }
    }
}
