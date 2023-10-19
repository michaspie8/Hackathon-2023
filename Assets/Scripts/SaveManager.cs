using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class SaveData
{
    public int SaveSlot = 0;
    public string PlayerName = "PLAYER";
    public int PlayerLevel = 0;
    public int PlayerCircle = 0; // A number of the circle the player is in. 1 Circle =  levels

}
public class SaveManager
{
    public static SaveData saveData = new();
    public string dirPath;
    public string fileName = "save";
    public string fileExt = ".json";
    public int actualSlot;
    public SaveManager(string dirPath)
    {
        this.dirPath = dirPath;
    }
    public SaveData LoadGame(int slot)
    {
        string fullPath = Path.Combine(dirPath, fileName + "-" + slot + fileExt);

        SaveData loadedData = null;
        if (File.Exists(fullPath))
        {

            try
            {
                string dataToLoad = "";
                using (FileStream stream = new(fullPath, FileMode.Open))
                {

                    using (StreamReader reader = new StreamReader(stream))

                    {

                        dataToLoad = reader.ReadToEnd();

                    }
                }

                loadedData = JsonUtility.FromJson<SaveData>(dataToLoad);
                actualSlot = slot;
            }
            catch (Exception e)

            {
                actualSlot = -1;
                Debug.LogError("Error occured when trying to load data from file" + fullPath + "\n" + e);

            }
        }
        return loadedData;
    }
    public void SaveGame(int slot)
    {
        saveData.PlayerName = GameManager.instance.PlayerName;
        WriteToFile(saveData, slot);

    }
    public void WriteToFile(SaveData data, int slot )
    {
        string fullPath = Path.Combine(dirPath, fileName + "-" + slot + fileExt);
        try
        {

            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            var file = JsonUtility.ToJson(data, true);
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(file);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Cant save: " + e.Message);
            throw;
        }
    }
}
