using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[CreateAssetMenu(fileName = "New Melee Weapon", menuName = "Inventory/AssignableItem/Melee Weapon")]
public class MeleeWeapon : Weapon
{
    private ref PlayerController.States playerState => ref PlayerController.instance.GetComponent<PlayerController>().actualState;
    public int dashForce;
    public bool isAtacking;
    public override void Equip()
    {
        base.Equip();
        WeaponsManager.instance.actualWeaponObjects[slotIndex].GetComponent<Collider>().enabled = false;

        //anim.ResetTrigger("MeleeWeaponSlash");
        
    }
    public override void Unequip()
    {
        base.Unequip();
    }
    public override void UseInGame()
    {
        base.UseInGame();

        if (isAtacking == false)
        {
            anim.SetTrigger("RightHandSlice");
            WeaponsManager.instance.StartCoroutine(SlashCo());
        }
    }
    public IEnumerator SlashCo()
    {
        if (anim.IsInTransition(0))
        {

            var transinfo = anim.GetAnimatorTransitionInfo(0);
            if (transinfo.IsName("melee-attack-1 -> melee-attack-2") || transinfo.IsName("melee-attack-2 -> melee-attack-3"))
            {
                //before
                playerState = PlayerController.States.atacking;
                WeaponsManager.instance.actualWeaponObjects[slotIndex].GetComponent<Collider>().enabled = true;
                PlayerController.instance.GetComponent<PlayerController>().rb.AddForce(PlayerController.instance.transform.forward * dashForce, ForceMode.Impulse);
                yield return new WaitUntil(() => anim.IsInTransition(0));
                //after
                isAtacking = false;
                playerState = PlayerController.States.free;
                WeaponsManager.instance.actualWeaponObjects[slotIndex].GetComponent<Collider>().enabled = false;
            }


            //WeaponsManager.instance.actualWeaponObjects[slotIndex].GetComponent<Collider>().enabled = true;
            //PlayerController.instance.GetComponent<PlayerController>().actualState = PlayerController.States.free;
            //WeaponsManager.instance.actualWeaponObjects[slotIndex].GetComponent<Collider>().enabled = false;

            yield return null;


        }
        else
        {
            //on attack
            isAtacking = true;
            anim.SetTrigger("RightHandSlice");
            playerState = PlayerController.States.atacking;
            WeaponsManager.instance.actualWeaponObjects[slotIndex].GetComponent<Collider>().enabled = true;
            PlayerController.instance.GetComponent<PlayerController>().rb.AddForce(PlayerController.instance.transform.forward * dashForce, ForceMode.Impulse);
            yield return new WaitUntil(() => anim.IsInTransition(0));
            //after attack
            isAtacking = false;
            playerState = PlayerController.States.free;
            WeaponsManager.instance.actualWeaponObjects[slotIndex].GetComponent<Collider>().enabled = false;

        }
    }


}