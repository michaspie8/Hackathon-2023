using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Potion", menuName = "Inventory/AssignableItem/Potion")]
public class Potion : AssignableItem
{
    public int health;
    public override void UseInGame()
    {
        base.UseInGame();
        Debug.Log("Recovered "+health+" hp using potion");
        Unequip();
        Inventory.instance.Remove(this, Inventory.instance.assignableItems);
    }
}
