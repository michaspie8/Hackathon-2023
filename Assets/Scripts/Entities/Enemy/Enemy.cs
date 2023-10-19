using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    public EnemyScriptableObject scriptableObject;
    public Stat Damage;
    public Stat Armor;
    public Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        name = scriptableObject.name;
        health = scriptableObject.health;
        Armor = new(scriptableObject.Armor);
        Damage = new(scriptableObject.Damage);

    }
    public virtual void DealDamage()
    {
        PlayerController.instance.GetComponent<PlayerController>().DecreaseHealth(Damage.GetValue());
    }
    public virtual void DealDamage(int val)
    {
        PlayerController.instance.GetComponent<PlayerController>().DecreaseHealth(val);
    }
    public virtual void TakeDamage()
    {
        DecreaseHealth(PlayerController.instance.GetComponent<PlayerController>().Damage.GetValue());
    }
    public virtual void TakeDamage(int val)
    {
        DecreaseHealth(val);
    }
    private void OnCollisionEnter(Collision other)
    {
        
        if (other.collider.isTrigger && other.gameObject.TryGetComponent<Weapon>(out Weapon weapon))
        {
            if (weapon.type == WeaponType.melee)
                TakeDamage(weapon.damage.GetValue());
        }
    }
    // Update is called once per frame
    void Update()
    {

    }

    public override void Die()
    {
        Debug.Log("Enemy" + name + "died");
        Destroy(gameObject);
    }
}
