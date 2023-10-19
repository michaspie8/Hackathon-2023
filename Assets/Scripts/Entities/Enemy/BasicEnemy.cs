using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : Enemy
{
    public float autoDamageDelay = 0.3f;
    bool doAutoDamage = false;
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.body == PlayerController.instance.GetComponent<Rigidbody>())
        {
            doAutoDamage = true;
            StartCoroutine(StillDamageCo(Damage.GetValue(), autoDamageDelay));
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.body == PlayerController.instance.GetComponent<Rigidbody>())
            doAutoDamage = false;

    }
    public override void Die()
    {
        Debug.Log("BasicEnemy " + name + " died");
        Destroy(gameObject);
    }
    IEnumerator StillDamageCo(int val, float delay)
    {
        do
        {
            DealDamage(val);
            yield return new WaitForSeconds(delay);
        } while (doAutoDamage);

    }
}
