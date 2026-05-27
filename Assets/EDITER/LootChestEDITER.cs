using UnityEditor;
using UnityEngine;
using SQLite4Unity3d;
using System.IO;
using System.Linq;
using System.Collections.Generic;

[CustomEditor(typeof(LootChest))]
public class LootChestEditor : Editor
{
    private class RarityGroup
    {
        public string rarity;
        public float chance = 0f;
        public bool foldout = true;
        public List<ItemEntity> items = new List<ItemEntity>();
    }

    private List<RarityGroup> groups = new List<RarityGroup>();
    private bool loaded = false;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LootChest chest = (LootChest)target;

        EditorGUILayout.Space();

        if (!loaded)
        {
            if (GUILayout.Button("Load Items By Rarity"))
            {
                LoadItemsGrouped();
            }
            return;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("RARITY DROP TABLE", EditorStyles.boldLabel);

        float total = groups.Sum(g => g.chance);

        foreach (var group in groups)
        {
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.LabelField(
                $"Rarity {group.rarity} ({group.items.Count} items)",
                EditorStyles.boldLabel
            );

            group.chance = EditorGUILayout.Slider(
                "Drop Chance %",
                group.chance,
                0f,
                100f
            );

            group.foldout = EditorGUILayout.Foldout(
                group.foldout,
                "Show Items"
            );

            if (group.foldout)
            {
                foreach (var item in group.items)
                {
                    EditorGUILayout.LabelField($"• [{item.id}] {item.name}");
                }
            }

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space();

        EditorGUILayout.HelpBox(
            $"Total Chance = {total:0.0}%",
            total > 100f ? MessageType.Warning : MessageType.Info
        );

        if (GUILayout.Button("Normalize To 100%"))
        {
            NormalizeTo100();
        }

        if (GUILayout.Button("Apply To LootTable"))
        {
            ApplyToLootTable(chest);
        }
    }

    void LoadItemsGrouped()
    {
        string dbPath = Path.Combine(
            Application.streamingAssetsPath,
            "Datagame.db"
        );

        using var db = new SQLiteConnection(
            dbPath,
            SQLiteOpenFlags.ReadOnly
        );

        var items = db.Table<ItemEntity>().ToList();

        groups = items
            .GroupBy(i => i.rarity)
            .Select(g => new RarityGroup
            {
                rarity = g.Key,
                chance = 0f,
                items = g.OrderBy(x => x.name).ToList()
            })
            .OrderBy(g => g.rarity)
            .ToList();

        loaded = true;

        Debug.Log($"Loaded {items.Count} items in {groups.Count} rarity groups");
    }

    void NormalizeTo100()
    {
        float total = groups.Sum(g => g.chance);

        if (total <= 0f) return;

        foreach (var g in groups)
        {
            g.chance = (g.chance / total) * 100f;
        }
    }

    void ApplyToLootTable(LootChest chest)
    {
        chest.lootTable = new List<LootEntry>();

        foreach (var group in groups)
        {
            if (group.chance <= 0f)
                continue;

            float perItemChance = group.chance / group.items.Count;

            foreach (var item in group.items)
            {
                chest.lootTable.Add(new LootEntry
                {
                    itemID = item.id,
                    dropChance = perItemChance,
                    minAmount = 1,
                    maxAmount = 1
                });
            }
        }

        EditorUtility.SetDirty(chest);

        Debug.Log($"Generated {chest.lootTable.Count} loot entries");
    }
}