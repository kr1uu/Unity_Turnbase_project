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
        Vector3 spawnPos = defaultSpawnPoint.position;

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
