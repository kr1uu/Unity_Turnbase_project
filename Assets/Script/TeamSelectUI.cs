using SQLite4Unity3d;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TeamSelectUI : MonoBehaviour
{
    public Transform content;
    public GameObject slotPrefab;

    SQLiteConnection db;

    void Start()
    {
        LoadCharacters();
        Debug.Log("TeamSelectUI START");
    }

    void LoadCharacters()
    {
        Debug.Log("LOAD CHARACTER CALLED FROM: " + gameObject.name);

        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        string dbPath = System.IO.Path.Combine(Application.streamingAssetsPath, "Datagame.db");
        db = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadOnly);

        // player faction o
        var characters = db.Query<CharacterData>(
            "SELECT * FROM Characters WHERE faction_id = 1"
        );
        var unlocked =
        PlayerProgression.Instance
            .player
            .unlockedCharacters;

        Debug.Log("Loaded characters: " + characters.Count);

        foreach (var c in characters)
        {
            if (!unlocked.Contains(c.id))
                continue;

            GameObject go = Instantiate(slotPrefab, content);

            var ui = go.GetComponent<CharacterSlotUI>();
            if (ui == null)
            {
                Debug.LogError("CharacterSlotUI missing on prefab!");
                return;
            }

            ui.Setup(c);
        }
    }
    void OnEnable()
    {
        Debug.Log("TeamSelectUI ENABLED");
    }

}
