using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public string Name;
    public float health;
    public float HealthValue { get { return health; } }
    public void IncreaseHealth(float value)
    {
        health += value;
    }
    public void DecreaseHealth(float value)
    {
        if (health - value < 0)
        {
            health = 0;
            Die();
        }
        else
            health -= value;
    }
    public virtual void Die()
    {
        Debug.Log("Entity " + this.gameObject.name + " died.");
    }
    private void Start()
    {
        if (name == null)
        {
            name = gameObject.name;
        }
    }


}

