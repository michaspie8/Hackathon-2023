
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

/* This object manages the inventory UI. */

public class InventoryUI : MonoBehaviour
{

    public GameObject inventoryUI;  // The entire UI
    public Transform itemsParent;   // The parent object of all the items
    public bool isButtonDown;

    Inventory inventory;    // Our current inventory
    public InventorySlot[] slots;
    public List<Item> ItemList;

    public Transform EquipmentParent;
    public InventorySlot[] EquipmentSlots;
    public List<Item> EquipmentList;

    public Transform AssignableItemsParent;
    public InventorySlot[] AssignableItemsSlots;
    public List<Item> AssignableItemsList;

    public AssignableItem[] allAssignableItemsScrObj;

    public ToggleGroup inventoryToggleGroup;
    public ToggleGroup AssignableItemsToggleGroup;

    private void Awake()
    {
        
    }
    /*void setTogleGroup(ToggleGroup group)
    {
        foreach (var toggle in itemsParent.gameObject.GetComponentsInChildren<Toggle>(true))
        {
            toggle.group = group;
        }
    }*/
    public void assignItem(int index)
    {

        if (!inventoryToggleGroup.AnyTogglesOn()) return;
        Toggle toggle = inventoryToggleGroup.GetFirstActiveToggle();
        var slot = toggle.GetComponent<InventorySlot>();
        if (slot != null && slot.item != null)
        {
            var item = slot.item;
            foreach (var i in AssignableItemsManager.instance.currentItems)
            {
                if (i != null && i.name == item.name) return;

            }

            //if (item.GetType().IsSubclassOf(typeof(AssignableItem)) || item.GetType() == typeof(AssignableItem))
            //{
            AssignableItemsManager.instance.Unequip(index);
            if (item != null)
                ((AssignableItem)item).Equip(index);
            //}
        }

    }
    public void unassignItem()
    {
        if (!AssignableItemsToggleGroup.AnyTogglesOn()) return;
        Toggle toggle = AssignableItemsToggleGroup.GetFirstActiveToggle();

        if (toggle != null && toggle.TryGetComponent(out InventorySlot slot))
        {
            if (slot.item != null)
                AssignableItemsManager.instance.Unequip(((AssignableItem)slot.item).slotIndex);
        }
    }

    void Start()
    {
        allAssignableItemsScrObj = ExtensionMethods.GetAllInstances<AssignableItem>();
        inventory = Inventory.instance;
        inventory.onItemChangedCallback += UpdateUI;
    }

    // Check to see if we should open/close the inventory
    void Update()
    {
        if (isButtonDown)
        {
            if (inventoryUI.activeSelf)
            {
                Debug.Log("Closeing Inventory");
                GameManager.instance.ResumeGame();
                GameManager.instance.EnablePlayerControls();
                GameManager.instance.DisableUIControls();

            }
            else
            {
                Debug.Log("Opening Inventory");
                GameManager.instance.EnableUIControls();
                GameManager.instance.DisablePlayerControls();
                GameManager.instance.PauseGame();
            }

            inventoryUI.SetActive(!inventoryUI.activeSelf);
            isButtonDown = false;
            UpdateUI();
        }
    }

    // Update the inventory UI by:
    //		- Adding items
    //		- Clearing empty slots
    // This is called using a delegate on the Inventory.
    public void UpdateUI()
    {
        slots = itemsParent.GetComponentsInChildren<InventorySlot>();
        EquipmentSlots = EquipmentParent.GetComponentsInChildren<InventorySlot>();
        AssignableItemsSlots = AssignableItemsParent.GetComponentsInChildren<InventorySlot>();
        updateTypeOfItem(slots, inventory.items);
        updateTypeOfItem(EquipmentSlots, inventory.equipmentItems);

        for (int i = 0; i < AssignableItemsSlots.Length; i++)
        {

            if (AssignableItemsManager.instance.currentItems[i])
            {
                AssignableItemsSlots[i].AddItem(new(AssignableItemsManager.instance.currentItems), i);
            }
            else
            {
                AssignableItemsSlots[i].ClearSlot();
            }
        }
        void updateTypeOfItem(InventorySlot[] slots, List<Item> InventoryItems)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (i < InventoryItems.Count)
                {
                    if (InventoryItems[i])
                        slots[i].AddItem(InventoryItems, i);
                }
                else
                {
                    slots[i].ClearSlot();
                }
            }
        }

        {

        }
    }

}