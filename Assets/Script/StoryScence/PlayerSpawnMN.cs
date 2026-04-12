using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    public static PlayerSpawnManager Instance;

    public Transform defaultSpawnPoint;

    void Awake()
    {
        Instance = this;
    }

    public void SpawnPlayer(GameObject player)
    {
        //    if (GameStateManager.Instance != null &&
        //    GameStateManager.Instance.isLoadingGame)
        //{
        //    Debug.Log("? Skip spawn (loading game)");
        //    return;
        //}
        Vector3 spawnPos;

        // ?? ?U TIÊN 1: SAVE DATA
        GameData data = SaveSystem.Load();

        if (data != null && data.player != null)
        {
            spawnPos = new Vector3(
                data.player.posX,
                data.player.posY,
                data.player.posZ
            );

            Debug.Log("?? Spawn from SAVE DATA");
        }
        else if (PlayerPosition.Instance != null &&
                 PlayerPosition.Instance.returnPosition != Vector3.zero)
        {
            // ?? ?U TIÊN 2: BATTLE RETURN
            spawnPos = PlayerPosition.Instance.returnPosition;

            Debug.Log("? Spawn from BATTLE RETURN");
        }
        else
        {
            // ?? ?U TIÊN 3: DEFAULT
            spawnPos = PlayerSpawnManager.Instance.GetSpawnPosition();

            Debug.Log("?? Spawn from DEFAULT");
        }
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.position = spawnPos;
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            player.transform.position = spawnPos;
        }

        Debug.Log("Spawned player at: " + spawnPos);
    }
    public Vector3 GetSpawnPosition()
    {
        if (defaultSpawnPoint == null)
        {
            Debug.LogError("? DefaultSpawnPoint IS NULL");
            return Vector3.zero;
        }

        Debug.Log("Using spawn point: " + defaultSpawnPoint.position);
        return defaultSpawnPoint.position;
    }

}
