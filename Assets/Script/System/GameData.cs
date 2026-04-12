using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class GameData
{
    public PlayerState player;
    public string currentScene;
    // Placeholder cho inventory
    public List<string> inventoryItems = new List<string>();
    public List<EncounterSaveData> encounters;
}
