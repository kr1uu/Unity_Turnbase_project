using System.IO;
using UnityEngine;

public static class SaveSystem
{
    // =====================================================
    // SAVE
    // =====================================================

    public static void Save(
        GameData data,
        int slotID
    )
    {
        string json =
            JsonUtility.ToJson(
                data,
                true
            );

        string path =
            Application.persistentDataPath +
            "/save_" +
            slotID +
            ".json";

        File.WriteAllText(
            path,
            json
        );

        Debug.Log(
            "SAVE PATH = " + path
        );
    }

    // =====================================================
    // LOAD
    // =====================================================

    public static GameData Load(int slotID)
    {
        string path =
            Application.persistentDataPath +
            "/save_" +
            slotID +
            ".json";

        if (!File.Exists(path))
        {
            Debug.LogWarning(
                "SAVE NOT FOUND"
            );

            return null;
        }

        string json =
            File.ReadAllText(path);

        return JsonUtility.FromJson<GameData>(
            json
        );
    }

    // =====================================================
    // CHECK SLOT
    // =====================================================

    public static bool HasSave(
        int slotID
    )
    {
        string path =
            Application.persistentDataPath +
            "/save_" +
            slotID +
            ".json";

        return File.Exists(path);
    }
}