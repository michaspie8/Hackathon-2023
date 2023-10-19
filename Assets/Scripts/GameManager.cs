using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor.ShaderGraph;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem.HID;
using UnityEngine.InputSystem.Utilities;

public class GameManager : MonoBehaviour
{

    #region Singleton
    public static GameManager instance;
    private void Awake()
    {

        if (instance != null)
        {
            Debug.LogWarning("More than one instance of GameManager found!");
            return;
        }
        instance = this;
        controls = new Controls();
        playerInput = GetComponent<PlayerInput>();
        DontDestroyOnLoad(gameObject);

        //Controls for InventoryUI
        var InventoryUI = mainCanvas.GetComponent<InventoryUI>();
        controls.Player.OpenInventory.performed += ctx => InventoryUI.isButtonDown = ctx.ReadValueAsButton();
        controls.UI.CloseInventory.performed += ctx => InventoryUI.isButtonDown = ctx.ReadValueAsButton();
        controls.UI.AssignItem1.performed += ctx => InventoryUI.assignItem(0);
        controls.UI.AssignItem2.performed += ctx => InventoryUI.assignItem(1);
        controls.UI.AssignItem3.performed += ctx => InventoryUI.assignItem(2);
        controls.UI.Back.performed += ctx => InventoryUI.unassignItem();
    }
    #endregion
    public Controls controls;
    public PlayerInput playerInput;
    [Space]
    public static string pathToSaveData;

    public GameObject mainCanvas;

    public Interactable nearestInteractable;
    public float nearestInteractableDistance;
    public bool isInteracting;
    public string PlayerName;
    public int PlayerLevel; // 1-6
    public int PlayerCircle; // A number of the circle the player is in. 1 Circle = 6 levels

    public SaveManager saveManager;

    public Camera mainCamera;
    // Start is called before the first frame update

    void Start()
    {

        Screen.SetResolution(300, 720, FullScreenMode.ExclusiveFullScreen);
        //TODO save manager, nearest interactable decetion, player name, player level, player circle, camera
        //saveManager = new(Application.persistentDataPath, "save");
        saveManager = new SaveManager(Application.persistentDataPath);
        controls.UI.Disable();
        controls.Player.Enable();


        //Handle input device change and set the correct interact image. TODO set all sprites at once, not only interact image
        InputSystem.onDeviceChange += (device, change) =>
        {
            if (change == InputDeviceChange.Added || change == InputDeviceChange.Enabled)
            {
                Debug.Log("Device added: " + device);
            }
            else if (change == InputDeviceChange.Removed)
            {
                Debug.Log("Device removed: " + device);
            }
        };
        InputSystem.onEvent.Call( a =>
        {
            var device = InputSystem.GetDeviceById(a.deviceId);
            if (device == Gamepad.current)
            {


                switch (Gamepad.current)
                {
                    case DualShockGamepad or DualSenseGamepadHID:
                        PlayerUI.instance.interactImage_Current = PlayerUI.instance.interactImage_Ps;
                        break;
                    case XInputController or XInputControllerWindows:
                        PlayerUI.instance.interactImage_Current = PlayerUI.instance.interactImage_Xbox;
                        break;
                    case Gamepad:
                        PlayerUI.instance.interactImage_Current = PlayerUI.instance.interactImage_Xbox;
                        break;
                    default:

                        break;

                }
            }
            else if (device == Keyboard.current)
            {
                PlayerUI.instance.interactImage_Current = PlayerUI.instance.interactImage_Keyboard;
            }
            else
            {
                /*PlayerUI.instance.interactImage_Current = PlayerUI.instance.interactImage_Keyboard;*/
            }
        });
    }


    // Update is called once per frame
    void Update()
    {
    }

    public void LoadSave(int slot)
    {
        saveManager.LoadGame(slot);
        if (SaveManager.saveData != null)
        {
            PlayerName = SaveManager.saveData.PlayerName;
            PlayerLevel = SaveManager.saveData.PlayerLevel;
            PlayerCircle = SaveManager.saveData.PlayerCircle;
        }
        else
        {
            StartNewGame(slot);
        }
    }



    public void DisablePlayerControls()
    {
        controls.Player.Disable();
    }
    public void EnablePlayerControls()
    {
        controls.Player.Enable();
    }
    public void DisableUIControls()
    {
        controls.UI.Disable();
    }
    public void EnableUIControls()
    {
        controls.UI.Enable();
    }
    public void PauseGame()
    {
        Time.timeScale = 0;

    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
    }
    public void EndGame()
    {

    }
    public static T CopyScriptableObject<T>(T t) where T : ScriptableObject
    {
        return Instantiate(t);
    }
    public void StartNewGame(int slot)
    {
        saveManager.SaveGame(slot);
        //Play opening cutscene and then load the first level

    }
}
