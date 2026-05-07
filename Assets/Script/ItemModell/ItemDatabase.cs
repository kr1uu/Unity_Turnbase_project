using UnityEngine;
using SQLite4Unity3d;
using System.IO;
using System.Linq;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;

    public ItemVisualDatabase visualDB;

    private SQLiteConnection db;

    void Awake()
    {
        Instance = this;

        string dbPath = Path.Combine(
            Application.streamingAssetsPath,
            "Datagame.db"
        );

        Debug.Log("ITEM DB PATH: " + dbPath);

        db = new SQLiteConnection(
            dbPath,
            SQLiteOpenFlags.ReadWrite
        );

        var allItems =
            db.Table<ItemEntity>().ToList();

        Debug.Log(
            "ITEM COUNT = " +
            allItems.Count
        );

        foreach (var i in allItems)
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