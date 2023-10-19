using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : AssignableItem
{
    public Animator anim => PlayerController.instance.GetComponent<Animator>();
    public int damageBaseValue;
    public Stat damage;
    //public GameObject weapon;
    public GameObject prefab;
    public WeaponParent parent;
    public WeaponType type;

    
    public virtual void Equip()
    {
        if(damage == null) damage = new(damageBaseValue);

        WeaponsManager.instance.Equip(this, type);

    }
    
    public override void Unequip()
    {
        WeaponsManager.instance.Unequip(slotIndex);
        base.Unequip();
    }
    public override void UseInInventory()
    {
        Equip();
        base.UseInInventory();
        
    }
    public override void UseInGame()
    {
        base.UseInGame();

    }
}



