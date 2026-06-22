using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;

public class GameStartManager : MonoBehaviour
{
    public static int StarterCharacterID;
    public GameObject starterPanel;

    public void SelectStarter(int characterID)
    {
        StarterCharacterID = characterID;

        PlayerPrefs.SetInt("StarterID", characterID);
        PlayerPrefs.Save();

        Debug.Log(
            $"Saved StarterID = {characterID}"
        );

        StartGame();
    }
    public void back()
    {
        starterPanel.SetActive(false);
    }
    void StartGame()
    {
        PartyManager.Instance.SelectedPlayerIDs.Clear();

        PartyManager.Instance.SelectedPlayerIDs.Add(
            StarterCharacterID
        );
        PlayerProgression.Instance
            .player
            .unlockedCharacters
            .Clear();

        PlayerProgression.Instance
            .player
            .unlockedCharacters
            .Add(StarterCharacterID);

        PartyManager.Instance.BuildPartyFromDB();

        PartyManager.Instance.NotifyPartyChanged();

        SceneManager.LoadScene("IntroScene");
    }
}