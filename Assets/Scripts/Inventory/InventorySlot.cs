using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/* Sits on all InventorySlots. */

public class InventorySlot : MonoBehaviour
{

    public List<Item> ItemsList;
    public Image icon;
    //public Button removeButton;

    public Item item;  // Current item in the slot

    // Add item to the slot
    public void AddItem(List<Item> ItemList, int index)
    {
        item = ItemList[index];
        ItemsList = ItemList;
        icon.sprite = item.icon;
        icon.enabled = true;
        //removeButton.interactable = true;
    }

    // Clear the slot
    public void ClearSlot()
    {
        item = null;

        icon.sprite = null;
        icon.enabled = false;
        //removeButton.interactable = false;
    }

    // If the remove button is pressed, this function will be called.
    public void RemoveItemFromInventory()
    {
        if (ItemsList == Inventory.instance.items)
        {

            Inventory.instance.Remove(item, Inventory.instance.items);
        }
        else
        {
            if (ItemsList == new List<Item>(AssignableItemsManager.instance.currentItems))
            {
                AssignableItemsManager.instance.Remove(((AssignableItem)item).slotIndex, (AssignableItem)item);
            }
            else
                if (ItemsList == new List<Item>(Inventory.instance.equipmentItems))
            {
            }
        }
    }

    // UseInInventory the item
    public void UseItem()
    {
        if (item != null)
        {
            item.UseInInventory();
        }
    }

}