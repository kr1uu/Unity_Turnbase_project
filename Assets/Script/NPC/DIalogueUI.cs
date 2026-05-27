using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance;

    [Header("UI")]
    public GameObject panel;

    public TMP_Text speakerText;
    public TMP_Text contentText;
    private System.Action onFinish;
    private List<DialogueData> lines;

    private int currentIndex = 0;
    private float inputDelay;

    // =====================================================
    // AWAKE
    // =====================================================

    void Awake()
    {
        Instance = this;

        panel.SetActive(false);
    }

    // =====================================================
    // SHOW
    // =====================================================

    public void Show(List<DialogueData> dialogueLines,System.Action finishAction = null)
    {
        lines = dialogueLines;

        currentIndex = 0;

        onFinish = finishAction;

        panel.SetActive(true);

        inputDelay = 0.15f;

        ShowCurrentLine();
    }
    // =====================================================
    // SHOW CURRENT LINE
    // =====================================================

    void ShowCurrentLine()
    {
        if (lines == null || lines.Count == 0)
            return;

        var line = lines[currentIndex];

        speakerText.text =
            line.speaker_name;

        contentText.text =
            line.content;
    }

    // =====================================================
    // NEXT
    // =====================================================

    public void Next()
    {
        currentIndex++;

        if (currentIndex >= lines.Count)
        {
            Close();
            return;
        }

        ShowCurrentLine();
    }

    // =====================================================
    // CLOSE
    // =====================================================

    public void Close()
    {
        panel.SetActive(false);

        FindFirstObjectByType<PlayerInteract>() ?.BlockInteract(0.2f);

        onFinish?.Invoke();

        onFinish = null;
    }

    // =====================================================
    // UPDATE
    // =====================================================

    void Update()
    {
        if (!panel.activeSelf)
            return;

        inputDelay -= Time.deltaTime;

        if (inputDelay > 0f)
            return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyUp(KeyCode.E))
        {
            Next();
        }
    }
    public bool IsOpen()
    {
        return panel.activeSelf;
    }
}