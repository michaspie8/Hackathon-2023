using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* An Item that can be equipped. */

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
public class Equipment : Item
{



	public EquipmentSlot equipSlot; // Slot to store equipment in

	public int armorModifier;       // Increase/decrease in armor
	public int damageModifier;      // Increase/decrease in damage

	// When pressed in inventory
	public override void UseInInventory()
	{
		base.UseInInventory();
		EquipmentManager.instance.Equip(this);  // EquipOrUse it
		RemoveFromInventory();                  // Remove it from inventory
	}

}

public enum EquipmentSlot { Armor, Item, Item2, Item3  }