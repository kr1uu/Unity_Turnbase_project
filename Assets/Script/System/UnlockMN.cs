using UnityEngine;
using SQLite4Unity3d;
using System.IO;
using System.Linq;

public class UnlockManager : MonoBehaviour
{
    public static UnlockManager Instance;

    SQLiteConnection db;

    private void Awake()
    {
        Instance = this;

        string dbPath =
            Path.Combine(
                Application.streamingAssetsPath,
                "Datagame.db"
            );

        db = new SQLiteConnection(
            dbPath,
            SQLiteOpenFlags.ReadOnly
        );
    }

    public void CheckUnlocks()
    {
        int level =
            PlayerProgression.Instance
            .player.level;

        var unlocks =
            db.Table<CharacterUnlockData>()
            .ToList();

        foreach (var unlock in unlocks)
        {
            if (level >= unlock.unlock_level)
            {
                if (
                    !PlayerProgression.Instance
                    .player.unlockedCharacters
                    .Contains(
                        unlock.character_id
                    )
                )
                {
                    PlayerProgression.Instance
                        .player.unlockedCharacters
                        .Add(
                            unlock.character_id
                        );

                    Debug.Log(
                        "Unlocked Character: "
                        + unlock.character_id
                    );
                }
            }
        }
    }
}