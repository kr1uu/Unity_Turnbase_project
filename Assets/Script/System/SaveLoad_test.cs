using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadUI : MonoBehaviour
{
    public PlayerState player;

    void Awake()
    {
        // Fake data ?? test (sau này thay b?ng GameManager)
        player = new PlayerState();
        player.characterId = 1;
        player.level = 99;
        player.exp = 100;
        player.currentHP = 80;
    }

    public void SaveGame()
    {
        GameData data = new GameData();

        data.player = player;
        data.currentScene = SceneManager.GetActiveScene().name;
        data.encounters = new List<EncounterSaveData>();
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        data.player.posX = playerTransform.position.x;
        data.player.posY = playerTransform.position.y;
        data.player.posZ = playerTransform.position.z;

        foreach (var kvp in EncounterStateManager.Instance.GetAll())
        {
            EncounterSaveData e = new EncounterSaveData();
            e.chunkID = kvp.Key;
            e.defeatedIDs = new List<string>(kvp.Value);

            data.encounters.Add(e);
        }
        SaveSystem.Save(data);
    }

    public void LoadGame()
    {
        Debug.Log("=== LOAD START ===");

        GameData data = SaveSystem.Load();

        Debug.Log("data = " + data);

        if (data == null)
        {
            Debug.LogError("? data NULL");
            return;
        }

        Debug.Log("GameStateManager = " + GameStateManager.Instance);

        if (GameStateManager.Instance == null)
        {
            Debug.LogError("? GameStateManager NULL");
        }

        Debug.Log("Scene = " + data.currentScene);

        GameStateManager.Instance.isLoadingGame = true;

        UnityEngine.SceneManagement.SceneManager.LoadScene(data.currentScene);
    }
}