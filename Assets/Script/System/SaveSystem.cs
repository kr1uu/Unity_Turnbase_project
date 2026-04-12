using System.IO;
using UnityEngine;

public static class SaveSystem
{
    static string path = Application.persistentDataPath + "/save.json";

    public static void Save(GameData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);

        Debug.Log("? SAVED: " + json);
        Debug.Log("Save path: " + Application.persistentDataPath);
    }

    public static GameData Load()
    {
        if (!File.Exists(path))
        {
            Debug.LogWarning("? No save file");
            return null;
        }

        string json = File.ReadAllText(path);
        Debug.Log("?? LOADED: " + json);

        return JsonUtility.FromJson<GameData>(json);
    }
}