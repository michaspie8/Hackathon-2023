using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Melee Weapon", menuName = "Inventory/AssignableItem/Melee Weapon")]
public class MeleeWeapon : Weapon
{

    public override void Equip()
    {
        base.Equip();
        //anim.ResetTrigger("MeleeWeaponSlash");
    }
    public override void Unequip()
    {
        base.Unequip();
    }
    public override void UseInGame()
    {
        base.UseInGame();
        anim.SetTrigger("MeleeWeaponSlash");
        anim.SetBool("MeleeWeaponSlashB",true);
        PlayerController.instance.GetComponent<PlayerController>().actualState = PlayerController.States.atacking;
        WeaponsManager.instance.actualWeaponObjects[slotIndex].GetComponent<Collider>().enabled = false;
        WeaponsManager.instance.StartCoroutine(SlashCo());
    }
    public IEnumerator SlashCo()
    {

        yield return new WaitUntil (()=> anim.GetCurrentAnimatorStateInfo(0).IsName("SwordIdle"));
        anim.SetBool("MeleeWeaponSlashB", false);
        PlayerController.instance.GetComponent<PlayerController>().actualState = PlayerController.States.free;
        yield return null;
    }
}


