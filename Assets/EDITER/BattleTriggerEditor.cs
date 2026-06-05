using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using SQLite4Unity3d;
using System.IO;

[CustomEditor(typeof(BattleTrigger))]
public class BattleTriggerEditor : Editor
{
    private class EnemyOption
    {
        public int id;
        public string name;
        public bool selected;
        public BattleTrigger.EnemyRank rank;
        public int baseHP;
        public int baseATK;
        public int level = 1 ;

        // AI
        public int aiProfileId;
        public int originalAIProfileId;
    }

    private List<EnemyOption> options = new();
    private bool loaded = false;

    // ===== AI PRESET =====
    private readonly int[] aiProfileIds = {1, 2, 3, 4, 5, 6, 7, 8 };
    private readonly string[] aiProfileNames =
    {
        "Aggressive",
        "Debuff",
        "Balance",
        "Assasin",
        "Shielder",
        "Captain",
        "Oldgaurd",
        "Nemesis"
    };

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var trigger = (BattleTrigger)target;

        EditorGUILayout.Space();

        if (!loaded)
        {
            if (GUILayout.Button("Take Information from Database"))
            {
                LoadEnemiesFromDB();

                foreach (var opt in options)
                {
                    opt.selected = trigger.selectedEnemyIDs.Contains(opt.id);

                    var rankEntry = trigger.enemyRanks.Find(e => e.enemyID == opt.id);
                    if (rankEntry != null)
                    {
                        opt.rank = rankEntry.rank;
                        opt.level = rankEntry.level;
                    }
                    opt.rank = rankEntry != null
                        ? rankEntry.rank
                        : BattleTrigger.EnemyRank.Normal;
                }
            }
        }

        EditorGUILayout.LabelField("Enemies", EditorStyles.boldLabel);

        foreach (var opt in options)
        {
            EditorGUILayout.BeginHorizontal();

            opt.selected = EditorGUILayout.Toggle(opt.selected, GUILayout.Width(20));
            EditorGUILayout.LabelField($"[{opt.id}] {opt.name}", GUILayout.ExpandWidth(true));

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Rank", GUILayout.Width(40));

            opt.rank = (BattleTrigger.EnemyRank)
                EditorGUILayout.EnumPopup(
                    opt.rank,
                    GUILayout.Width(100));

            EditorGUILayout.LabelField("AI", GUILayout.Width(20));

            int currentIndex =
                System.Array.IndexOf(
                    aiProfileIds,
                    opt.aiProfileId);

            int newIndex =
                EditorGUILayout.Popup(
                    currentIndex < 0 ? 0 : currentIndex,
                    aiProfileNames);

            opt.aiProfileId =
                aiProfileIds[newIndex];

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(
                $"Level {opt.level}",
                GUILayout.Width(70));

            opt.level =
                EditorGUILayout.IntSlider(
                    opt.level,
                    1,
                    20);

            EditorGUILayout.EndHorizontal();


            GetPreviewStats(opt.baseHP, opt.baseATK, opt.rank, opt.level, out var hp, out var atk);
            EditorGUILayout.LabelField(
                $" Base HP: {opt.baseHP} to {hp} HP , Base ATK: {opt.baseATK} to {atk} ATK",
                EditorStyles.miniLabel
            );

            EditorGUILayout.Space();
        }

        if (GUILayout.Button("Open Encounter Editor"))
        {
            EncounterEditorWindow.Open((BattleTrigger)target);
        }
        EditorGUILayout.Space();

        if (GUILayout.Button("Update List into BattleTrigger"))
        {
            // Update BattleTrigger
            trigger.selectedEnemyIDs = options
                .Where(o => o.selected)
                .Select(o => o.id)
                .ToList();

            trigger.enemyRanks = options
                .Where(o => o.selected)
                .Select(o => new BattleTrigger.EnemyRankEntry
                {
                    enemyID = o.id,
                    rank = o.rank,
                    level = o.level
                })
                .ToList();

            // Update AI back to DB
            UpdateAIProfileToDatabase();

            // Sync global encounter data
            if (BattleEncounterData.Instance != null)
            {
                BattleEncounterData.Instance.SetEnemies(
                    trigger.selectedEnemyIDs,
                    trigger.enemyRanks
                );
            }

            EditorUtility.SetDirty(trigger);
            Debug.Log($"[Editor] had chosen {trigger.selectedEnemyIDs.Count} enemy cho encounter.");
        }
    }

    private void LoadEnemiesFromDB()
    {
        try
        {
            string dbPath = Path.Combine(Application.streamingAssetsPath, "Datagame.db");
            using var db = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadOnly);

            var enemies = db.Table<CharacterData>()
                            .Where(c => c.faction_id == 2)
                            .ToList();

            options = enemies.Select(e => new EnemyOption
            {
                id = e.id,
                name = e.name,
                selected = false,
                baseHP = e.hp,
                baseATK = e.atk,
                aiProfileId = e.ai_profile_id,
                originalAIProfileId = e.ai_profile_id
            })
            .OrderBy(o => o.name)
            .ToList();

            loaded = true;
            Debug.Log($"Loaded {options.Count} enemies from Characters.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Cant Read the DB: " + ex.Message);
        }
    }

    private void UpdateAIProfileToDatabase()
    {
        string dbPath = Path.Combine(Application.streamingAssetsPath, "Datagame.db");
        using var db = new SQLiteConnection(dbPath);

        foreach (var opt in options)
        {
            if (opt.aiProfileId != opt.originalAIProfileId)
            {
                db.Execute(
                    "UPDATE Characters SET ai_profile_id = ? WHERE id = ?",
                    opt.aiProfileId,
                    opt.id
                );

                opt.originalAIProfileId = opt.aiProfileId;

                Debug.Log(
                    $"[DB] Enemy [{opt.id}] {opt.name} AI updated ? {opt.aiProfileId}"
                );
            }
        }
    }

    private static void GetPreviewStats(
        int baseHP,
        int baseATK,
        BattleTrigger.EnemyRank rank,
        int level,
        out int hp,
        out int atk)
    {
        hp = baseHP;
        atk = baseATK;

        if (rank == BattleTrigger.EnemyRank.Elite)
        {
            hp = Mathf.RoundToInt(baseHP * 1.6f);
            atk = Mathf.RoundToInt(baseATK * 1.2f);
        }
        else if (rank == BattleTrigger.EnemyRank.Boss)
        {
            hp = Mathf.RoundToInt(baseHP * 2.2f);
            atk = Mathf.RoundToInt(baseATK * 1.5f);
        }
    }
}
