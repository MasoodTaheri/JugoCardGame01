using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class Datamanager : MonoBehaviour
{

    public static Datamanager Instance;
    public AllPurchaseData data;
    public List<ImageData> imageDatas;

    private string file = "player.text";
    private string file1 = "player.image";

    private void Awake()
    {
        Instance = this;
        Load();
    }

    private void OnDestroy()
    {
        Save();
    }
    private void Start()
    {
        Debug.Log(Application.persistentDataPath);
    }


    public void UpdateAllPurchase(PlayerData newdata)
    {
        data.AllPurchedData.Add(newdata);
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(data);
        WriteToFile(file, json);
    }

    public void Load()
    {
        data = new AllPurchaseData();
        string json = ReadFromFile(file);
        JsonUtility.FromJsonOverwrite(json, data);
    }

    private void WriteToFile(string fileName, string json)
    {
        string path = GetFilePath(fileName);
        FileStream fileStream = new FileStream(path, FileMode.Create);

        using (StreamWriter writer = new StreamWriter(fileStream))
        {
            writer.Write(json);
        }
    }

    public string ReadFromFile(string fileName)
    {
        string path = GetFilePath(fileName);
        if (File.Exists(path))
        {
            Debug.Log(path);
            using (StreamReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
                return json;
            }
        }
        else
            Debug.LogWarning("File not found!");
        return "";
    }

    private string GetFilePath(string fileName)
    {
        return Application.persistentDataPath + "/" + fileName;

    }
    public Sprite FetchSprite(string iconName)
    {
        Sprite icon = null;
        foreach (var item in imageDatas)
        {
            if (item.IconName == iconName)
            {
                icon = item.iconSprite;
                break;
            }
        }
        return icon;
    }
}

[System.Serializable]
public class AllPurchaseData
{
    public List<PlayerData> AllPurchedData = new List<PlayerData>();
}

[System.Serializable]
public class PlayerData
{
    public string IconName = " ";
    public int Value;
}

[System.Serializable]
public class ImageData
{
    public string IconName = " ";
    public Sprite iconSprite;
}

