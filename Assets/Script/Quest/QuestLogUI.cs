using TMPro;
using UnityEngine;

public class QuestLogUI : MonoBehaviour
{
    public static QuestLogUI Instance;

    [Header("UI")]
    public GameObject panel;

    public Transform content;

    public GameObject entryPrefab;

    [Header("DETAIL")]
    public TMP_Text questNameText;
    public TMP_Text questDescText;
    public TMP_Text questProgressText;
    public TMP_Text questRewardText;

    private bool opened;

    // =====================================================
    // AWAKE
    // =====================================================

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        panel.SetActive(false);
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
        if (Input.GetKeyDown(KeyCode.J))
        {
            Toggle();
        }
    }

    // =====================================================
    // TOGGLE
    // =====================================================

    public void Toggle()
    {
        opened = !opened;

        panel.SetActive(opened);

        if (opened)
        {
            Refresh();
        }
    }

    // =====================================================
    // REFRESH
    // =====================================================

    public void Refresh()
    {
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            Destroy(
                content.GetChild(i).gameObject
            );
        }

        foreach (var q in QuestManager.Instance.activeQuests)
        {
            GameObject go =
                Instantiate(
                    entryPrefab,
                    content
                );

            QuestEntryUI ui =
                go.GetComponent<QuestEntryUI>();

            ui.Setup(q);
        }
        if (QuestManager.Instance.activeQuests.Count > 0)
        {
            ShowQuestDetail(
                QuestManager.Instance.activeQuests[0]
            );
        }
        else
        {
            ClearDetailPanel();
        }
    }

    // =====================================================
    // SHOW DETAIL
    // =====================================================

    public void ShowQuestDetail(
        QuestRuntime q
    )
    {
        questNameText.text =
            q.data.quest_name;

        questDescText.text =
            q.data.description;

        questProgressText.text =
            q.currentAmount +
            " / " +
            q.data.required_amount;

        questRewardText.text =
            $"Gold: {q.data.reward_gold}\n" +
            $"EXP: {q.data.reward_exp}";
    }
    public void ClearDetailPanel()
    {
        questNameText.text = "No Quest";
        questDescText.text = "";
        questProgressText.text = "";
        questRewardText.text = "";
    }
}