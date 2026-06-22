using SQLite4Unity3d;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using UnityEngine.UI;
using static DialogueUI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class IntroManager : MonoBehaviour
{
    [Header("Intro Data")]
    public List<IntroData> intros;

    [Header("UI")]
    public Image backgroundImage;

    [Header("Story Image")]
    public Image storyImage;

    public float fadeDuration = 0.2f;

    public float delayBetweenImages = 0.2f;

    public Button playButton;

    private Dictionary<int, List<DialogueData>> dialogCache = new Dictionary<int, List<DialogueData>>();

    [Header("Scene")]
    public string nextSceneName = "StoryScene";

    private IntroData currentIntro;

    private void Start()
    {
        LoadDialogueCache();
        storyImage.gameObject.SetActive(false);

        playButton.gameObject.SetActive(false);

        int starterID =
            PlayerPrefs.GetInt(
                "StarterID",
                1
            );

        Debug.Log($"Loaded StarterID = {starterID}");
        currentIntro =
            intros.FirstOrDefault(
                x => x.starterID ==
                starterID
            );

        if (currentIntro == null)
        {
            Debug.LogError(
                $"No IntroData found for StarterID {starterID}"
            );

            playButton.gameObject.SetActive(true);
            return;
        }

        StartCoroutine(
            PlayIntro()
        );
    }

    IEnumerator PlayIntro()
    {
        foreach (
            var page in currentIntro.pages
        )
        {
            yield return ShowPage(
                page
            );
        }

        playButton.gameObject.SetActive(true);
    }
    void LoadDialogueCache()
    {
        string dbPath =
            System.IO.Path.Combine(
                Application.streamingAssetsPath,
                "Datagame.db"
            );

        using (
            SQLiteConnection db =
            new SQLiteConnection(
                dbPath,
                SQLiteOpenFlags.ReadOnly
            )
        )
        {
            var allDialogue =
                db.Table<DialogueData>()
                .ToList();

            dialogCache =
                allDialogue
                .GroupBy(
                    x => x.group_id
                )
                .ToDictionary(
                    g => g.Key,
                    g => g
                        .OrderBy(
                            x => x.line_order
                        )
                        .ToList()
                );
        }

        Debug.Log(
            $"Loaded {dialogCache.Count} groups"
        );
    }
    IEnumerator ShowPage(
     IntroPage page
 )
    {
        foreach (var image in page.images)
        {
            yield return ShowImage(image);
        }
    }
    // =====================================================
    //fade in/out
    // =====================================================
    IEnumerator FadeImage(
    CanvasGroup group
)
    {
        float t = 0;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;

            group.alpha =
                Mathf.Lerp(
                    0,
                    1,
                    t / fadeDuration
                );

            yield return null;
        }

        group.alpha = 1;
    }
    IEnumerator FadeOutImage(
    CanvasGroup group
)
    {
        float t = 0;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;

            group.alpha =
                Mathf.Lerp(
                    1,
                    0,
                    t / fadeDuration
                );

            yield return null;
        }

        group.alpha = 0;
    }
    // =====================================================
    //show image
    // =====================================================
    IEnumerator ShowImage(
     IntroImage imageData
 )
    {
        storyImage.sprite =
            imageData.sprite;

        storyImage.gameObject
            .SetActive(true);

        CanvasGroup group =
            storyImage.GetComponent<CanvasGroup>();

        Coroutine fadeRoutine = null;

        if (group != null)
        {
            group.alpha = 0;

            fadeRoutine =
                StartCoroutine(
                    FadeImage(group)
                );

            yield return new WaitUntil(
                () => group.alpha >= 0.4f
            );
        }

        yield return ShowDialogue(
            imageData.dialogueGroupID
        );

        if (fadeRoutine != null)
        {
            yield return fadeRoutine;
        }

        if (group != null)
        {
            yield return FadeOutImage(
                group
            );
        }

        yield return new WaitForSeconds(
            imageData.delay
        );
    }
    void ClearImages()
    {
            storyImage.gameObject.SetActive(false);

            CanvasGroup group =
                storyImage.GetComponent<CanvasGroup>();

            if (group != null)
            {
                group.alpha = 0;
            }
    }
    // =====================================================
    //show dialogue
    // =====================================================
    IEnumerator ShowDialogue(
     int groupID
 )
    {
        if (
            !dialogCache.TryGetValue(
                groupID,
                out var lines
            )
        )
        {
            Debug.LogWarning(
                $"Dialogue Group {groupID} not found."
            );

            yield break;
        }

        bool finished = false;

        DialogueUI.Instance.Show(
            lines,
            () =>
            {
                finished = true;
            },
            DialogueMode.Narrator
        );

        yield return new WaitUntil(
            () => finished
        );
    }
    public void StartGame()
    {
        SceneManager.LoadScene("StoryScene");
    }
}