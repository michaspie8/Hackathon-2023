using UnityEngine;

/* The base item class. All items should derive from this. */

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{

	new public string name = "New Item";    // Name of the item
	public Sprite icon = null;              // Item icon
	public bool showInInventory = true;
	public string description;

	// Called when the item is pressed in the inventory
	public virtual void UseInInventory()
	{
		// UseInInventory the item
		// Something may happen
		Debug.Log("Item: "+name+" is used in Invenotry");
	}

	// Call this method to remove the item from inventory
	public void RemoveFromInventory()
	{
		Inventory.instance.Remove(this, Inventory.instance.items);
	}

}