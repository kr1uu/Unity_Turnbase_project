using SQLite4Unity3d;
using System.IO;
using UnityEditor.U2D.Animation;
using UnityEngine;

public class DataManager
{
    private SQLiteConnection db;

    public DataManager(string dbName)
    {
        string dbPath = Path.Combine(Application.streamingAssetsPath, dbName);
        db = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
    }

    public CharacterData GetCharacter(string name)
    {
        return db.Table<CharacterData>().Where(c => c.name == name).FirstOrDefault();
    }
}