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
            GameData data = SaveSystem.Load(SaveSlotManager.Instance.currentSlotID);

            if (data != null && data.player != null)
            {
                spawnPos = new Vector3(
                    data.player.posX,
                    data.player.posY,
                    data.player.posZ
                );
            }
            else
            {
                spawnPos =
                    PlayerSpawnManager
                    .Instance
                    .GetSpawnPosition();
            }
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
        Debug.Log("FINAL SPAWN POS = " +spawnPos);
        transform.position = spawnPos;

        Physics2D.SyncTransforms();

        yield return new WaitForEndOfFrame();

        GameStateManager.Instance
            .isLoadingGame = false;

        Debug.Log("LOAD COMPLETE");
        UpdateCameraZone();

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
    void UpdateCameraZone()
    {
        CameraFollow cam =
            Camera.main.GetComponent<CameraFollow>();

        if (cam == null)
            return;

        Collider2D hit =
            Physics2D.OverlapPoint(
                transform.position,
                LayerMask.GetMask("CameraZone")
            );

        if (hit != null)
        {
            BoxCollider2D box =
                hit.GetComponent<BoxCollider2D>();

            if (box != null)
            {
                cam.cameraBounds = box;
                cam.UpdateBounds();

                Debug.Log(
                    "Camera Zone Updated"
                );
            }
        }
    }
}
