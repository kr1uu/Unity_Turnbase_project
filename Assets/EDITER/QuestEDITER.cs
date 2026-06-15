using UnityEditor;
using UnityEngine;
using SQLite4Unity3d;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class QuestEditor : EditorWindow
{
    private string dbPath;

    private List<QuestData> quests = new();

    private List<CharacterData> characters = new();

    private List<ItemEntity> items = new();

    private List<NPCData> npcs = new();

    private Vector2 leftScroll;
    private Vector2 centerScroll;
    private Vector2 rightScroll;

    private QuestData selectedQuest;

    // =====================================================
    // OPEN
    // =====================================================

    [MenuItem("Tools/RPG/Quest Editor")]
    public static void Open()
    {
        GetWindow<QuestEditor>("Quest Editor");
    }

    // =====================================================
    // ENABLE
    // =====================================================

    private void OnEnable()
    {
        dbPath =
            Path.Combine(
                Application.streamingAssetsPath,
                "Datagame.db"
            );

        LoadData();
    }

    // =====================================================
    // LOAD
    // =====================================================

    void LoadData()
    {
        using (
            var conn =
            new SQLiteConnection(
                dbPath,
                SQLiteOpenFlags.ReadWrite
            )
        )
        {
            quests =
                conn.Table<QuestData>()
                .OrderBy(x => x.id)
                .ToList();

            characters =
                 conn.Table<CharacterData>()
                 .ToList();

            items =
                conn.Table<ItemEntity>()
                .ToList();

            npcs =
                conn.Table<NPCData>()
                .ToList();
        }

        Repaint();

    }
    public enum QuestType
    {
        Kill,
        Collect,
        TalktoNPC,
        Openchest,
        Boss
    }
    // =====================================================
    // GUI
    // =====================================================

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();

        DrawQuestList();

        DrawQuestInfo();

        DrawRewardPanel();

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        DrawBottomButtons();
    }

    // =====================================================
    // QUEST LIST
    // =====================================================

    void DrawQuestList()
    {
        EditorGUILayout.BeginVertical(
            GUILayout.Width(300)
        );

        GUILayout.Label(
            "Quest List",
            EditorStyles.boldLabel
        );

        leftScroll =
            EditorGUILayout.BeginScrollView(
                leftScroll
            );
        foreach (var q in quests)
        {
            bool isSelected =
                selectedQuest != null &&
                selectedQuest.id == q.id;

            Color oldColor = GUI.backgroundColor;

            if (isSelected)
                GUI.backgroundColor = new Color(0.4f, 0.6f, 1f);

            if (GUILayout.Button($"{q.id} - {q.quest_name}"))
            {
                if (!isSelected)
                {
                    selectedQuest = CloneQuest(q);
                    GUI.FocusControl(null);
                }
            }

            GUI.backgroundColor = oldColor;
        }

        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);

        if (
            GUILayout.Button(
                "Add Quest",
                GUILayout.Height(30)
            )
        )
        {
            CreateNewQuest();
        }

        if (
            selectedQuest != null &&
            GUILayout.Button(
                "Delete Quest",
                GUILayout.Height(30)
            )
        )
        {
            DeleteQuest(selectedQuest.id);
        }

        EditorGUILayout.EndVertical();
    }

    // =====================================================
    // INFO PANEL
    // =====================================================

    void DrawQuestInfo()
    {
        EditorGUILayout.BeginVertical();

        GUILayout.Label(
            "Quest Info",
            EditorStyles.boldLabel
        );

        if (selectedQuest == null)
        {
            EditorGUILayout.HelpBox(
                "Select Quest",
                MessageType.Info
            );

            EditorGUILayout.EndVertical();
            return;
        }

        centerScroll =
            EditorGUILayout.BeginScrollView(
                centerScroll
            );

        selectedQuest.id =
            EditorGUILayout.IntField(
                "ID",
                selectedQuest.id
            );

        selectedQuest.quest_name =
            EditorGUILayout.TextField(
                "Quest Name",
                selectedQuest.quest_name
            );

        GUILayout.Label("Description");

        selectedQuest.description =
            EditorGUILayout.TextArea(
                selectedQuest.description,
                GUILayout.Height(100)
            );
        QuestType type =
            (QuestType)System.Enum.Parse(
                typeof(QuestType),
                selectedQuest.quest_type
            );

        type =
            (QuestType)
            EditorGUILayout.EnumPopup(
                "Quest Type",
                type
            );

        selectedQuest.quest_type =
            type.ToString();

        DrawTargetSelector(type);   

        selectedQuest.required_amount =
            EditorGUILayout.IntField(
                "Required Amount",
                selectedQuest.required_amount
            );

        selectedQuest.is_main_quest =
            EditorGUILayout.Toggle(
                "Main Quest",
                selectedQuest.is_main_quest
            );

        selectedQuest.story_flag_on_complete =
            EditorGUILayout.IntField(
                "Story Flag",
                selectedQuest.story_flag_on_complete
            );

        EditorGUILayout.EndScrollView();

        EditorGUILayout.EndVertical();
    }
    // =====================================================
    // DROPDOWNS TARGETID
    // =====================================================
    void DrawTargetSelector(
    QuestType type
)
    {
        switch (type)
        {
            case QuestType.Kill:
            case QuestType.Boss:
                DrawEnemyDropdown();
                break;

            case QuestType.Collect:
                DrawItemDropdown();
                break;

            case QuestType.TalktoNPC:
                DrawNPCDropdown();
                break;

            case QuestType.Openchest:
                selectedQuest.target_id =
                    EditorGUILayout.IntField(
                        "Chest ID",
                        selectedQuest.target_id
                    );
                break;
        }
    }
    void DrawEnemyDropdown()
    {
        string[] options =
            characters
            .Select(
                c =>
                $"[{c.id}] {c.name}"
            )
            .ToArray();

        int currentIndex =
            characters.FindIndex(
                c =>
                c.id ==
                selectedQuest.target_id
            );

        if (currentIndex < 0)
            currentIndex = 0;

        int newIndex =
            EditorGUILayout.Popup(
                "Target Enemy",
                currentIndex,
                options
            );
        if (EditorGUI.EndChangeCheck())
        {
            //Debug.Log($"Popup Changed: {currentIndex} -> {newIndex}");

            selectedQuest.target_id = characters[newIndex].id;

            //Debug.Log($"target_id = {selectedQuest.target_id}");
        }
    }
    void DrawItemDropdown()
    {
        string[] options =
            items
            .Select(
                i =>
                $"[{i.id}] {i.name}"
            )
            .ToArray();

        int currentIndex =
            items.FindIndex(
                i =>
                i.id ==
                selectedQuest.target_id
            );

        if (currentIndex < 0)
            currentIndex = 0;

        int newIndex =
            EditorGUILayout.Popup(
                "Target Item",
                currentIndex,
                options
            );

        selectedQuest.target_id =
            items[newIndex].id;
    }
    void DrawNPCDropdown()
    {
        string[] options =
            npcs
            .Select(
                n =>
                $"[{n.id}] {n.npc_name}"
            )
            .ToArray();

        int currentIndex =
            npcs.FindIndex(
                n =>
                n.id ==
                selectedQuest.target_id
            );

        if (currentIndex < 0)
            currentIndex = 0;

        int newIndex =
            EditorGUILayout.Popup(
                "Target NPC",
                currentIndex,
                options
            );

        selectedQuest.target_id =
            npcs[newIndex].id;
    }
    // =====================================================
    // REWARD PANEL
    // =====================================================

    void DrawRewardPanel()
    {
        EditorGUILayout.BeginVertical(
            GUILayout.Width(350)
        );

        GUILayout.Label(
            "Reward / Chain",
            EditorStyles.boldLabel
        );

        if (selectedQuest == null)
        {
            EditorGUILayout.EndVertical();
            return;
        }

        rightScroll =
            EditorGUILayout.BeginScrollView(
                rightScroll
            );

        GUILayout.Label(
            "Rewards",
            EditorStyles.boldLabel
        );

        selectedQuest.reward_gold =
            EditorGUILayout.IntField(
                "Gold",
                selectedQuest.reward_gold
            );

        selectedQuest.reward_exp =
            EditorGUILayout.IntField(
                "EXP",
                selectedQuest.reward_exp
            );

        selectedQuest.reward_item_id =
            EditorGUILayout.IntField(
                "Reward Item ID",
                selectedQuest.reward_item_id
            );

        selectedQuest.reward_item_amount =
            EditorGUILayout.IntField(
                "Item Amount",
                selectedQuest.reward_item_amount
            );

        GUILayout.Space(20);

        GUILayout.Label(
            "Quest Chain",
            EditorStyles.boldLabel
        );

        selectedQuest.next_quest_ids =
            EditorGUILayout.TextField(
                "Next Quest IDs",
                selectedQuest.next_quest_ids
            );

        EditorGUILayout.HelpBox(
            "Example: 2,3,5",
            MessageType.None
        );

        EditorGUILayout.EndScrollView();

        EditorGUILayout.EndVertical();
    }

    // =====================================================
    // BUTTONS
    // =====================================================

    void DrawBottomButtons()
    {
        EditorGUILayout.BeginHorizontal();

        if (
            GUILayout.Button(
                "Save Quest",
                GUILayout.Height(35)
            )
        )
        {
            SaveQuest();
        }

        if (
            GUILayout.Button(
                "Reload",
                GUILayout.Height(35)
            )
        )
        {
            LoadData();
        }

        EditorGUILayout.EndHorizontal();
    }

    // =====================================================
    // SAVE
    // =====================================================

    void SaveQuest()
    {
        if (selectedQuest == null)
            return;

        using (
            var conn =
            new SQLiteConnection(
                dbPath,
                SQLiteOpenFlags.ReadWrite
            )
        )
        {
            conn.InsertOrReplace(
                selectedQuest
            );
        }

        LoadData();

        Debug.Log(
            "Quest Saved"
        );
    }

    // =====================================================
    // ADD
    // =====================================================

    void CreateNewQuest()
    {
        int nextID = 1;

        if (quests.Count > 0)
        {
            nextID =
                quests.Max(x => x.id) + 1;
        }

        selectedQuest =
            new QuestData()
            {
                id = nextID,
                quest_name = "New Quest",
                description = "",
                quest_type = "Kill"
            };
    }

    // =====================================================
    // DELETE
    // =====================================================

    void DeleteQuest(int id)
    {
        if (
            !EditorUtility.DisplayDialog(
                "Delete Quest",
                $"Delete Quest {id} ?",
                "Delete",
                "Cancel"
            )
        )
            return;

        using (
            var conn =
            new SQLiteConnection(
                dbPath,
                SQLiteOpenFlags.ReadWrite
            )
        )
        {
            conn.Execute(
                "DELETE FROM Quests WHERE id=?",
                id
            );
        }

        selectedQuest = null;

        LoadData();
    }

    // =====================================================
    // CLONE
    // =====================================================

    QuestData CloneQuest(
        QuestData q
    )
    {
        return new QuestData
        {
            id = q.id,
            quest_name = q.quest_name,
            description = q.description,
            quest_type = q.quest_type,
            target_id = q.target_id,
            required_amount = q.required_amount,
            reward_gold = q.reward_gold,
            reward_exp = q.reward_exp,
            reward_item_id = q.reward_item_id,
            reward_item_amount = q.reward_item_amount,
            next_quest_ids = q.next_quest_ids,
            is_main_quest = q.is_main_quest,
            story_flag_on_complete =
                q.story_flag_on_complete
        };
    }
}