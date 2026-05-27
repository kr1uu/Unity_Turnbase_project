using UnityEditor;
using UnityEngine;
using SQLite4Unity3d;
using System.Collections.Generic;
using System.Linq;
using System.IO;

[CustomEditor(typeof(NPCInteract))]
public class NPCInteractEditor : Editor
{
    private class NPCOption
    {
        public int id;
        public string name;
        public string type;

        public int shopID;

        public string shopName;
        public string shopType;

        public int shopTier;
    }

    private List<NPCOption> npcOptions =
        new();

    private bool loaded = false;

    public override void OnInspectorGUI()
    {
        NPCInteract npcInteract =
            (NPCInteract)target;

        DrawDefaultInspector();

        EditorGUILayout.Space();

        // =========================================
        // LOAD BUTTON
        // =========================================

        if (!loaded)
        {
            if (GUILayout.Button("Load NPC Database"))
            {
                LoadNPCs();
            }

            return;
        }

        // =========================================
        // FIND CURRENT NPC
        // =========================================

        int currentIndex =
            npcOptions.FindIndex(
                x => x.id == npcInteract.npcID
            );

        if (currentIndex < 0)
            currentIndex = 0;

        // =========================================
        // DROPDOWN
        // =========================================

        string[] displayOptions =
            npcOptions
            .Select(
                x =>
                $"[{x.id}] {x.name} ({x.type})"
            )
            .ToArray();

        EditorGUILayout.LabelField(
            "NPC Selector",
            EditorStyles.boldLabel
        );

        int newIndex =
            EditorGUILayout.Popup(
                "Selected NPC",
                currentIndex,
                displayOptions
            );

        // =========================================
        // APPLY
        // =========================================

        if (newIndex >= 0 &&
            newIndex < npcOptions.Count)
        {
            npcInteract.npcID =
                npcOptions[newIndex].id;
        }

        // =========================================
        // PREVIEW
        // =========================================

        var selectedNPC =
            npcOptions[newIndex];

        EditorGUILayout.Space();

        EditorGUILayout.LabelField(
            "NPC Information",
            EditorStyles.boldLabel
        );

        EditorGUILayout.HelpBox(
            $"Name : {selectedNPC.name}\n" +
            $"ID : {selectedNPC.id}\n" +
            $"Type : {selectedNPC.type}",
            MessageType.Info
        );

        // =========================================
        // SHOP PREVIEW
        // =========================================

        if (selectedNPC.type == "Blacksmith")
        {
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(
                "Shop Information",
                EditorStyles.boldLabel
            );

            EditorGUILayout.HelpBox(
                $"Shop Name : {selectedNPC.shopName}\n" +
                $"Shop Type : {selectedNPC.shopType}\n" +
                $"Shop Tier : {selectedNPC.shopTier}",
                MessageType.None
            );
        }

        EditorUtility.SetDirty(npcInteract);
    }

    private void LoadNPCs()
    {
        try
        {
            string dbPath =
                Path.Combine(
                    Application.streamingAssetsPath,
                    "Datagame.db"
                );

            using var db =
                new SQLiteConnection(
                    dbPath,
                    SQLiteOpenFlags.ReadOnly
                );

            var npcs =
                db.Table<NPCData>()
                .ToList();

            var shops =
                db.Table<ShopData>()
                .ToList();

            npcOptions =
                npcs.Select(
                    npc =>
                    {
                        ShopData shop =
                            shops.FirstOrDefault(
                                s => s.id ==
                                npc.shop_id
                            );

                        return new NPCOption
                        {
                            id = npc.id,
                            name = npc.npc_name,
                            type = npc.npc_type,

                            shopID = npc.shop_id,

                            shopName =
                                shop != null
                                ? shop.shop_name
                                : "None",

                            shopType =
                                shop != null
                                ? shop.shop_type
                                : "None",

                            shopTier =
                                shop != null
                                ? shop.shop_tier
                                : 0
                        };
                    }
                )
                .OrderBy(x => x.name)
                .ToList();

            loaded = true;

            Debug.Log(
                $"Loaded {npcOptions.Count} NPCs"
            );
        }
        catch (System.Exception ex)
        {
            Debug.LogError(
                "FAILED TO LOAD NPC DB : " +
                ex.Message
            );
        }
    }
}