using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsManager : MonoBehaviour
{

    #region Singleton
    public static WeaponsManager instance;

    void Awake()
    {
        instance = this;
    }

    #endregion

    //public WeaponType weaponType;
    public GameObject[] actualWeaponObjects;
    [Header("BodyParents")]
    public Transform[] parentList;
    public Transform rightHandParent;
    public Transform leftHandParent;
    private Animator anim;
    
    private void Start()
    {
        actualWeaponObjects = new GameObject[2];
        parentList = new Transform[] { rightHandParent, leftHandParent };
        anim = PlayerController.instance.GetComponent<Animator>();
    }

    public void Equip(Weapon weapon, WeaponType type)
    {
        /*anim.ResetTrigger("UnEquip");
        anim.SetTrigger("EquipOrUse");*/
        switch (type)
        {
            case WeaponType.melee:
                break;
            case WeaponType.ranged:
                break;
            case WeaponType.shield:
                break;
            case WeaponType.other:
                break;
        }
        /*     if (AssignableItemsManager.instance.currentItems[weapon.slotIndex] != null)
             {
                 Unequip(weapon.slotIndex);
             }*/
        actualWeaponObjects[weapon.slotIndex] = Instantiate(weapon.prefab, parentList[(int)weapon.parent]);
        Debug.Log("Equpie");
        //weaponType = type;
    }
    public void Unequip(int slotIndex)
    {

        /*anim.SetTrigger("UnEquip");
        anim.ResetTrigger("EquipOrUse");*/
        
        anim.SetBool("MeleeWeaponEquipped", false);
        if (actualWeaponObjects[slotIndex] != null)
            Destroy(actualWeaponObjects[slotIndex]);
        
    }
    /*public void OnItemChanged(AssignableItem oldItem, AssignableItem newItem)
    {
        if (newItem == null)
        {
            Unequip(oldItem.slotIndex);
        }
    }*/
}
public enum WeaponType { melee, ranged, shield, other };
public enum WeaponParent { rightHand, leftHand };