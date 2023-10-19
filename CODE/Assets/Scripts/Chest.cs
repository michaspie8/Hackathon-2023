using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chest : Interactable
{
    [Header("Components")]
    public Animator anim;
    [Space]
    public Item[] items;
    void Start()
    {
        
        anim = GetComponent<Animator>();



    }
    new void Update()
    {
        base.Update();
        


        if (!gameObject.activeInHierarchy && Vector3.Distance(PlayerController.instance.transform.position, gameObject.transform.position) < 100 && !GetComponent<Renderer>().isVisible)
        {
            gameObject.SetActive(true);
        }
    }





    public override void Interact()
    {
        base.Interact();
        anim.SetTrigger("Open");
        StartCoroutine(OpenCo());

    }

    public IEnumerator OpenCo()
    {

        print("Chest opened");
        foreach (Item i in items)
        {
            
            Inventory.instance.Add(i, Inventory.instance.items);
        }
        yield return new WaitForSeconds(15f);
        gameObject.SetActive(false);
    }
}
