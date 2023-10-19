using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New AssignableItem", menuName = "Inventory/AssignableItem/AssignableItem")]
public class AssignableItem : Item
{
    [Tooltip("Zero-based index, do not change this value in editor")]
    public int slotIndex = -1;
    // When pressed in inventory
    
    
    public override void UseInInventory()
    {

        base.UseInInventory();
        AssignableItemsManager.instance.Equip(this, slotIndex);  // Equip it
        RemoveFromInventory();

    }
    public virtual void Equip(int index)
    {
        if (slotIndex == -1)
        {
            slotIndex = index;
            UseInInventory();
        }
    }
    public virtual void Unequip()
    {
        slotIndex = -1;
        /*AssignableItemsManager.instance.Unequip(slotIndex);*/
        /*
        Inventory.instance.Add(this, Inventory.instance.items);*/
    }
    public virtual void UseInGame()
    {
        Debug.Log("AsgnItem: " + name + " is in use");
    }
}
