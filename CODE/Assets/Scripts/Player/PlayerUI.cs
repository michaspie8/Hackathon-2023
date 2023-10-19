using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    #region Singleton
    public static PlayerUI instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of PlayerUI found!");
            return;
        }
        instance = this;
    }
    #endregion

    

    [Header("UI elements")]
    public GameObject PlayerNameText;

    [Header("Iteract")]
    //Object that displays in the UI the name of the object with which the player can interact
    public GameObject interactObject;
    public Sprite interactImage_Current;
    public Sprite interactImage_Keyboard;
    public Sprite interactImage_Xbox;
    public Sprite interactImage_Ps;
    // Start is called before the first frame update
    void Start()
    {
        PlayerNameText.GetComponent<TMP_Text>().text = SaveManager.saveData.PlayerName;
        interactImage_Current = interactImage_Keyboard;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    public void DisplayInteractImage(string name)
    {
        
        interactObject.GetComponentInChildren<TMP_Text>().text = name;
        interactObject.GetComponentInChildren<Image>().sprite = interactImage_Current;
        interactObject.SetActive(true);
    }
    public void EndDisplayInteractImage()
    {
        interactObject.SetActive(false);
    }
}
