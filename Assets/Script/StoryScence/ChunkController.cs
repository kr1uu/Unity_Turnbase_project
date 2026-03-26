using UnityEngine;

public class ChunkController : MonoBehaviour
{
    public string chunkID;

    private void OnEnable()
    {
        Debug.Log($"[Chunk Loaded] {chunkID}");
        
    }

    private void OnDisable()
    {
        Debug.Log($"[Chunk Unloaded] {chunkID}");
        
        EncounterStateManager.Instance.ResetChunk(chunkID);
    }
}
