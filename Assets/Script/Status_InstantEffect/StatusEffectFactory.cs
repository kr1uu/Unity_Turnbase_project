using SQLite4Unity3d;
using UnityEngine;
using System.IO;
using System.Linq;

public static class StatusEffectFactory
{
    public static StatusEffect Create(
        int effectID,
        BattleUnit source
    )
    {
        string dbPath =
            Path.Combine(
                Application.streamingAssetsPath,
                "Datagame.db"
            );

        var db =
            new SQLiteConnection(
                dbPath,
                SQLiteOpenFlags.ReadWrite
            );

        // ===== DATABASE =====

        var data =
            db.Table<StatusEffectData>()
            .FirstOrDefault(x => x.id == effectID);

        if (data == null)
        {
            Debug.LogWarning(
                "No StatusEffect id="
                + effectID
            );

            return null;
        }

        // ===== MAKE RUNTIME EFFECT =====

        return new StatusEffect(
         data.effect_type,
         data.power,
         data.duration,
         source
         );
    }
}