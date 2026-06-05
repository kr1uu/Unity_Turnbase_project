using SQLite4Unity3d;
using UnityEngine;
using System.Linq;

public class NPCInteract :
    MonoBehaviour,
    IInteractable
{
    public int npcID;

    private SQLiteConnection db;

    private NPCData npcData;

    // =====================================================
    // START
    // =====================================================

    void Start()
    {
        string dbPath =
            System.IO.Path.Combine(
                Application.streamingAssetsPath,
                "Datagame.db"
            );

        db = new SQLiteConnection(
            dbPath,
            SQLiteOpenFlags.ReadOnly
        );

        npcData =
            db.Table<NPCData>()
            .FirstOrDefault(
                x => x.id == npcID
            );
    }

    // =====================================================
    // OPEN DIALOGUE
    // =====================================================

    void OpenDialogue(
     System.Action onFinish = null
 )
    {
            int groupID = ResolveDialogueGroup();
             var lines =
            db.Table<DialogueData>()
            .Where(
                x => x.group_id == groupID
            )
            .OrderBy(
                x => x.line_order
            )
            .ToList();
        DialogueUI.Instance
            .Show(lines, onFinish);
    }

    // =====================================================
    // GET DIALOGUE
    // =====================================================

    private System.Collections.Generic.List<DialogueData>
        GetDialogueLines()
    {
        return db.Table<DialogueData>()
            .Where(
                x =>
                x.group_id ==
                npcData.dialogue_group_id
            )
            .OrderBy(
                x => x.line_order
            )
            .ToList();
    }

    // =====================================================
    // OPEN SHOP
    // =====================================================

    void OpenShop()
    {
        var lines =
            GetDialogueLines();

        ShopData shop =
            db.Table<ShopData>()
            .FirstOrDefault(
                x => x.id ==
                npcData.shop_id
            );

        DialogueUI.Instance.Show(
            lines,
            () =>
            {
                if (shop != null)
                {
                    ShopUI.Instance.Open(shop);
                }
                else
                {
                    Debug.LogError(
                        "SHOP NULL"
                    );
                }
            }
        );

        Debug.Log(
            $"OPEN SHOP : {npcData.npc_type}"
        );
    }

    // =====================================================
    // INTERACT
    // =====================================================

    public void Interact()
    {
        if (npcData == null)
        {
            Debug.LogError("NPC NULL");
            return;
        }

        switch (npcData.npc_type)
        {
            // =====================================
            // NORMAL DIALOGUE
            // =====================================

            case "dialogue":

                OpenDialogue();

                break;

            // =====================================
            // ALL SHOP TYPES
            // =====================================

            case "Blacksmith":
            case "Armorsmith":
            case "Innkeeper":
            case "Mystery":

                OpenShop();

                break;

            // =====================================
            // QUEST
            // =====================================
            case "Quest":

                int questID = npcData.quest_id;

                // =========================
                // QUEST NOT STARTED
                // =========================

                if (
                    !QuestManager.Instance
                    .HasQuest(questID)
                )
                {
                    OpenDialogue(() =>
                    {
                        QuestManager.Instance
                            .StartQuest(questID);

                        Debug.Log(
                            "START QUEST"
                        );
                    });

                    return;
                }

                // =========================
                // QUEST COMPLETED
                // =========================

                if (
                    QuestManager.Instance
                    .IsQuestCompleted(questID)
                    &&
                    !QuestManager.Instance
                    .IsQuestRewarded(questID)
                )
                {
                    OpenDialogue(() =>
                    {
                        QuestManager.Instance
                            .RewardQuest(questID);

                        Debug.Log(
                            "QUEST REWARDED"
                        );
                    });

                    return;
                }

                // =========================
                // QUEST ALREADY REWARDED
                // =========================

                if (
                    QuestManager.Instance
                    .IsQuestRewarded(questID)
                )
                {
                    OpenDialogue();

                    Debug.Log(
                        "QUEST ALREADY DONE"
                    );

                    return;
                }

                // =========================
                // QUEST IN PROGRESS
                // =========================

                OpenDialogue();

                Debug.Log(
                    "QUEST IN PROGRESS"
                );

                break;
            // =====================================
            // Boss NPC
            // =====================================

            case "BossNPC":

                OpenDialogue();

                Debug.Log(
                    "Encounter BossNPC"
                );

                break;

            // =====================================
            // DEFAULT
            // =====================================

            default:

                OpenDialogue();

                break;
        }

        Debug.Log(
            $"Interact NPC: {npcData.npc_name}"
        );
    }
    int ResolveDialogueGroup()
    {
        var conditions =
            db.Table<NPCDialogueCondition>()
            .Where(x => x.npc_id == npcID)
            .OrderByDescending(x => x.priority)
            .ToList();
        //foreach (var e in FindObjectsByType <ConditionalEncounter>( FindObjectsSortMode.None ) )
        //{
        //    e.Refresh();
        //}
        foreach (var c in conditions)
        {
            bool valid = true;

            // =====================
            // FLAG CHECK
            // =====================

            if (c.required_flag > 0)
            {
                if (!StoryFlagManager.Instance
                    .HasFlag(c.required_flag))
                {
                    valid = false;
                }
            }

            // =====================
            // QUEST CHECK
            // =====================

            if (
                c.required_quest_id > 0
            )
            {
                QuestRuntime q =
                    QuestManager.Instance
                    .GetQuest(
                        c.required_quest_id
                    );

                if (q == null)
                {
                    valid = false;
                }
                else
                {
                    switch (
                        c.required_quest_state
                    )
                    {
                        case "InProgress":

                            if (q.completed)
                                valid = false;

                            break;

                        case "Completed":

                            if (!q.completed ||
                                q.rewarded)
                                valid = false;

                            break;

                        case "Rewarded":

                            if (!q.rewarded)
                                valid = false;

                            break;
                    }
                }
            }

            // =====================
            // SUCCESS
            // =====================

            if (valid)
            {
                return c.dialogue_group_id;
            }
        }

        // fallback
        return npcData.dialogue_group_id;
    }
}