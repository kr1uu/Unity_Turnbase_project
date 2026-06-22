using SQLite4Unity3d;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPreviewUI : MonoBehaviour
{
    public static CharacterPreviewUI Instance;

    [Header("UI")]
    public Image portraitImage;

    public TMP_Text nameText;
    public TMP_Text hpText;
    public TMP_Text atkText;
    public TMP_Text defText;
    public TMP_Text spdText;

    public Transform skillParent;
    public GameObject skillSlotPrefab;

    public Button startButton;

    [Header("Database")]
    public CharacterSpriteDatabase spriteDB;

    private int selectedCharacterId;

    private SQLiteConnection db;

    void Awake()
    {
        Instance = this;

        string dbPath =
            Path.Combine(
                Application.streamingAssetsPath,
                "Datagame.db"
            );

        db = new SQLiteConnection(
            dbPath,
            SQLiteOpenFlags.ReadOnly
        );
        //var allRows = db.Table<CharacterSkillData>().ToList();

        //Debug.Log("===== CharacterSkills =====");

        //foreach (var row in allRows)
        //{
        //    Debug.Log(
        //        $"ID={row.id}, Character={row.characterId}, Skill={row.skillId}"
        //    );
        //}
    }
    void Start()
    {
        CharacterData firstCharacter =
            db.Table<CharacterData>()
              .OrderBy(x => x.id)
              .FirstOrDefault();

        if (firstCharacter != null)
        {
            ShowCharacter(firstCharacter.id);
        }
    }
    public void ShowCharacter(int characterId)
    {
        selectedCharacterId = characterId;

        CharacterData character =
            db.Table<CharacterData>()
            .FirstOrDefault(x => x.id == characterId);

        if (character == null)
            return;

        //----------------------------------
        // Portrait
        //----------------------------------

        portraitImage.sprite =
            spriteDB.GetSplashArt(characterId);

        //----------------------------------
        // Stats
        //----------------------------------

        nameText.text = character.name;

        hpText.text = $"HP : {character.hp}";
        atkText.text = $"ATK : {character.atk}";
        defText.text = $"DEF : {character.def}";
        spdText.text = $"SPD : {character.spd}";

        //----------------------------------
        // Skills
        //----------------------------------

        foreach (Transform child in skillParent)
            Destroy(child.gameObject);

        var skillIds =
        db.Table<CharacterSkillData>()
          .Where(x => x.characterId == characterId)
          .ToList()
          .Select(x => x.skillId)
          .ToList();

        Debug.Log($"Character ID = {characterId}");
        Debug.Log($"Skill Count = {skillIds.Count}");

        foreach (var id in skillIds)
        {
            Debug.Log($"SkillID = {id}");
        }

        foreach (int skillId in skillIds)
        {
            SkillData skill =
                db.Table<SkillData>()
                .FirstOrDefault(x => x.id == skillId);

            Debug.Log($"Loading Skill {skillId}");

            if (skill == null)
            {
                Debug.LogError($"Skill {skillId} not found!");
                continue;
            }

            GameObject slot =
                Instantiate(skillSlotPrefab, skillParent);

            Debug.Log($"Spawned {slot.name}");

            SkillSlotUI ui =
                slot.GetComponent<SkillSlotUI>();

            Debug.Log($"UI = {ui}");

            ui.Setup(skill);
        }

        startButton.gameObject.SetActive(true);
    }

    public void OnStartButton()
    {
        FindAnyObjectByType<GameStartManager>()
            .SelectStarter(selectedCharacterId);
    }
}