using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class StartupMenu : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject OptionsMenu;
    public GameObject AuthorsMenu;
    public GameObject SlotSelectMenu;
    public GameObject SaveMenu;
    private GameObject[] Menus;

    private void Start()
    {
        Menus = new GameObject[] { MainMenu, OptionsMenu, AuthorsMenu, SaveMenu, SlotSelectMenu };
        GoToMenu(MainMenu);

    }
    public void OnStartButton(int index)
    {
        if (index > 0)
        {
            AbsoluteManager.instance.saveManager.LoadGame(index);
            
            SceneManager.LoadScene("MainDungeon");
        }
    }
    public void GoToMenu(GameObject menuObject)
    {
        if (menuObject == SlotSelectMenu)
        {
            foreach (var slot in SlotSelectMenu.GetComponents<Button>())
            {
                for (int i = 1; i < 4; i++)
                {
                    if (slot.name == "Slot" + i)
                    {
                        var data = AbsoluteManager.instance.saveManager.LoadGame(i);
                        //Display data about save slot
                        if (data != null)
                        {
                            foreach (var obj in slot.GetComponentsInParent<TMP_Text>())
                            {
                                if (obj.name == "PlayerName")
                                {
                                    obj.text = data.PlayerName;
                                }
                                else if (obj.name == "Info")
                                {
                                    obj.text = data.PlayerLevel + "  " + data.date;
                                }
                            }
                        }
                        else
                        {
                            slot.GetComponentInChildren<TMP_Text>().text = "Empty";
                        }
                    }

                }
            }
        }
        foreach (var menu in Menus)
        {
            menu.SetActive(false);
        }
        menuObject.SetActive(true);

    }
    public void Quit()
    {
        Debug.Log("Quit!!!");
        Application.Quit();

    }
}
