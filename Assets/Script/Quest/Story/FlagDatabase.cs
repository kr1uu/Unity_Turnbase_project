using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SQLite4Unity3d;
using System.IO;

public class FlagDatabase : MonoBehaviour
{
    public static FlagDatabase Instance;

    public List<StoryFlagData> flags =
        new();

    private SQLiteConnection db;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);

            LoadFlags();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ====================================
    // LOAD
    // ====================================

    void LoadFlags()
    {
        string dbPath =
            Path.Combine(
                Application.streamingAssetsPath,
                "Datagame.db"
            );

        db =
            new SQLiteConnection(
                dbPath,
                SQLiteOpenFlags.ReadOnly
            );

        flags =
            db.Table<StoryFlagData>()
            .OrderBy(x => x.id)
            .ToList();

        Debug.Log(
            $"Loaded {flags.Count} Story Flags"
        );
    }

    // ====================================
    // GET BY ID
    // ====================================

    public StoryFlagData GetFlag(
        int id
    )
    {
        return flags.Find(
            x => x.id == id
        );
    }

    // ====================================
    // GET BY NAME
    // ====================================

    public StoryFlagData GetFlag(
        string flagName
    )
    {
        return flags.Find(
            x => x.flag_name ==
            flagName
        );
    }

    // ====================================
    // GET NAME
    // ====================================

    public string GetFlagName(
        int id
    )
    {
        StoryFlagData flag =
            GetFlag(id);

        return flag != null
            ? flag.flag_name
            : "";
    }
}