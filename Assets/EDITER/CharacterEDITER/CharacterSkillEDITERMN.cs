using UnityEditor;
using UnityEngine;
using SQLite4Unity3d;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class CharacterSkillEditor : EditorWindow
{
    private string dbPath;

    private List<CharacterData> characters =
        new();

    private List<SkillData> skills =
        new();

    private List<CharacterSkillLink> links =
        new();

    private Vector2 leftScroll;
    private Vector2 rightScroll;

    private int selectedCharacter = -1;

    // =========================================
    // DELAY ACTION
    // =========================================

    private bool pendingAction = false;

    private bool pendingAdd;

    private int pendingCharID;
    private int pendingSkillID;

    // =====================================================
    // OPEN
    // =====================================================

    [MenuItem("Tools/RPG/Character Skill Editor")]
    public static void Open()
    {
        GetWindow<CharacterSkillEditor>(
            "Character Skill Editor"
        );
    }

    // =====================================================
    // ENABLE
    // =====================================================

    void OnEnable()
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
            characters =
                conn.Table<CharacterData>()
                .ToList();

            skills =
                conn.Table<SkillData>()
                .ToList();

            links =
                conn.Table<CharacterSkillLink>()
                .ToList();
        }

        Repaint();
    }

    // =====================================================
    // GUI
    // =====================================================

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();

        DrawCharacterList();

        DrawSkillPanel();

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        if (
            GUILayout.Button(
                "Reload Database",
                GUILayout.Height(30)
            )
        )
        {
            LoadData();
        }

        // =====================================
        // EXECUTE DELAYED ACTION
        // =====================================

        if (pendingAction)
        {
            pendingAction = false;

            if (pendingAdd)
            {
                AddSkill(
                    pendingCharID,
                    pendingSkillID
                );
            }
            else
            {
                RemoveSkill(
                    pendingCharID,
                    pendingSkillID
                );
            }

            LoadData();
        }
    }

    // =====================================================
    // CHARACTER LIST
    // =====================================================

    void DrawCharacterList()
    {
        EditorGUILayout.BeginVertical(
            GUILayout.Width(250)
        );

        GUILayout.Label(
            "Characters",
            EditorStyles.boldLabel
        );

        leftScroll =
            EditorGUILayout.BeginScrollView(
                leftScroll
            );

        foreach (var c in characters)
        {
            bool selected =
                selectedCharacter == c.id;

            if (
                GUILayout.Toggle(
                    selected,
                    $"{c.id} - {c.name}",
                    "Button"
                )
            )
            {
                selectedCharacter = c.id;
            }
        }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.EndVertical();
    }

    // =====================================================
    // SKILL PANEL
    // =====================================================

    void DrawSkillPanel()
    {
        EditorGUILayout.BeginVertical();

        GUILayout.Label(
            "Skills",
            EditorStyles.boldLabel
        );

        if (selectedCharacter == -1)
        {
            EditorGUILayout.HelpBox(
                "Select Character",
                MessageType.Info
            );

            EditorGUILayout.EndVertical();

            return;
        }

        rightScroll =
            EditorGUILayout.BeginScrollView(
                rightScroll
            );

        foreach (var skill in skills)
        {
            bool hasSkill =
                links.Any(
                    x =>
                    x.character_id ==
                    selectedCharacter
                    &&
                    x.skill_id ==
                    skill.id
                );

            EditorGUI.BeginChangeCheck();

            bool newValue =
                EditorGUILayout.ToggleLeft(
                    $"{skill.id} - {skill.name}",
                    hasSkill
                );

            if (EditorGUI.EndChangeCheck())
            {
                pendingAction = true;

                pendingAdd = newValue;

                pendingCharID =
                    selectedCharacter;

                pendingSkillID =
                    skill.id;
            }
        }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.EndVertical();
    }

    // =====================================================
    // ADD
    // =====================================================

    void AddSkill(
        int charID,
        int skillID
    )
    {
        try
        {
            bool exists =
                links.Any(
                    x =>
                    x.character_id == charID
                    &&
                    x.skill_id == skillID
                );

            if (exists)
                return;

            CharacterSkillLink link =
                new CharacterSkillLink
                {
                    character_id = charID,
                    skill_id = skillID
                };

            using (
                var conn =
                new SQLiteConnection(
                    dbPath,
                    SQLiteOpenFlags.ReadWrite
                )
            )
            {
                conn.Insert(link);
            }

            Debug.Log(
                $"ADD SKILL {skillID} TO {charID}"
            );
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }

    // =====================================================
    // REMOVE
    // =====================================================

    void RemoveSkill(
        int charID,
        int skillID
    )
    {
        try
        {
            using (
                var conn =
                new SQLiteConnection(
                    dbPath,
                    SQLiteOpenFlags.ReadWrite
                )
            )
            {
                conn.Execute(
                    "DELETE FROM CharacterSkills WHERE character_id=? AND skill_id=?",
                    charID,
                    skillID
                );
            }

            Debug.Log(
                $"REMOVE SKILL {skillID} FROM {charID}"
            );
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }
}