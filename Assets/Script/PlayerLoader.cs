using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLoader : MonoBehaviour
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
        GameData data = SaveSystem.Load(SaveSlotManager.Instance.currentSlotID);
        if (data == null) return;

        Vector3 pos = new Vector3(
            data.player.posX,
            data.player.posY,
            data.player.posZ
        );

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.position = pos;
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            transform.position = pos;
        }

        Debug.Log("?? FORCE LOAD POSITION: " + pos);

        // reset flag
        GameStateManager.Instance.isLoadingGame = false;
    }
}