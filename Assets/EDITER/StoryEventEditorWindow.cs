using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using SQLite4Unity3d;
using System.IO;
using System.Linq;

public class StoryEventEditorWindow : EditorWindow
{
    private StoryEventComponent selectedEvent;
    private StoryEventComponent[] allEvents;
    private string[] eventNames;
    private int selectedIndex;

    private List<StoryFlagData> flags = new();

    private Vector2 scroll;

    [MenuItem("Tools/RPG/Story Event Editor")]
    public static void Open()
    {
        GetWindow<StoryEventEditorWindow>(
            "Story Event Editor"
        );
    }
    private void OnEnable()
    {
        LoadFlags();
        RefreshEvents();
    }
    private void RefreshEvents()
    {
        allEvents =
            FindObjectsByType<StoryEventComponent>(
                FindObjectsSortMode.None
            );

        eventNames =
            allEvents
            .Select(x =>
            string.IsNullOrEmpty(x.data.eventID)
            ? x.gameObject.name
            : x.data.eventID)
            .ToArray();
    }
    private void LoadFlags()
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

            flags =
                db.Table<StoryFlagData>()
                .OrderBy(x => x.flag_name)
                .ToList();

            Debug.Log(
                $"Loaded {flags.Count} Story Flags"
            );
        }
        catch (System.Exception ex)
        {
            Debug.LogError(
                $"LoadFlags Error: {ex.Message}"
            );
        }
    }
    private void OnGUI()
    {
        GUILayout.Space(5);

        EditorGUILayout.LabelField(
            "Story Event Editor",
            EditorStyles.boldLabel
        );

        GUILayout.Space(5);

        if (GUILayout.Button("Refresh Events"))
        {
            RefreshEvents();
        }

        if (allEvents == null || allEvents.Length == 0)
        {
            EditorGUILayout.HelpBox(
                "No Story Events found in scene.",
                MessageType.Warning
            );

            return;
        }

        selectedIndex =
            EditorGUILayout.Popup(
                "Story Event",
                selectedIndex,
                eventNames
            );

        selectedEvent =
            allEvents[selectedIndex];
        if (GUILayout.Button("Ping Object"))
        {
            Selection.activeGameObject =
                selectedEvent.gameObject;

            EditorGUIUtility.PingObject(
                selectedEvent.gameObject
            );
        }

        if (selectedEvent == null)
        {
            EditorGUILayout.HelpBox(
                "Select a StoryEventComponent from the scene.",
                MessageType.Info
            );

            return;
        }

        if (selectedEvent.data == null)
        {
            selectedEvent.data =
                new StoryEventData();
        }

        scroll =
            EditorGUILayout.BeginScrollView(
                scroll
            );

        DrawEventData();

        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);

        if (GUILayout.Button(
            "Save",
            GUILayout.Height(35)
        ))
        {
            EditorUtility.SetDirty(
                selectedEvent
            );

            Debug.Log(
                $"[StoryEventEditor] Saved {selectedEvent.data.eventID}"
            );
        }
    }

    private void DrawEventData()
    {
        var data = selectedEvent.data;

        GUILayout.Space(10);

        EditorGUILayout.LabelField(
            "Basic Settings",
            EditorStyles.boldLabel
        );

        data.eventID =
            EditorGUILayout.TextField(
                "Event ID",
                data.eventID
            );

        data.requiredFlag =
             DrawFlagPopup(
             "Required Flag",
             data.requiredFlag
            );

        data.setFlag =
             DrawFlagPopup(
                 "Set Flag",
                 data.setFlag
             );

        data.triggerOnce =
            EditorGUILayout.Toggle(
                "Trigger Once",
                data.triggerOnce
            );

        GUILayout.Space(10);

        EditorGUILayout.LabelField(
            "Event Type",
            EditorStyles.boldLabel
        );

        data.type =
            (EventType)
            EditorGUILayout.EnumPopup(
                "Type",
                data.type
            );

        GUILayout.Space(10);

        DrawTypeSpecificFields(data);
    }

    private void DrawTypeSpecificFields(
        StoryEventData data
    )
    {
        switch (data.type)
        {
            // ==========================
            // DIALOGUE
            // ==========================

            case EventType.Dialogue:

                data.dialogueGroupID =
                    EditorGUILayout.IntField(
                        "Dialogue Group ID",
                        data.dialogueGroupID
                    );

                break;

            // ==========================
            // START QUEST
            // ==========================

            case EventType.StartQuest:

                data.questID =
                    EditorGUILayout.IntField(
                        "Quest ID",
                        data.questID
                    );

                break;

            // ==========================
            // COMPLETE QUEST
            // ==========================

            case EventType.CompleteQuest:

                data.questID =
                    EditorGUILayout.IntField(
                        "Quest ID",
                        data.questID
                    );

                break;

            // ==========================
            // ENABLE OBJECT
            // ==========================

            case EventType.EnableObject:

                data.targetObjectName =
                    EditorGUILayout.TextField(
                        "Target Object",
                        data.targetObjectName
                    );

                break;

            // ==========================
            // SPAWN OBJECT
            // ==========================

            case EventType.SpawnObject:

                data.targetObjectName =
                    EditorGUILayout.TextField(
                        "Prefab/Object Name",
                        data.targetObjectName
                    );

                break;

            // ==========================
            // PLAY CUTSCENE
            // ==========================

            case EventType.PlayCutscene:

                data.cutscene =
                    (CutsceneData)
                    EditorGUILayout.ObjectField(
                        "Cutscene",
                        data.cutscene,
                        typeof(CutsceneData),
                        false
                    );

                break;
        }
    }
    private int DrawFlagPopup(
     string label,
     int currentFlagID
 )
    {
        if (flags.Count == 0)
            return currentFlagID;

        List<string> names = new();
        names.Add("<None>");

        foreach (var flag in flags)
        {
            names.Add(
                $"[{flag.id}] {flag.flag_name}"
            );
        }

        int index = 0;

        for (int i = 0; i < flags.Count; i++)
        {
            if (flags[i].id == currentFlagID)
            {
                index = i + 1;
                break;
            }
        }

        index = EditorGUILayout.Popup(
            label,
            index,
            names.ToArray()
        );

        if (index == 0)
            return 0;

        return flags[index - 1].id;
    }
}