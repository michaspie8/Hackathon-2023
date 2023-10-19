using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : Weapon
{
    Controls controls;

    Rigidbody _arrowPrefab;
    Transform _arrowParent;
    Rigidbody _bowPrefab;
    Transform _bowParent;
    public int arrowSpeedMultipler;

    public override void UseInGame()
    {
        base.UseInGame();
        anim.SetTrigger("fire");
    }

    public void Fire()
    {
        Rigidbody shell = Instantiate(_arrowPrefab, _arrowParent.position, PlayerController.instance.transform.rotation);
        shell.AddForce(shell.transform.forward * arrowSpeedMultipler);
    }
}
