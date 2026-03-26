using UnityEngine;

public class EncounterMarker : MonoBehaviour
{
    public string fromChunkID;
    public string toChunkID;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("TRIGGER HIT: " + other.name);
        if (!other.CompareTag("Player")) return;

        var playerPos = PlayerPosition.Instance;
        var encounterState = EncounterStateManager.Instance;

        if (string.IsNullOrEmpty(playerPos.currentChunkID))
        {
            playerPos.currentChunkID = toChunkID;
            Debug.Log($"[Chunk Init] currentChunk = {toChunkID}");
            return;
        }

        if (playerPos.currentChunkID != toChunkID)
        {
            Debug.Log($"[Chunk Change] {playerPos.currentChunkID} ? {toChunkID}");

            // reset encounter c?a chunk c?
            encounterState.ResetChunk(playerPos.currentChunkID);

            playerPos.currentChunkID = toChunkID;
        }
    }
}
