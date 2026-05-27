using SQLite4Unity3d;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;

    public ItemVisualDatabase visualDB;

    private SQLiteConnection db;

    public List<ItemEntity> items = new List<ItemEntity>();

    void Awake()
    {
        Instance = this;

        string dbPath = Path.Combine(
            Application.streamingAssetsPath,
            "Datagame.db"
        );

        db = new SQLiteConnection(
            dbPath,
            SQLiteOpenFlags.ReadWrite
        );

        items =
            db.Table<ItemEntity>()
            .ToList();

        Debug.Log(
            "ITEM COUNT = " +
            items.Count
        );

        foreach (var i in items)
        {
            Debug.Log(
                $"ITEM: {i.id} {i.name}"
            );
        }
    }
    public ItemEntity GetItem(int id)
    {
        return db.Table<ItemEntity>()
            .FirstOrDefault(i => i.id == id);
    }

    public Sprite GetIcon(int id)
    {
        return visualDB.GetIcon(id);
    }
}