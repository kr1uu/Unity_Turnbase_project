using UnityEngine;
using UnityEngine.SceneManagement;

public class WinSceneUI : MonoBehaviour
{
    
    public void OnNextButton()
    {
        if (BattleEncounterData.Instance != null)
        {
            Debug.Log($"[WIN] returnChunkID = '{PlayerPosition.Instance.returnChunkID}'");
            Debug.Log($"[WIN] encounterID   = '{BattleEncounterData.Instance.LastEncounterID}'");

            EncounterStateManager.Instance.MarkDefeated(
                PlayerPosition.Instance.returnChunkID,
                BattleEncounterData.Instance.LastEncounterID
            );


        }

        SceneManager.LoadScene("StoryScene");
    }
    public void OnRetryButton()
    {
        Debug.Log("Retry Button clicked!");

        SceneManager.LoadScene("BattleScene");
    }
}