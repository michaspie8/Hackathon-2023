using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
        Menus = new GameObject[] { MainMenu, OptionsMenu, AuthorsMenu, SaveMenu };
        GoToMenu(MainMenu);

    }
    public void OnStartButton(int index)
    {
        GameManager.instance.saveManager.LoadGame(index);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
                       var data = GameManager.instance.saveManager.LoadGame(i);
                       //Display data about save slot
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
