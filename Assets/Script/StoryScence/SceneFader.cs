using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance;   // Singleton
    public float fadeDuration = 1f;

    private float alpha = 1f;
    private bool isFadingIn = true;
    private bool isFadingOut = false;
    private string nextScene = "";

    void Awake()
    {
        // ??m b?o ch? có m?t SceneFader t?n t?i
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // gi? l?i khi load scene m?i
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnGUI()
    {
        if (alpha > 0)
        {
            GUI.color = new Color(0, 0, 0, alpha);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        }
    }

    void Update()
    {
        if (isFadingIn)
        {
            alpha -= Time.deltaTime / fadeDuration;
            if (alpha <= 0)
            {
                alpha = 0;
                isFadingIn = false;
            }
        }

        if (isFadingOut)
        {
            alpha += Time.deltaTime / fadeDuration;
            if (alpha >= 1)
            {
                alpha = 1;
                isFadingOut = false;
                SceneManager.LoadScene(nextScene);
                isFadingIn = true; // khi scene m?i load thì fade in
            }
        }
    }

    public void FadeToScene(string sceneName)
    {
        nextScene = sceneName;
        isFadingOut = true;
    }
}