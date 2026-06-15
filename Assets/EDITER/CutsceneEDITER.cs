using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using SQLite4Unity3d;
using System.IO;

public class CutsceneEditorWindow : EditorWindow
{
    private CutsceneData cutscene;

    private Vector2 scroll;

    private List<StoryFlagData> flags =
        new();

    [MenuItem("Tools/RPG/Cutscene Editor")]
    public static void Open()
    {
        GetWindow<CutsceneEditorWindow>(
            "Cutscene Editor"
        );
    }

    private void OnEnable()
    {
        LoadFlags();
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
        }
        catch
        {
            flags = new();
        }
    }

    private void OnGUI()
    {
        GUILayout.Space(5);

        EditorGUILayout.LabelField(
            "Cutscene Editor",
            EditorStyles.boldLabel
        );

        GUILayout.Space(5);

        cutscene =
            (CutsceneData)
            EditorGUILayout.ObjectField(
                "Cutscene",
                cutscene,
                typeof(CutsceneData),
                false
            );

        if (cutscene == null)
        {
            EditorGUILayout.HelpBox(
                "Select a CutsceneData asset.",
                MessageType.Info
            );

            return;
        }

        GUILayout.Space(10);

        if (
            GUILayout.Button(
                "Add Action",
                GUILayout.Height(30)
            )
        )
        {
            Undo.RecordObject(
                cutscene,
                "Add Action"
            );

            cutscene.actions.Add(
                new CutsceneAction()
            );

            EditorUtility.SetDirty(
                cutscene
            );
        }

        GUILayout.Space(10);

        scroll =
            EditorGUILayout.BeginScrollView(
                scroll
            );

        for (
            int i = 0;
            i < cutscene.actions.Count;
            i++
        )
        {
            DrawAction(
                cutscene.actions[i],
                i
            );
        }

        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);

        if (
            GUILayout.Button(
                "Save",
                GUILayout.Height(35)
            )
        )
        {
            EditorUtility.SetDirty(
                cutscene
            );

            AssetDatabase.SaveAssets();

            Debug.Log(
                $"Saved Cutscene : {cutscene.name}"
            );
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(
                cutscene
            );
        }
    }

    private void DrawAction(
        CutsceneAction action,
        int index
    )
    {
        EditorGUILayout.BeginVertical(
            "box"
        );

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(
            $"Action {index}",
            EditorStyles.boldLabel
        );

        if (
            GUILayout.Button(
                "^",
                GUILayout.Width(30)
            )
            && index > 0
        )
        {
            Undo.RecordObject(
                cutscene,
                "Move Action Up"
            );

            (
                cutscene.actions[index],
                cutscene.actions[index - 1]
            )
            =
            (
                cutscene.actions[index - 1],
                cutscene.actions[index]
            );
        }

        if (
            GUILayout.Button(
                "v",
                GUILayout.Width(30)
            )
            &&
            index <
            cutscene.actions.Count - 1
        )
        {
            Undo.RecordObject(
                cutscene,
                "Move Action Down"
            );

            (
                cutscene.actions[index],
                cutscene.actions[index + 1]
            )
            =
            (
                cutscene.actions[index + 1],
                cutscene.actions[index]
            );
        }

        if (
            GUILayout.Button(
                "D",
                GUILayout.Width(30)
            )
        )
        {
            Undo.RecordObject(
                cutscene,
                "Duplicate Action"
            );

            CutsceneAction clone =
                JsonUtility.FromJson<CutsceneAction>(
                    JsonUtility.ToJson(action)
                );

            cutscene.actions.Insert(
                index + 1,
                clone
            );
        }

        if (
            GUILayout.Button(
                "X",
                GUILayout.Width(30)
            )
        )
        {
            Undo.RecordObject(
                cutscene,
                "Delete Action"
            );

            cutscene.actions.RemoveAt(
                index
            );

            GUIUtility.ExitGUI();
        }

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5);

        action.type =
            (CutsceneActionType)
            EditorGUILayout.EnumPopup(
                "Type",
                action.type
            );

        DrawActionFields(action);

        EditorGUILayout.EndVertical();

        GUILayout.Space(5);
    }

    private void DrawActionFields(
        CutsceneAction action
    )
    {
        switch (action.type)
        {
            case CutsceneActionType.MoveObject:

                DrawSceneObjectPopup(
                    action
                );

                action.targetPosition =
                    EditorGUILayout.Vector3Field(
                        "Target Position",
                        action.targetPosition
                    );

                action.duration =
                    EditorGUILayout.FloatField(
                        "Duration",
                        action.duration
                    );

                break;

            case CutsceneActionType.Dialogue:

                action.dialogueGroupID =
                    EditorGUILayout.IntField(
                        "Dialogue Group ID",
                        action.dialogueGroupID
                    );

                break;

            case CutsceneActionType.Wait:

                action.duration =
                    EditorGUILayout.FloatField(
                        "Duration",
                        action.duration
                    );

                break;

            case CutsceneActionType.SetFlag:

                action.flagToSet =
                    DrawFlagPopup(
                        "Flag",
                        action.flagToSet
                    );

                break;

            case CutsceneActionType.EnableObject:

            case CutsceneActionType.DisableObject:

                DrawSceneObjectPopup(
                    action
                );

                break;

            case CutsceneActionType.StartQuest:

            case CutsceneActionType.CompleteQuest:

                action.questID =
                    EditorGUILayout.IntField(
                        "Quest ID",
                        action.questID
                    );

                break;

            case CutsceneActionType.StartBattle:

                DrawEncounterPopup(
                    action
                );

                break;
        }
    }

    private void DrawSceneObjectPopup(
        CutsceneAction action
    )
    {
        SceneObjectID[] objs =
            Resources
            .FindObjectsOfTypeAll<
                SceneObjectID>()
            .Where(x =>
                x.gameObject.scene.IsValid() &&
                !EditorUtility.IsPersistent(x)
            )
            .ToArray();

        if (objs.Length == 0)
        {
            EditorGUILayout.HelpBox(
                "No SceneObjectID found.",
                MessageType.Warning
            );

            return;
        }

        string[] ids =
            objs
            .Select(x => x.objectID)
            .ToArray();

        int index =
            System.Array.IndexOf(
                ids,
                action.targetObjectID
            );

        if (index < 0)
            index = 0;

        index =
            EditorGUILayout.Popup(
                "Target Object",
                index,
                ids
            );

        action.targetObjectID =
            ids[index];
    }

    private int DrawFlagPopup(
        string label,
        int currentFlagID
    )
    {
        if (flags.Count == 0)
            return currentFlagID;

        List<string> names =
            new();

        names.Add("<None>");

        foreach (var flag in flags)
        {
            names.Add(
                $"[{flag.id}] {flag.flag_name}"
            );
        }

        int index = 0;

        for (
            int i = 0;
            i < flags.Count;
            i++
        )
        {
            if (
                flags[i].id ==
                currentFlagID
            )
            {
                index = i + 1;
                break;
            }
        }

        index =
            EditorGUILayout.Popup(
                label,
                index,
                names.ToArray()
            );

        if (index == 0)
            return 0;

        return flags[index - 1].id;
    }

    private void DrawEncounterPopup(
        CutsceneAction action
    )
    {
        BattleTrigger[] triggers =
            FindObjectsByType<
                BattleTrigger>(
                FindObjectsSortMode.None
            );

        if (triggers.Length == 0)
        {
            EditorGUILayout.HelpBox(
                "No BattleTrigger found.",
                MessageType.Warning
            );

            return;
        }

        string[] ids =
            triggers
            .Select(x =>
                x.encounterID
            )
            .ToArray();

        int index =
            System.Array.IndexOf(
                ids,
                action.encounterID
            );

        if (index < 0)
            index = 0;

        index =
            EditorGUILayout.Popup(
                "Encounter",
                index,
                ids
            );

        action.encounterID =
            ids[index];
    }
}