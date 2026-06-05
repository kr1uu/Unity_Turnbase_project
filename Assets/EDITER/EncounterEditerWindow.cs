using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using SQLite4Unity3d;
using System.IO;

public class EncounterEditorWindow : EditorWindow
{
    private BattleTrigger trigger;

    private SQLiteConnection db;

    private Vector2 leftScroll;
    private Vector2 rightScroll;

    private List<EnemyEntry> enemies = new();

    private string searchText = "";

    public static void Open(BattleTrigger trigger)
    {
        var window =
            GetWindow<EncounterEditorWindow>(
                "Encounter Editor");

        window.trigger = trigger;

        window.LoadEnemies();

        window.Show();
    }

    private class EnemyEntry
    {
        public int id;
        public string name;

        public bool selected;

        public int baseHP;
        public int baseATK;

        public BattleTrigger.EnemyRank rank =
            BattleTrigger.EnemyRank.Normal;

        public int level = 1;

        public int aiProfileId;
    }
    private void LoadEnemies()
    {
        string dbPath =
            Path.Combine(
                Application.streamingAssetsPath,
                "Datagame.db");

        db = new SQLiteConnection(
            dbPath,
            SQLiteOpenFlags.ReadOnly);

        var chars =
            db.Table<CharacterData>()
            .Where(c => c.faction_id == 2)
            .ToList();

        enemies =
            chars.Select(c => new EnemyEntry
            {
                id = c.id,
                name = c.name,

                baseHP = c.hp,
                baseATK = c.atk,

                aiProfileId = c.ai_profile_id
            })
            .OrderBy(x => x.name)
            .ToList();

        SyncFromTrigger();
    }
    private readonly int[] aiProfileIds =
{
    1,2,3,4,5,6,7,8
};

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
    private void SyncFromTrigger()
    {
        if (trigger == null)
            return;

        foreach (var e in enemies)
        {
            e.selected =
                trigger.selectedEnemyIDs
                .Contains(e.id);

            var entry =
                trigger.enemyRanks
                .Find(x => x.enemyID == e.id);

            if (entry != null)
            {
                e.rank = entry.rank;
                e.level = entry.level;
            }
        }
    }
    private void OnGUI()
    {
        if (trigger == null)
        {
            EditorGUILayout.HelpBox(
                "No BattleTrigger selected",
                MessageType.Warning);

            return;
        }

        EditorGUILayout.LabelField(
            $"Encounter : {trigger.encounterID}",
            EditorStyles.boldLabel);

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        DrawEnemyList();

        DrawEnemyConfig();

        EditorGUILayout.EndHorizontal();

        DrawBottomButtons();
    }
    private void DrawEnemyList()
    {
        EditorGUILayout.BeginVertical(
            GUILayout.Width(250));

        searchText =
            EditorGUILayout.TextField(
                "Search",
                searchText);

        leftScroll =
            EditorGUILayout.BeginScrollView(
                leftScroll);

        foreach (var e in enemies)
        {
            if (!string.IsNullOrEmpty(searchText))
            {
                if (!e.name.ToLower()
                    .Contains(searchText.ToLower()))
                    continue;
            }

            e.selected =
                EditorGUILayout.ToggleLeft(
                    $"[{e.id}] {e.name}",
                    e.selected);
        }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.EndVertical();
    }
    private void DrawEnemyConfig()
    {
        EditorGUILayout.BeginVertical();

        rightScroll =
            EditorGUILayout.BeginScrollView(
                rightScroll);

        foreach (var e in enemies.Where(x => x.selected))
        {
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.LabelField(
                e.name,
                EditorStyles.boldLabel);

            e.rank =
                (BattleTrigger.EnemyRank)
                EditorGUILayout.EnumPopup(
                    "Rank",
                    e.rank);

            e.level =
                EditorGUILayout.IntSlider(
                    "Level",
                    e.level,
                    1,
                    20);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(
                "AI",
                GUILayout.Width(50));

            int currentIndex =
                System.Array.IndexOf(
                    aiProfileIds,
                    e.aiProfileId);

            int newIndex =
                EditorGUILayout.Popup(
                    currentIndex < 0 ? 0 : currentIndex,
                    aiProfileNames);

            e.aiProfileId =
                aiProfileIds[newIndex];

            EditorGUILayout.EndHorizontal();
            int hp;
            int atk;

            GetPreviewStats(
                e.baseHP,
                e.baseATK,
                e.rank,
                e.level,
                out hp,
                out atk);

            EditorGUILayout.HelpBox(
                $"HP : {hp}\nATK : {atk}",
                MessageType.None);

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.EndVertical();
    }
    private void GetPreviewStats(
    int baseHP,
    int baseATK,
    BattleTrigger.EnemyRank rank,
    int level,
    out int hp,
    out int atk)
    {
        float levelScale =
            1f + ((level - 1) * 0.1f);

        hp =
            Mathf.RoundToInt(
                baseHP * levelScale);

        atk =
            Mathf.RoundToInt(
                baseATK * levelScale);

        switch (rank)
        {
            case BattleTrigger.EnemyRank.Elite:

                hp =
                    Mathf.RoundToInt(
                        hp * 1.6f);

                atk =
                    Mathf.RoundToInt(
                        atk * 1.2f);

                break;

            case BattleTrigger.EnemyRank.Boss:

                hp =
                    Mathf.RoundToInt(
                        hp * 2.2f);

                atk =
                    Mathf.RoundToInt(
                        atk * 1.5f);

                break;
        }
    }
    private void DrawBottomButtons()
    {
        EditorGUILayout.Space();

        if (GUILayout.Button(
            "Apply To Encounter"))
        {
            trigger.selectedEnemyIDs =
                enemies
                .Where(x => x.selected)
                .Select(x => x.id)
                .ToList();

            trigger.enemyRanks =
                enemies
                .Where(x => x.selected)
                .Select(x =>
                    new BattleTrigger.EnemyRankEntry
                    {
                        enemyID = x.id,
                        rank = x.rank,
                        level = x.level
                    })
                .ToList();

            EditorUtility.SetDirty(trigger);

            Debug.Log(
                "[EncounterEditor] Saved");
        }
    }
}