using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerSpawnHandler : MonoBehaviour
{
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(SpawnAfterSceneReady());
    }

    IEnumerator SpawnAfterSceneReady()
    {
        yield return null; // frame 1
        yield return null; // frame 2 

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        PlayerMovement pm = GetComponent<PlayerMovement>();

        rb.simulated = false;

        Vector3 spawnPos;
        if (GameStateManager.Instance != null &&
            GameStateManager.Instance.isLoadingGame)
        {
            GameData data = SaveSystem.Load();

            spawnPos = new Vector3(
                data.player.posX,
                data.player.posY,
                data.player.posZ
            );

            Debug.Log("?? Spawn from SAVE DATA");
        }
        // Back from Battle Priority
        else if (PlayerPosition.Instance != null &&
            PlayerPosition.Instance.returnPosition != Vector3.zero)
        {
            spawnPos = PlayerPosition.Instance.returnPosition;
        }
        else
        {
            // back to spwan default
            spawnPos = PlayerSpawnManager.Instance.GetSpawnPosition();
        }
        transform.position = spawnPos;

        yield return new WaitForFixedUpdate(); 

        rb.position = spawnPos;
        rb.linearVelocity = Vector2.zero;
        rb.simulated = true;

        pm.canMove = true;

        Debug.Log("Spawn FINAL (scene ready) at: " + spawnPos);
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.isLoadingGame = false;
        }
    }
}
