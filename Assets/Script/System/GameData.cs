using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class GameData
{
    public PlayerState player = new();
    public string currentScene;

    public List<EncounterSaveData> encounters = new();
    public List<int> selectedTeamIDs = new();

    public List<InventoryItemSaveData> inventoryItems = new();

    public List<CharacterEquipmentSaveData> equipments = new();

    public List<ChestSaveData> chests = new();

    public List<string> storyFlags = new();

    public List<QuestSaveData> quests = new();
}
