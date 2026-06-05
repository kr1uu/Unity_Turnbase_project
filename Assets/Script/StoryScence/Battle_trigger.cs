using System.Collections.Generic;
using UnityEngine;

public class BattleTrigger : MonoBehaviour
{
    [Header("Encounter Identity")]
    public string encounterID;
    [Header("Chunk")]
    public string chunkID;

    public enum EnemyRank { Normal, Elite, Boss }

    [Header("Enemy IDs chose from Editor")]
    public List<int> selectedEnemyIDs = new List<int>();

    [Header("Rank foreach enemy ID")]
    [SerializeField]
    public List<EnemyRankEntry> enemyRanks = new List<EnemyRankEntry>();

    [System.Serializable]
    public class EnemyRankEntry
    {
        public int enemyID;
        public EnemyRank rank;
        [Range(1, 20)]
        public int level = 1;
    }

    public EnemyRank GetRank(int enemyID)
    {
        var entry = enemyRanks.Find(e => e.enemyID == enemyID);
        return entry != null ? entry.rank : EnemyRank.Normal;

    }
    private void OnEnable()
    {
        if (EncounterRegistry.Instance != null)
        {
            EncounterRegistry.Instance
                .Register(this);
        }
    }

    private void OnDisable()
    {
        if (EncounterRegistry.Instance != null)
        {
            EncounterRegistry.Instance
                .Unregister(this);
        }
    }
    private void Start()
    {
        Debug.Log(
        $"[CHECK] chunk='{chunkID}' encounter='{encounterID}' defeated=" +
        EncounterStateManager.Instance.IsDefeated(chunkID, encounterID)
        );
        bool defeated = EncounterStateManager.Instance != null &&
                        EncounterStateManager.Instance.IsDefeated(chunkID, encounterID);

        if (defeated)
            gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameStateManager.Instance.isLoadingGame)
            return;
        if (!collision.CompareTag("Player")) return;

        Debug.Log("[BattleTrigger] encounterID = " + encounterID);

        BattleEncounterData.Instance.LastEncounterID = encounterID;

        if (BattleEncounterData.Instance == null)
        {
            Debug.LogError("[BattleTrigger] EncounterData ain't going");
            return;
        }
        BattleEncounterData.Instance.LastEncounterID = encounterID;

        Vector3 safePos = collision.transform.position -
                          (collision.transform.up * 0.5f);

        PlayerPosition.Instance.returnPosition = safePos;

            PlayerPosition.Instance.returnChunkID = chunkID;

        // setup enemy
        BattleEncounterData.Instance.SetEnemies(selectedEnemyIDs, enemyRanks);

        SceneFader.Instance.FadeToScene("BattleScene");
    }

}
