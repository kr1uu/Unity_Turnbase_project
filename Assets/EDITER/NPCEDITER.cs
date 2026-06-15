using UnityEditor;
using UnityEngine;
using SQLite4Unity3d;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class NPCEditor : EditorWindow
{
    private string dbPath;

    private List<NPCData> npcs = new();

    private List<DialogueData> dialogues = new();

    private List<QuestData> quests = new();

    private List<NPCDialogueCondition> conditions = new();

    private NPCDialogueCondition selectedCondition;

    private Vector2 conditionScroll;
    private NPCData selectedNPC;

    private Vector2 leftScroll;
    private Vector2 rightScroll;

    [MenuItem("Tools/RPG/NPC Editor")]
    public static void Open()
    {
        GetWindow<NPCEditor>("NPC Editor");
    }

    private void OnEnable()
    {
        dbPath =
            Path.Combine(
                Application.streamingAssetsPath,
                "Datagame.db"
            );

        LoadData();
    }
    public enum NPCType
    {
        Normal,
        Quest,
        Merchant,
        Blacksmith,
        Armorsmith,
        Innkeeper,
        Mystery
    }

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
            npcs =
                conn.Table<NPCData>()
                .OrderBy(x => x.id)
                .ToList();

            dialogues =
                conn.Table<DialogueData>()
                .ToList();

            conditions =
                conn.Table<NPCDialogueCondition>()
                .ToList();

            quests =
                conn.Table<QuestData>()
                .ToList();
        }

        Repaint();
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();

        DrawNPCList();

        DrawNPCInfo();

        DrawConditionPanel(); 

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        DrawBottomButtons();
    }

    //=================================================
    // LEFT PANEL
    //=================================================

    void DrawNPCList()
    {
        EditorGUILayout.BeginVertical(
            GUILayout.Width(300)
        );

        GUILayout.Label(
            "NPC List",
            EditorStyles.boldLabel
        );

        leftScroll =
            EditorGUILayout.BeginScrollView(
                leftScroll
            );

        foreach (var npc in npcs)
        {
            bool selected =
                selectedNPC != null &&
                selectedNPC.id == npc.id;

            Color old =
                GUI.backgroundColor;

            if (selected)
            {
                GUI.backgroundColor =
                    new Color(
                        0.4f,
                        0.6f,
                        1f
                    );
            }

            if (
                GUILayout.Button(
                    $"[{npc.id}] {npc.npc_name}"
                )
            )
            {
                selectedNPC =
                    CloneNPC(npc);

                GUI.FocusControl(null);
            }

            GUI.backgroundColor =
                old;
        }

        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);

        if (
            GUILayout.Button(
                "Add NPC",
                GUILayout.Height(30)
            )
        )
        {
            AddNPC();
        }

        if (
            selectedNPC != null &&
            GUILayout.Button(
                "Delete NPC",
                GUILayout.Height(30)
            )
        )
        {
            DeleteNPC();
        }

        EditorGUILayout.EndVertical();
    }

    //=================================================
    // RIGHT PANEL
    //=================================================

    void DrawNPCInfo()
    {
        EditorGUILayout.BeginVertical();

        GUILayout.Label(
            "NPC Detail",
            EditorStyles.boldLabel
        );

        if (selectedNPC == null)
        {
            EditorGUILayout.HelpBox(
                "Select NPC",
                MessageType.Info
            );

            EditorGUILayout.EndVertical();
            return;
        }

        rightScroll =
            EditorGUILayout.BeginScrollView(
                rightScroll
            );

        EditorGUILayout.LabelField(
            "ID",
            selectedNPC.id.ToString()
        );

        selectedNPC.npc_name =
            EditorGUILayout.TextField(
                "Name",
                selectedNPC.npc_name
            );

        DrawTypeDropdown();

        DrawDialogueDropdown();

        DrawQuestDropdown();

        selectedNPC.shop_id =
            EditorGUILayout.IntField(
                "Shop ID",
                selectedNPC.shop_id
            );

        GUILayout.Space(20);

        DrawDialoguePreview();

        EditorGUILayout.EndScrollView();

        EditorGUILayout.EndVertical();
    }
    //=================================================
    // MID  PANEL
    //=================================================
    void DrawConditionPanel()
    {
        EditorGUILayout.BeginVertical(
            GUILayout.Width(400)
        );

        GUILayout.Label(
            "Dialogue Conditions",
            EditorStyles.boldLabel
        );

        if (selectedNPC == null)
        {
            EditorGUILayout.EndVertical();
            return;
        }

        conditionScroll =
            EditorGUILayout.BeginScrollView(
                conditionScroll
            );

        var npcConditions =
            conditions
            .Where(
                x =>
                x.npc_id ==
                selectedNPC.id
            )
            .OrderBy(
                x =>
                x.priority
            );

        foreach (var condition in npcConditions)
        {
            string label =
                $"[{condition.priority}] " +
                $"{condition.required_quest_state} " +
                $"-> {condition.dialogue_group_id}";

            if (
                GUILayout.Button(label)
            )
            {
                selectedCondition =
                    CloneCondition(
                        condition
                    );
            }
        }

        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);

        if (
            GUILayout.Button(
                "Add Rule"
            )
        )
        {
            AddCondition();
        }

        if (
            selectedCondition != null
        )
        {
            DrawConditionDetail();
        }

        EditorGUILayout.EndVertical();
    }
    void DrawConditionDetail()
    {
        GUILayout.Space(10);

        GUILayout.Label(
            "Rule Detail",
            EditorStyles.boldLabel
        );

        selectedCondition.required_flag =
            EditorGUILayout.IntField(
                "Required Flag",
                selectedCondition.required_flag
            );

        DrawConditionQuestDropdown();

        DrawQuestStateDropdown();

        DrawConditionDialogueDropdown();

        selectedCondition.priority =
            EditorGUILayout.IntField(
                "Priority",
                selectedCondition.priority
            );

        if (
            GUILayout.Button(
                "Save Rule"
            )
        )
        {
            SaveCondition();
        }

        if (
            GUILayout.Button(
                "Delete Rule"
            )
        )
        {
            DeleteCondition();
        }
    }
    void DrawConditionQuestDropdown()
    {
        List<string> options =
            new();

        options.Add("None");

        foreach (var q in quests)
        {
            options.Add(
                $"[{q.id}] {q.quest_name}"
            );
        }

        int current = 0;

        for (
            int i = 0;
            i < quests.Count;
            i++
        )
        {
            if (
                quests[i].id ==
                selectedCondition.required_quest_id
            )
            {
                current = i + 1;
                break;
            }
        }

        int newIndex =
            EditorGUILayout.Popup(
                "Quest",
                current,
                options.ToArray()
            );

        selectedCondition.required_quest_id =
            newIndex == 0
            ? 0
            : quests[newIndex - 1].id;
    }
    readonly string[] questStates =
{
    "None",
    "NotStarted",
    "InProgress",
    "Completed",
    "Rewarded"
};

    void DrawQuestStateDropdown()
    {
        int current =
            System.Array.IndexOf(
                questStates,
                selectedCondition.required_quest_state
            );

        if (current < 0)
            current = 0;

        int newIndex =
            EditorGUILayout.Popup(
                "Quest State",
                current,
                questStates
            );

        selectedCondition.required_quest_state =
            questStates[newIndex];
    }
    void DrawConditionDialogueDropdown()
    {
        List<int> groups =
            dialogues
            .Select(
                x => x.group_id
            )
            .Distinct()
            .OrderBy(
                x => x
            )
            .ToList();

        string[] options =
            groups
            .Select(
                x =>
                $"Group {x}"
            )
            .ToArray();

        int current =
            groups.IndexOf(
                selectedCondition.dialogue_group_id
            );

        if (current < 0)
            current = 0;

        int newIndex =
            EditorGUILayout.Popup(
                "Dialogue Group",
                current,
                options
            );

        selectedCondition.dialogue_group_id =
            groups[newIndex];
    }
    void AddCondition()
    {
        selectedCondition =
            new NPCDialogueCondition
            {
                npc_id = selectedNPC.id,
                priority = 0,
                required_flag = 0,
                required_quest_id = 0,
                required_quest_state = "None",
                dialogue_group_id =
                    selectedNPC.dialogue_group_id
            };
    }
    void SaveCondition()
    {
        using (
            var conn =
            new SQLiteConnection(
                dbPath,
                SQLiteOpenFlags.ReadWrite
            )
        )
        {
            conn.InsertOrReplace(
                selectedCondition
            );
        }

        LoadData();
    }
    void DeleteCondition()
    {
        using (
            var conn =
            new SQLiteConnection(
                dbPath,
                SQLiteOpenFlags.ReadWrite
            )
        )
        {
            conn.Delete(
                selectedCondition
            );
        }

        selectedCondition = null;

        LoadData();
    }

    //=================================================
    // TYPE
    //=================================================

    void DrawTypeDropdown()
    {
        NPCType current;

        if (
            !System.Enum.TryParse(
                selectedNPC.npc_type,
                out current
            )
        )
        {
            current =
                NPCType.Normal;
        }

        current =
            (NPCType)
            EditorGUILayout.EnumPopup(
                "NPC Type",
                current
            );

        selectedNPC.npc_type =
            current.ToString();
    }

    //=================================================
    // DIALOGUE
    //=================================================

    void DrawDialogueDropdown()
    {
        List<int> groups =
            dialogues
            .Select(
                x => x.group_id
            )
            .Distinct()
            .OrderBy(
                x => x
            )
            .ToList();

        string[] options =
            groups
            .Select(
                x =>
                $"Group {x}"
            )
            .ToArray();

        int current =
            groups.IndexOf(
                selectedNPC.dialogue_group_id
            );

        if (current < 0)
            current = 0;

        int newIndex =
            EditorGUILayout.Popup(
                "Dialogue",
                current,
                options
            );

        if (
            groups.Count > 0
        )
        {
            selectedNPC.dialogue_group_id =
                groups[newIndex];
        }
    }

    //=================================================
    // QUEST
    //=================================================

    void DrawQuestDropdown()
    {
        if (
            quests.Count == 0
        )
            return;

        List<string> options =
            new();

        options.Add("None");

        foreach (var q in quests)
        {
            options.Add(
                $"[{q.id}] {q.quest_name}"
            );
        }

        int current = 0;

        for (
            int i = 0;
            i < quests.Count;
            i++
        )
        {
            if (
                quests[i].id ==
                selectedNPC.quest_id
            )
            {
                current =
                    i + 1;
                break;
            }
        }

        int newIndex =
            EditorGUILayout.Popup(
                "Quest",
                current,
                options.ToArray()
            );

        if (newIndex == 0)
        {
            selectedNPC.quest_id = 0;
        }
        else
        {
            selectedNPC.quest_id =
                quests[newIndex - 1].id;
        }
    }

    //=================================================
    // PREVIEW
    //=================================================

    void DrawDialoguePreview()
    {
        GUILayout.Label(
            "Dialogue Preview",
            EditorStyles.boldLabel
        );

        var lines =
            dialogues
            .Where(
                x =>
                x.group_id ==
                selectedNPC.dialogue_group_id
            )
            .OrderBy(
                x =>
                x.line_order
            );

        foreach (var line in lines)
        {
            EditorGUILayout.HelpBox(
                $"{line.speaker_name}\n{line.content}",
                MessageType.None
            );
        }
    }

    //=================================================
    // BUTTONS
    //=================================================

    void DrawBottomButtons()
    {
        EditorGUILayout.BeginHorizontal();

        if (
            GUILayout.Button(
                "Save",
                GUILayout.Height(35)
            )
        )
        {
            SaveNPC();
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

    //=================================================
    // SAVE
    //=================================================

    void SaveNPC()
    {
        if (selectedNPC == null)
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
                selectedNPC
            );
        }

        LoadData();

        Debug.Log(
            "NPC Saved"
        );
    }

    //=================================================
    // ADD
    //=================================================

    void AddNPC()
    {
        int nextID = 1;

        if (
            npcs.Count > 0
        )
        {
            nextID =
                npcs.Max(
                    x => x.id
                ) + 1;
        }

        selectedNPC =
            new NPCData()
            {
                id = nextID,
                npc_name = "New NPC",
                npc_type = "Normal",
                dialogue_group_id = 0,
                shop_id = 0,
                quest_id = 0
            };
    }

    //=================================================
    // DELETE
    //=================================================

    void DeleteNPC()
    {
        if (
            !EditorUtility.DisplayDialog(
                "Delete NPC",
                $"Delete {selectedNPC.npc_name} ?",
                "Delete",
                "Cancel"
            )
        )
        {
            return;
        }

        using (
            var conn =
            new SQLiteConnection(
                dbPath,
                SQLiteOpenFlags.ReadWrite
            )
        )
        {
            conn.Delete(
                selectedNPC
            );
        }

        selectedNPC = null;

        LoadData();
    }

    //=================================================
    // CLONE
    //=================================================
    NPCDialogueCondition CloneCondition(
    NPCDialogueCondition c
)
    {
        return new NPCDialogueCondition
        {
            id = c.id,
            npc_id = c.npc_id,
            required_flag = c.required_flag,
            required_quest_id =
                c.required_quest_id,
            required_quest_state =
                c.required_quest_state,
            dialogue_group_id =
                c.dialogue_group_id,
            priority =
                c.priority
        };
    }
    NPCData CloneNPC(
        NPCData npc
    )
    {
        return new NPCData
        {
            id = npc.id,
            npc_name = npc.npc_name,
            npc_type = npc.npc_type,
            dialogue_group_id = npc.dialogue_group_id,
            shop_id = npc.shop_id,
            quest_id = npc.quest_id
        };
    }
}