using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignableItemsManager : MonoBehaviour
{
    #region Singleton

    public Controls controls;

    //function(userAge, x){
    //
    //console,log("wiek...");
    //}
    //
    //
    public Equipment[] defaultAssignableItems;
    public static AssignableItemsManager instance;

    void Awake()
    {
        instance = this;
    }

    #endregion

    public AssignableItem[] currentItems;   // Items we currently have equipped

    // Callback for when an item is equipped/unequipped
    public delegate void OnItemChanged(AssignableItem newItem, AssignableItem oldItem);
    public OnItemChanged onItemChanged;


    Inventory inventory;    // Reference to our inventory

    void Start()
    {
        inventory = Inventory.instance;     // Get a reference to our inventory
        currentItems = new AssignableItem[3];
        onItemChanged += (oldItem, newItem) => { inventory.onItemChangedCallback.Invoke(); };
    }

    // EquipOrUse a new item
    public void Equip(AssignableItem newItem, int slotIndex)
    {
        AssignableItem oldItem = Unequip(slotIndex);

        // An item has been equipped so we trigger the callback
        if (onItemChanged != null)
        {
            onItemChanged.Invoke(newItem, oldItem);
        }

        // Insert the item into the slot
        currentItems[slotIndex] = newItem;
    }

    // Unequip an item with a particular index
    public AssignableItem Unequip(int slotIndex)
    {
        AssignableItem oldItem = null;
        // Only do this if an item is there
        if (currentItems[slotIndex] != null)
        {
            // Add the item to the inventory
            oldItem = currentItems[slotIndex];
            inventory.Add(oldItem, inventory.items);


            Remove(slotIndex, oldItem);
        }
        return oldItem;
    }
    public void Remove(int slotIndex, AssignableItem RemovedItem)
    {
        RemovedItem.Unequip();
        // Remove the item from the equipment array
        currentItems[slotIndex] = null;

        // Equipment has been removed so we trigger the callback
        if (onItemChanged != null)
        {
            onItemChanged.Invoke(null, RemovedItem);
        }
    }
    void Update()
    {

    }
}