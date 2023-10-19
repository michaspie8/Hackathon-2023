using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerController>() != null)
        {
            AbsoluteManager.instance.PlayerLevel++;
            AbsoluteManager.instance.PlayerCircle = AbsoluteManager.instance.PlayerLevel / 6;
            AbsoluteManager.instance.PlayerLevel %= 6;
            AbsoluteManager.instance.saveManager.SaveGame(AbsoluteManager.instance.saveManager.actualSlot);
            gameObject.transform.root.GetComponentInChildren<RoomGenerator>().GenerateRoom();
        }
    }
}
