using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SQLite4Unity3d;
using System.IO;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    private SQLiteConnection db;

    public List<QuestRuntime> activeQuests =
        new();

    // =====================================================
    // AWAKE
    // =====================================================

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);

            return;
        }

        // =========================
        // CONNECT DATABASE
        // =========================

        string dbPath =
            Path.Combine(
                Application.streamingAssetsPath,
                "Datagame.db"
            );

        db = new SQLiteConnection(
            dbPath,
            SQLiteOpenFlags.ReadOnly
        );
    }

    // =====================================================
    // START QUEST
    // =====================================================

    public void StartQuest(int questID)
    {

        bool alreadyHave =
            activeQuests.Exists(
                q => q.questID == questID
            );

        if (alreadyHave)
        {
            Debug.Log(
                "Quest already active"
            );

            return;
        }

        // LOAD DATA FROM DB
        QuestData data =
            db.Table<QuestData>()
            .FirstOrDefault(
                x => x.id == questID
            );

        if (data == null)
        {
            Debug.LogError(
                $"QUEST NOT FOUND: {questID}"
            );

            return;
        }

        // CREATE RUNTIME
        QuestRuntime q =
            new QuestRuntime();

        q.questID = questID;

        q.data = data;

        q.currentAmount = 0;

        q.completed = false;

        q.rewarded = false;

        activeQuests.Add(q);

        if (QuestTrackerUI.Instance != null)
        {
            QuestTrackerUI.Instance.TrackQuest(q);
        }

        Debug.Log(
            $"START QUEST: {data.quest_name}"
        );
    }
    // =====================================================
    // GET QUEST
    // =====================================================

    public QuestRuntime GetQuest(
        int questID
    )
    {
        return activeQuests.Find(
            q => q.questID == questID
        );
    }

    // =====================================================
    // COMPLETE
    // =====================================================
    public void AddProgress(
     string objectiveType,
     int targetID,
     int amount = 1
 )
    {
        foreach (var q in activeQuests)
        {
            Debug.Log(
                $"QUEST CHECK | " +
                $"QuestTarget={q.data.target_id} | " +
                $"EnemyKilled={targetID}"
            );

            if (q.completed)
                continue;

            if (q.data.quest_type != objectiveType)
                continue;

            if (q.data.target_id != targetID)
                continue;

            q.currentAmount += amount;

            if (QuestLogUI.Instance != null)
            {
                QuestLogUI.Instance.Refresh();
            }

            if (QuestTrackerUI.Instance != null)
            {
                QuestTrackerUI.Instance.Refresh();
            }

            Debug.Log(
                $"QUEST UPDATE: " +
                $"{q.data.quest_name} " +
                $"{q.currentAmount}/" +
                $"{q.data.required_amount}"
            );
            if (
                q.currentAmount >=
                q.data.required_amount
            )
            {
                q.completed = true;
                q.state = QuestState.Completed;

                //RewardQuest(q.questID);

                Debug.Log(
                    $"QUEST COMPLETE: " +
                    q.data.quest_name
                );
            }
        }
    }
    public void CompleteQuest(
        int questID
    )
    {
        QuestRuntime q =
            GetQuest(questID);

        if (q == null)
            return;

        q.completed = true;

        Debug.Log(
            $"QUEST COMPLETE {questID}"
        );
    }
    public void RewardQuest(int questID)
    {
        QuestRuntime q =
            GetQuest(questID);

        if (q == null)
            return;

        // Undone
        if (!q.completed)
        {
            Debug.Log(
                "Quest not completed"
            );

            return;
        }

        // already rewarded
        if (q.rewarded)
        {
            Debug.Log(
                "Quest already rewarded"
            );

            return;
        }

        // =====================
        // GIVE REWARD
        // =====================

        PlayerProgression.Instance.AddGold(
            q.data.reward_gold
        );

        PlayerProgression.Instance.AddEXP(
            q.data.reward_exp
        );

        // item reward
        if (q.data.reward_item_id > 0)
        {
            InventoryManager.Instance.AddItem(
                q.data.reward_item_id,
                q.data.reward_item_amount
            );
        }
        if (q.data.story_flag_on_complete > 0)
        {
            StoryFlagManager.Instance.SetFlag(
                q.data.story_flag_on_complete
            );
        }
        q.rewarded = true;
        q.state = QuestState.Rewarded;

        Debug.Log(
            $"QUEST REWARDED: " +
            q.data.quest_name
        );
    }
    public void LoadQuestRuntime(
    QuestSaveData save
)
    {
        QuestData data =
            db.Table<QuestData>()
            .FirstOrDefault(
                x => x.id ==
                save.questID
            );

        if (data == null)
        {
            Debug.LogError(
                $"QUEST NOT FOUND {save.questID}"
            );

            return;
        }

        QuestRuntime q =
            new QuestRuntime();

        q.questID =
            save.questID;

        q.data = data;

        q.currentAmount =
            save.currentAmount;

        q.completed =
            save.completed;

        q.rewarded =
            save.rewarded;

        q.state =
            (QuestState)save.state;

        activeQuests.Add(q);

        Debug.Log(
            $"LOAD QUEST: {q.data.quest_name}"
        );
    }
    public bool HasQuest(int id)
    {
        return activeQuests.Exists(
            q => q.questID == id
        );
    }

    public bool IsQuestCompleted(int id)
    {
        QuestRuntime q = GetQuest(id);

        return q != null && q.completed;
    }

    public bool IsQuestRewarded(int id)
    {
        QuestRuntime q = GetQuest(id);

        return q != null && q.rewarded;
    }
}