using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject starterPanel;

    public void NewGame()
    {
        starterPanel.SetActive(true);
    }

    public void ContinueGame()
    {
        SaveSlotPanelUI.Instance.OpenFromMainMenu();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}