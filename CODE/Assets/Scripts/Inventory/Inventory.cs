using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    #region Singleton

    public static Inventory instance;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        instance = this;
    }

    #endregion

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    public int space = 12;  // Amount of item spaces

    // Our current list of items in the inventory
    public List<Item> items = new();
    public List<Item> equipmentItems = new();
    public List<Item> assignableItems = new();

    public void Start()
    {



    }
    // Add a new item if enough room
    public bool Add(Item item, List<Item> ItemType)
    {
        if (item.showInInventory)
        {
            if (ItemType.Count >= space)
            {
                Debug.Log("Not enough room.");
                return false;
            }

            ItemType.Add(GameManager.CopyScriptableObject(item));

            if (onItemChangedCallback != null)
                onItemChangedCallback.Invoke();
        }
        return true;
    }

    // Remove an item
    public void Remove(Item item, List<Item> ItemType)
    {
        ItemType.Remove(item);

        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();
    }

    public List<Item> SerializeItems()
    {
        return items;
    }
    public virtual void AddItems()
    {

    }
}