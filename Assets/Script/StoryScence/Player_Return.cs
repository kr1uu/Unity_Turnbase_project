using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerReturnHandler : MonoBehaviour
{
    void Start()
    {
        if (SceneManager.GetActiveScene().name != "StoryScene") return;
        if (PlayerPosition.Instance == null) return;

        transform.position = PlayerPosition.Instance.returnPosition;
    }
}
