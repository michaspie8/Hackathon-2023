
using UnityEngine;

public class AbsoluteManager : MonoBehaviour
{
    #region Singleton
    public static AbsoluteManager instance;
    
    
    private void Awake()
    {

        if (instance != null)
        {
            Debug.LogWarning("More than one instance of AbsoluteManager found!");
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion


    public static string pathToSaveData;

    
    public string PlayerName = "PLAYER";
    public int PlayerLevel = 0; // 1-6
    public int PlayerCircle = 0; // A number of the circle the player is in. 1 Circle = 6 levels
    public AudioClip maintheme;
    public SaveManager saveManager;
    // Start is called before the first frame update
    void Start()
    {
        var cam = FindAnyObjectByType<Camera>().transform;
        AudioSource.PlayClipAtPoint(maintheme, cam.position);
        
        saveManager = new SaveManager(Application.persistentDataPath);
        
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
    public void StartNewGame(int slot)
    {

        saveManager.SaveGame(slot);
        //Play opening cutscene and then load the first level

    }
    // Update is called once per frame
    void Update()
    {

    }
}
