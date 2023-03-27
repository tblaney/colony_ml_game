using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;


public static class SaveSystem 
{
    private static readonly string SAVE_FOLDER = Application.persistentDataPath + "/Saves/";
    private const string SAVE_EXTENSION = "txt";
    
    public static int _index = 0;
    public static List<int> _indices;


    public static void Initialize()
    {
        _indices = new List<int>();
        // test if save folder exists
        if (!Directory.Exists(SAVE_FOLDER))
        {
            // create save folder
            Directory.CreateDirectory(SAVE_FOLDER);
        }

        for (int i = 1; i < 4; i++)
        {
            if (SaveExists(i))
            {
                _indices.Add(i);
            }
        }
    }
    public static void SetIndex(int index)
    {
        _index = index;
    }
    public static void Save(SaveData saveData)
    {
        int idx = _index;
        // unsecure:
        if (File.Exists(SAVE_FOLDER + "save_" + idx + "." + SAVE_EXTENSION))
        {
            DeleteSave(idx);
        }
        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(SAVE_FOLDER + "save_" + idx + "." + SAVE_EXTENSION, json);
    }
    public static void DeleteSave(int _index)
    {
        File.Delete(SAVE_FOLDER + "save_" + _index + "." + SAVE_EXTENSION);
        File.Delete(SAVE_FOLDER + "saveSecure_" + _index + "." + SAVE_EXTENSION);
    }
    public static SaveData Load(int _index)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(SAVE_FOLDER);

        FileInfo[] saveFile = directoryInfo.GetFiles("save_" + _index + "." + SAVE_EXTENSION);

        string saveString = File.ReadAllText(saveFile[0].FullName);

        return JsonUtility.FromJson<SaveData>(saveString);
    }
    public static int GetOpenIndex()
    {
        for (int i = 1; i < 4; i++)
        {
            if (!SaveExists(i))
            {
                return i;
            }
        }

        return 0;
    }
    public static bool SaveExists(int saveIndex)
    {
        if (File.Exists(SAVE_FOLDER + "save_" + saveIndex + "." + SAVE_EXTENSION))
        {
            return true;
        } else
        {
            return false;
        }
    }
}

[Serializable]
public class SaveData
{
    public Habitation _habitation;
    public List<Node> _nodes;
    public List<ItemInventory> _inventories;
}
