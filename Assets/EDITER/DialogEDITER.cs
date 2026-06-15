using UnityEditor;
using UnityEngine;
using SQLite4Unity3d;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class DialogueEditor : EditorWindow
{
    private string dbPath;

    private List<DialogueData> dialogues =
        new();

    private List<int> groups =
        new();

    private Vector2 leftScroll;
    private Vector2 centerScroll;
    private Vector2 rightScroll;

    private int selectedGroupID = -1;

    private DialogueData selectedDialogue;

    // =====================================================
    // OPEN
    // =====================================================

    [MenuItem("Tools/RPG/Dialogue Editor")]
    public static void Open()
    {
        GetWindow<DialogueEditor>(
            "Dialogue Editor"
        );
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
            dialogues =
                conn.Table<DialogueData>()
                .OrderBy(x => x.group_id)
                .ThenBy(x => x.line_order)
                .ToList();
        }

        groups =
            dialogues
            .Select(x => x.group_id)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        Repaint();
    }

    // =====================================================
    // GUI
    // =====================================================

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();

        DrawGroupPanel();

        DrawLinePanel();

        DrawDetailPanel();

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        DrawBottomButtons();
    }

    // =====================================================
    // GROUP PANEL
    // =====================================================

    void DrawGroupPanel()
    {
        EditorGUILayout.BeginVertical(
            GUILayout.Width(250)
        );

        GUILayout.Label(
            "Dialogue Groups",
            EditorStyles.boldLabel
        );

        leftScroll =
            EditorGUILayout.BeginScrollView(
                leftScroll
            );

        foreach (var groupID in groups)
        {
            bool selected =
                selectedGroupID ==
                groupID;

            Color oldColor =
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
                    $"Group {groupID}"
                )
            )
            {
                selectedGroupID =
                    groupID;

                selectedDialogue =
                    null;
            }

            GUI.backgroundColor =
                oldColor;
        }

        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);

        if (
            GUILayout.Button(
                "Add Group",
                GUILayout.Height(30)
            )
        )
        {
            AddGroup();
        }

        if (
            selectedGroupID != -1 &&
            GUILayout.Button(
                "Delete Group",
                GUILayout.Height(30)
            )
        )
        {
            DeleteGroup();
        }

        EditorGUILayout.EndVertical();
    }

    // =====================================================
    // LINE PANEL
    // =====================================================

    void DrawLinePanel()
    {
        EditorGUILayout.BeginVertical(
            GUILayout.Width(300)
        );

        GUILayout.Label(
            "Dialogue Lines",
            EditorStyles.boldLabel
        );

        if (selectedGroupID == -1)
        {
            EditorGUILayout.HelpBox(
                "Select Group",
                MessageType.Info
            );

            EditorGUILayout.EndVertical();
            return;
        }

        centerScroll =
            EditorGUILayout.BeginScrollView(
                centerScroll
            );

        var lines =
            dialogues
            .Where(
                x =>
                x.group_id ==
                selectedGroupID
            )
            .OrderBy(
                x =>
                x.line_order
            );

        foreach (var line in lines)
        {
            bool selected =
                selectedDialogue != null &&
                selectedDialogue.id ==
                line.id;

            Color oldColor =
                GUI.backgroundColor;

            if (selected)
            {
                GUI.backgroundColor =
                    new Color(
                        0.4f,
                        1f,
                        0.4f
                    );
            }

            if (
                GUILayout.Button(
                    $"{line.line_order}. {line.speaker_name}"
                )
            )
            {
                selectedDialogue =
                    CloneDialogue(line);
            }

            GUI.backgroundColor =
                oldColor;
        }

        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);

        if (
            GUILayout.Button(
                "Add Line",
                GUILayout.Height(30)
            )
        )
        {
            AddLine();
        }
        EditorGUILayout.BeginHorizontal();

        if (
            selectedDialogue != null &&
            GUILayout.Button("Up")
        )
        {
            MoveLineUp();
        }

        if (
            selectedDialogue != null &&
            GUILayout.Button("Down")
        )
        {
            MoveLineDown();
        }

        EditorGUILayout.EndHorizontal();

        if (
            selectedDialogue != null &&
            GUILayout.Button(
                "Delete Line",
                GUILayout.Height(30)
            )
        )
        {
            DeleteLine();
        }

        EditorGUILayout.EndVertical();
    }

    // =====================================================
    // DETAIL PANEL
    // =====================================================

    void DrawDetailPanel()
    {
        EditorGUILayout.BeginVertical();

        GUILayout.Label(
            "Dialogue Detail",
            EditorStyles.boldLabel
        );

        if (selectedDialogue == null)
        {
            EditorGUILayout.HelpBox(
                "Select Dialogue",
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
            selectedDialogue.id.ToString()
        );

        selectedDialogue.group_id =
            EditorGUILayout.IntField(
                "Group ID",
                selectedDialogue.group_id
            );

        selectedDialogue.line_order =
            EditorGUILayout.IntField(
                "Line Order",
                selectedDialogue.line_order
            );

        selectedDialogue.speaker_name =
            EditorGUILayout.TextField(
                "Speaker",
                selectedDialogue.speaker_name
            );

        GUILayout.Space(10);

        GUILayout.Label(
            "Content"
        );

        selectedDialogue.content =
            EditorGUILayout.TextArea(
                selectedDialogue.content,
                GUILayout.Height(200)
            );

        EditorGUILayout.EndScrollView();

        EditorGUILayout.EndVertical();
    }

    // =====================================================
    // BOTTOM BUTTONS
    // =====================================================

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
            SaveDialogue();
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

    void SaveDialogue()
    {
        if (selectedDialogue == null)
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
                selectedDialogue
            );
        }

        LoadData();

        Debug.Log(
            "Dialogue Saved"
        );
    }

    // =====================================================
    // ADD GROUP
    // =====================================================

    void AddGroup()
    {
        int nextGroupID = 100;

        if (groups.Count > 0)
        {
            nextGroupID =
                groups.Max() + 1;
        }

        DialogueData dialogue =
            new DialogueData()
            {
                group_id =
                    nextGroupID,

                line_order = 1,

                speaker_name =
                    "NPC",

                content =
                    "New Dialogue"
            };

        using (
            var conn =
            new SQLiteConnection(
                dbPath,
                SQLiteOpenFlags.ReadWrite
            )
        )
        {
            conn.Insert(dialogue);
        }

        LoadData();

        selectedGroupID =
            nextGroupID;
    }

    // =====================================================
    // DELETE GROUP
    // =====================================================

    void DeleteGroup()
    {
        if (
            !EditorUtility.DisplayDialog(
                "Delete Group",
                $"Delete Group {selectedGroupID} ?",
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
                "DELETE FROM Dialogues WHERE group_id=?",
                selectedGroupID
            );
        }

        selectedGroupID = -1;
        selectedDialogue = null;

        LoadData();
    }

    // =====================================================
    // ADD LINE
    // =====================================================

    void AddLine()
    {
        int nextOrder =
            dialogues
            .Where(
                x =>
                x.group_id ==
                selectedGroupID
            )
            .Count() + 1;

        DialogueData dialogue =
            new DialogueData()
            {
                group_id =
                    selectedGroupID,

                line_order =
                    nextOrder,

                speaker_name =
                    "NPC",

                content =
                    "New Line"
            };

        using (
            var conn =
            new SQLiteConnection(
                dbPath,
                SQLiteOpenFlags.ReadWrite
            )
        )
        {
            conn.Insert(
                dialogue
            );
        }

        LoadData();
    }

    // =====================================================
    // DELETE LINE
    // =====================================================

    void DeleteLine()
    {
        if (selectedDialogue == null)
            return;

        using (
            var conn =
            new SQLiteConnection(
                dbPath,
                SQLiteOpenFlags.ReadWrite
            )
        )
        {
            conn.Delete(
                selectedDialogue
            );
        }

        selectedDialogue = null;

        LoadData();
    }
    // =====================================================
    // UP
    // =====================================================
    void MoveLineUp()
    {
        if (selectedDialogue == null)
            return;

        var lines =
            dialogues
            .Where(
                x =>
                x.group_id ==
                selectedDialogue.group_id
            )
            .OrderBy(
                x =>
                x.line_order
            )
            .ToList();

        int index =
            lines.FindIndex(
                x =>
                x.id ==
                selectedDialogue.id
            );

        if (index <= 0)
            return;

        DialogueData current =
            lines[index];

        DialogueData previous =
            lines[index - 1];

        int temp =
            current.line_order;

        current.line_order =
            previous.line_order;

        previous.line_order =
            temp;

        using (
            var conn =
            new SQLiteConnection(
                dbPath,
                SQLiteOpenFlags.ReadWrite
            )
        )
        {
            conn.Update(current);
            conn.Update(previous);
        }

        LoadData();

        selectedDialogue =
            CloneDialogue(previous);
    }
    // =====================================================
    // Down
    // =====================================================
    void MoveLineDown()
    {
        if (selectedDialogue == null)
            return;

        var lines =
            dialogues
            .Where(
                x =>
                x.group_id ==
                selectedDialogue.group_id
            )
            .OrderBy(
                x =>
                x.line_order
            )
            .ToList();

        int index =
            lines.FindIndex(
                x =>
                x.id ==
                selectedDialogue.id
            );

        if (index >= lines.Count - 1)
            return;

        DialogueData current =
            lines[index];

        DialogueData next =
            lines[index + 1];

        int temp =
            current.line_order;

        current.line_order =
            next.line_order;

        next.line_order =
            temp;

        using (
            var conn =
            new SQLiteConnection(
                dbPath,
                SQLiteOpenFlags.ReadWrite
            )
        )
        {
            conn.Update(current);
            conn.Update(next);
        }

        LoadData();

        selectedDialogue =
            CloneDialogue(next);
    }

    // =====================================================
    // CLONE
    // =====================================================

    DialogueData CloneDialogue(
        DialogueData d
    )
    {
        return new DialogueData
        {
            id = d.id,
            group_id = d.group_id,
            line_order = d.line_order,
            speaker_name = d.speaker_name,
            content = d.content
        };
    }
}