/*using UnityEngine;

public class ItemPickup : Interactable
{

	public Item item;   // Item to put in the inventory on pickup

    public void Start()
    {
		interactionTrigger = true;
    }
    // When the player interacts with the item
    public override void Interact()
	{
		base.Interact();

		PickUp();   // Pick it up!
	}

	// Pick up the item
	void PickUp()
	{
		Debug.Log("Picking up " + item.name);
		bool wasPickedUp = Inventory.instance.Add(item, Inventory.instance.items);    // Add to inventory

		// If successfully picked up
		if (wasPickedUp)
			StartCoroutine(DestroyWhenInteractionResetted());
	}
}*/