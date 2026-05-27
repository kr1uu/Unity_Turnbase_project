using TMPro;
using UnityEngine;

public class QuestTrackerUI : MonoBehaviour
{
    public static QuestTrackerUI Instance;

    [Header("UI")]
    public GameObject panel;

    public TMP_Text titleText;
    public TMP_Text progressText;

    private QuestRuntime trackedQuest;

    // =====================================================
    // AWAKE
    // =====================================================

    void Awake()
    {
        Instance = this;
    }
    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // =====================================================
    // UPDATE
    // =====================================================

    void Update()
    {
        Refresh();
    }

    // =====================================================
    // SET TRACK
    // =====================================================

    public void TrackQuest(
        QuestRuntime q
    )
    {
        trackedQuest = q;
    }

    // =====================================================
    // REFRESH
    // =====================================================

    public void Refresh()
    {
        // no quest
        if (trackedQuest == null)
        {
            panel.SetActive(false);
            return;
        }

        panel.SetActive(true);

        titleText.text =
            trackedQuest.data.quest_name;

        // complete
        if (trackedQuest.completed)
        {
            progressText.text =
                "COMPLETED";

            return;
        }

        progressText.text =
            trackedQuest.currentAmount +
            " / " +
            trackedQuest.data.required_amount;
    }
}