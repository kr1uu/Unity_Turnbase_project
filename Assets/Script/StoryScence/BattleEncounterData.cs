using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleEncounterData : MonoBehaviour
{
    public static BattleEncounterData Instance;

    // EnemyList ID enemy chosen
    public List<int> SelectedEnemyIDs = new List<int>();

    // rank list foreach enemy ID
    public List<BattleTrigger.EnemyRankEntry> EnemyRanks = new List<BattleTrigger.EnemyRankEntry>();
    public string LastEncounterID;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // have data from BattleTriggerEditor
    public void SetEnemies(List<int> ids, List<BattleTrigger.EnemyRankEntry> ranks)
    {
        SelectedEnemyIDs = new List<int>(ids);

        if (ranks != null && ranks.Count > 0)
        {
            EnemyRanks = new List<BattleTrigger.EnemyRankEntry>(ranks);
        }
        else
        {
            EnemyRanks = ids.ConvertAll(id => new BattleTrigger.EnemyRankEntry
            {
                enemyID = id,
                rank = BattleTrigger.EnemyRank.Normal,
                level = 1
            });
        }

        Debug.Log("[EncounterData] List enemy encounter and rank: " +
                  string.Join(", ", EnemyRanks.Select(e => $"{e.enemyID}:{e.rank}")));
    }

    // set rank theo ID
    public BattleTrigger.EnemyRank GetRank(int enemyID)
    {
        var entry = EnemyRanks.Find(e => e.enemyID == enemyID);
        return entry != null ? entry.rank : BattleTrigger.EnemyRank.Normal;
    }
    public int GetLevel(int enemyID)
    {
        var entry = EnemyRanks.Find(e => e.enemyID == enemyID);
        return entry != null ? entry.level : 1;
    }
}