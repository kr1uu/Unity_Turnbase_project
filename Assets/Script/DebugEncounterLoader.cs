using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;
using System.Linq;

public class DebugEncounterLoader : MonoBehaviour
{
    [Header("Debug Players")]
    public List<int> playerIDs =
        new List<int>();

    [System.Serializable]
    public class DebugEnemy
    {
        public int enemyID;

        public BattleTrigger.EnemyRank rank =
            BattleTrigger.EnemyRank.Normal;
    }

    [Header("Debug Enemies")]
    public List<DebugEnemy> enemies =
        new List<DebugEnemy>();

    [Header("Optional")]
    public string debugEncounterID =
        "DEBUG_BATTLE";

    private void Awake()
    {
        CreateDebugParty();

        CreateDebugEncounter();
    }

    void CreateDebugParty()
    {
        if (PartyManager.Instance != null)
            return;

        Debug.Log(
            "[DebugEncounterLoader] Create Debug Party"
        );

        GameObject partyGO =
            new GameObject("DebugPartyManager");

        var party =
            partyGO.AddComponent<PartyManager>();

        DontDestroyOnLoad(partyGO);

        string dbPath =
            System.IO.Path.Combine(
                Application.streamingAssetsPath,
                "Datagame.db"
            );

        var db =
            new SQLiteConnection(
                dbPath,
                SQLiteOpenFlags.ReadWrite
            );

        foreach (var id in playerIDs)
        {
            var c = db.Table<CharacterData>()
                .FirstOrDefault(x => x.id == id);

            if (c != null)
            {
                CharacterStats stats =
                    new CharacterStats(
                        c.id,
                        c.faction_id,
                        c.name,
                        c.hp,
                        c.atk,
                        c.def,
                        c.spd,
                        c.baseLevel,
                        c.expReward,
                        c.goldReward,
                        c.ai_profile_id
                    );

                party.PartyStats.Add(stats);
            }
            else
            {
                Debug.LogWarning(
                    $"Player ID {id} not found"
                );
            }
        }
    }

    void CreateDebugEncounter()
    {
        if (BattleEncounterData.Instance != null)
        {
            Debug.Log(
                "[DebugEncounterLoader] EncounterData already exists"
            );

            return;
        }

        Debug.Log(
            "[DebugEncounterLoader] Create Debug Encounter"
        );

        GameObject encounterGO =
            new GameObject("DebugEncounterData");

        var encounter =
            encounterGO.AddComponent<BattleEncounterData>();

        DontDestroyOnLoad(encounterGO);

        List<int> ids =
            new List<int>();

        List<BattleTrigger.EnemyRankEntry> ranks =
            new List<BattleTrigger.EnemyRankEntry>();

        foreach (var e in enemies)
        {
            ids.Add(e.enemyID);

            ranks.Add(
                new BattleTrigger.EnemyRankEntry
                {
                    enemyID = e.enemyID,
                    rank = e.rank
                }
            );
        }

        encounter.LastEncounterID =
            debugEncounterID;

        encounter.SetEnemies(ids, ranks);
    }
}