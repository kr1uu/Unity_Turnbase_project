using TMPro;
using UnityEngine;

public class QuestEntryUI : MonoBehaviour
{
    public TMP_Text titleText;

    private QuestRuntime quest;

    // =====================================================
    // SETUP
    // =====================================================

    public void Setup(QuestRuntime q)
    {
        quest = q;

        titleText.text =
            q.data.quest_name;

        if (q.completed)
        {
            titleText.text +=
                " [DONE]";
        }
    }

    // =====================================================
    // CLICK
    // =====================================================

    public void OnClick()
    {
        QuestLogUI.Instance
            .ShowQuestDetail(quest);
    }
}