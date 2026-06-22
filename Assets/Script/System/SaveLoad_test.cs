using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadUI : MonoBehaviour
{
    public PlayerState player;
    public static SaveLoadUI Instance;

    private GameData pendingLoadData;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        player = new PlayerState();

        player.characterId = 1;
        player.level = 1;
        player.currentExp = 0;
        player.gold = 0;
        player.currentHP = 80;
    }

    // =====================================================
    // SAVE
    // =====================================================

    public void SaveGame(int slotID)
    {
        player = PlayerProgression.Instance.player;

        GameData data = new GameData();

        data.player = player;

        data.currentScene =
            SceneManager.GetActiveScene().name;

        data.encounters =
            new List<EncounterSaveData>();

        GameObject playerObj =
     GameObject.FindGameObjectWithTag(
         "Player"
     );

        if (playerObj != null)
        {
            Transform playerTransform =
                playerObj.transform;

            data.player.posX =
                playerTransform.position.x;

            data.player.posY =
                playerTransform.position.y;

            data.player.posZ =
                playerTransform.position.z;
        }

        // =====================
        // SAVE ENCOUNTERS
        // =====================

        foreach (var kvp in
            EncounterStateManager.Instance.GetAll())
        {
            EncounterSaveData e =
                new EncounterSaveData();

            e.chunkID = kvp.Key;

            e.defeatedIDs =
                new List<string>(kvp.Value);

            data.encounters.Add(e);
        }

        // =====================
        // SAVE TEAM
        // =====================

        data.selectedTeamIDs =
            new List<int>(
                PartyManager.Instance
                    .SelectedPlayerIDs
            );

        // =====================
        // SAVE EQUIPMENT
        // =====================

        foreach (var member in
            PartyManager.Instance.PartyStats)
        {
            data.equipments.Add(
                new CharacterEquipmentSaveData
                {
                    characterID = member.id,
                    weaponID = member.weaponID,
                    armorID = member.armorID,
                    accessoryID = member.accessoryID
                }
            );
        }

        // =====================
        // SAVE INVENTORY
        // =====================

        foreach (var item in
            InventoryManager.Instance.items)
        {
            data.inventoryItems.Add(
                new InventoryItemSaveData
                {
                    itemID = item.itemID,
                    amount = item.quantity
                }
            );
        }
        // =====================
        // SAVE QUESTS
        // =====================

        data.quests.Clear();

        foreach (
            var q in QuestManager.Instance
            .activeQuests
        )
        {
            data.quests.Add(
                new QuestSaveData
                {
                    questID = q.questID,

                    currentAmount =
                        q.currentAmount,

                    completed =
                        q.completed,

                    rewarded =
                        q.rewarded,

                    state =
                        (int)q.state
                }
            );
        }

        // =====================
        // SAVE CHESTS
        // =====================

        data.chests.Clear();

        foreach (var chest in
            FindObjectsByType<LootChest>(
                FindObjectsSortMode.None))
        {
            data.chests.Add(
                new ChestSaveData
                {
                    chestID = chest.chestID,
                    opened = chest.opened
                }
            );
        }
        if (StoryFlagManager.Instance != null)
        {
            data.storyFlags =
                StoryFlagManager.Instance.GetAllFlags();
        }
        else
        {
            data.storyFlags =
                new List<int>();

            Debug.LogWarning(
                "StoryFlagManager NULL"
            );
        }
        SaveSystem.Save(data,slotID);

        Debug.Log("SAVE COMPLETE");
    }

    // =====================================================
    // LOAD
    // =====================================================

    public void LoadGame(int slotID)
    {
        SaveSlotManager.Instance.currentSlotID =slotID;

        Debug.Log("=== LOAD START ===");

        GameData data = SaveSystem.Load(slotID);

        if (data == null)
        {
            Debug.LogError("data NULL");
            return;
        }

        pendingLoadData = data;

        GameStateManager.Instance
            .isLoadingGame = true;

        SceneManager.sceneLoaded +=
            OnSceneLoaded;

        SceneManager.LoadScene(
            data.currentScene
        );
    }

    // =====================================================
    // ON SCENE LOADED
    // =====================================================

    private void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode
    )
    {
        SceneManager.sceneLoaded -=
            OnSceneLoaded;

        GameData data = pendingLoadData;
        PlayerProgression.Instance.player = data.player;

        // =====================
        // LOAD TEAM
        // =====================

        PartyManager.Instance
            .SelectedPlayerIDs =
            new List<int>(
                data.selectedTeamIDs
            );

        PartyManager.Instance
            .BuildPartyFromDB();

        // =====================
        // LOAD EQUIPMENT
        // =====================

        foreach (var equip in data.equipments)
        {
            var member =
                PartyManager.Instance
                .PartyStats
                .Find(
                    c => c.id ==
                    equip.characterID
                );

            if (member != null)
            {
                member.weaponID =
                    equip.weaponID;

                member.armorID =
                    equip.armorID;

                member.accessoryID =
                    equip.accessoryID;
            }
        }

        // =====================
        // LOAD INVENTORY
        // =====================

        InventoryManager.Instance
            .items.Clear();

        foreach (var item in
            data.inventoryItems)
        {
            InventoryManager.Instance
                .AddItem(
                    item.itemID,
                    item.amount
                );
        }
        // =====================
        // LOAD CHESTS
        // =====================

        ChestStateManager.Instance.ResetAll();

        // reset visual all chest
        foreach (var chest in
            FindObjectsByType<LootChest>(
                FindObjectsSortMode.None))
        {
            chest.opened = false;

            chest.RefreshVisual();
        }

        // apply save data
        foreach (var chestData in data.chests)
        {
            foreach (var chest in
                FindObjectsByType<LootChest>(
                    FindObjectsSortMode.None))
            {
                if (chest.chestID ==
                    chestData.chestID)
                {
                    chest.opened =
                        chestData.opened;

                    if (chest.opened)
                    {
                        ChestStateManager.Instance
                            .MarkOpened(
                                chest.chestID
                            );
                    }

                    chest.RefreshVisual();

                    Debug.Log(
                        $"LOAD CHEST {chest.chestID} opened={chest.opened}"
                    );
                }
            }
        }
        // =====================
        // LOAD ENCOUNTERS
        // =====================

        EncounterStateManager.Instance
            .ResetAll();

        foreach (var e in data.encounters)
        {
            foreach (var id in e.defeatedIDs)
            {
                EncounterStateManager.Instance
                    .MarkDefeated(
                        e.chunkID,
                        id
                    );
            }
        }
        // =====================
        // LOAD QUESTS
        // =====================

        QuestManager.Instance
            .activeQuests.Clear();

        foreach (var q in data.quests)
        {
            QuestManager.Instance
                .LoadQuestRuntime(q);
        }
        QuestTrackerUI.Instance.Refresh();
        if (QuestLogUI.Instance != null)
        {
            QuestLogUI.Instance.Refresh();
        }

        StoryFlagManager.Instance.LoadFlags
        (
            data.storyFlags
        );

        // =====================
        // LOAD PLAYER POSITION
        // =====================

        //GameObject player =
        //    GameObject.FindGameObjectWithTag(
        //        "Player"
        //    );

        //if (player != null)
        //{
        //    player.transform.position =
        //        new Vector3(
        //            data.player.posX,
        //            data.player.posY,
        //            data.player.posZ
        //        );

        //    Physics2D.SyncTransforms();
        //}

        // delay 1 frame
        //Invoke(nameof(FinishLoad), 0.1f);
    }

    // =====================================================
    // FINISH LOAD
    // =====================================================

    //private void FinishLoad()
    //{
    //    GameStateManager.Instance
    //        .isLoadingGame = false;

    //    Debug.Log("LOAD COMPLETE");
    //}
}