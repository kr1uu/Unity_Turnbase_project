using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CutsceneManager :
    MonoBehaviour
{
    public static CutsceneManager Instance;

    private bool playing = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Play(CutsceneData data)
    {
        if (playing)
            return;

        StartCoroutine(
            PlayRoutine(data)
        );
    }

    IEnumerator ShowDialogue(
    int groupID
)
    {
        string dbPath =
            System.IO.Path.Combine(
                Application.streamingAssetsPath,
                "Datagame.db"
            );

        using SQLite4Unity3d.SQLiteConnection db =
            new SQLite4Unity3d.SQLiteConnection(
                dbPath,
                SQLite4Unity3d.SQLiteOpenFlags.ReadOnly
            );

        var lines =
            db.Table<DialogueData>()
            .Where(x =>
                x.group_id == groupID
            )
            .OrderBy(x =>
                x.line_order
            )
            .ToList();

        bool finished = false;

        DialogueUI.Instance.Show(
            lines,
            () =>
            {
                finished = true;
            }
        );

        yield return new WaitUntil(
            () => finished
        );
    }
    IEnumerator PlayRoutine(
        CutsceneData data
    )
    {
        playing = true;

        FindFirstObjectByType<PlayerMovement>()
            .SetControl(false);

        foreach (var action in data.actions)
        {
            yield return ExecuteAction(action);
        }

        FindFirstObjectByType<PlayerMovement>()
            .SetControl(true);

        playing = false;
    }
    GameObject FindTarget(string objectID)
    {
        var target =
            Resources
            .FindObjectsOfTypeAll<SceneObjectID>()
            .FirstOrDefault(x =>
                x.objectID == objectID &&
                x.gameObject.scene.IsValid());

        return target?.gameObject;
    }
    IEnumerator ExecuteAction(
        CutsceneAction action
    )
    {
        switch (action.type)
        {
            case CutsceneActionType.Dialogue:

                yield return ShowDialogue(
                    action.dialogueGroupID
                );

                break;
            case CutsceneActionType.Wait:

                yield return new WaitForSeconds(
                    action.duration
                );

                break;

            case CutsceneActionType.MoveObject:

                GameObject moveObj =
                    FindTarget(
                        action.targetObjectID
                    );

                if (moveObj != null)
                {
                    yield return MoveTo(
                        moveObj,
                        action.targetPosition,
                        action.duration
                    );
                }

                break;

            case CutsceneActionType.SetFlag:

                StoryFlagManager.Instance
                    .SetFlag(
                        action.flagToSet
                    );

                break;

            case CutsceneActionType.EnableObject:

                GameObject enableObj =
                    FindTarget(
                        action.targetObjectID
                    );

                if (enableObj != null)
                {
                    enableObj.SetActive(true);
                }

                break;

            case CutsceneActionType.DisableObject:

                GameObject disableObj =
                    FindTarget(
                        action.targetObjectID
                    );

                if (disableObj != null)
                {
                    disableObj.SetActive(false);
                }

                break;

            case CutsceneActionType.StartQuest:

                QuestManager.Instance
                    .StartQuest(action.questID);

                break;

            case CutsceneActionType.CompleteQuest:

                QuestManager.Instance
                    .CompleteQuest(action.questID);

                break;

            case CutsceneActionType.StartBattle:

                BattleTrigger trigger =
                    EncounterRegistry.Instance
                    .Get(action.encounterID);

                if (trigger != null)
                {
                    BattleEncounterData.Instance
                        .LastEncounterID =
                        trigger.encounterID;

                    BattleEncounterData.Instance
                        .SetEnemies(
                            trigger.selectedEnemyIDs,
                            trigger.enemyRanks
                        );

                    SceneFader.Instance
                        .FadeToScene(
                            "BattleScene"
                        );
                }

                break;
        }
    }

    IEnumerator MoveTo(
        GameObject obj,
        Vector3 target,
        float duration
    )
    {
        Vector3 start =
            obj.transform.position;

        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;

            obj.transform.position =
                Vector3.Lerp(
                    start,
                    target,
                    t / duration
                );

            yield return null;
        }

        obj.transform.position =
            target;
    }
    BattleTrigger FindBattleTrigger(
    string encounterID
)
    {
        foreach (var trigger in
            FindObjectsByType<BattleTrigger>(
                FindObjectsSortMode.None))
        {
            if (trigger.encounterID ==
                encounterID)
            {
                return trigger;
            }
        }

        return null;
    }
    void StartBattle(
    BattleTrigger trigger
)
    {
        BattleEncounterData.Instance
            .LastEncounterID =
            trigger.encounterID;

        PlayerPosition.Instance
            .returnChunkID =
            trigger.chunkID;

        BattleEncounterData.Instance
            .SetEnemies(
                trigger.selectedEnemyIDs,
                trigger.enemyRanks
            );

        SceneFader.Instance
            .FadeToScene(
                "BattleScene"
            );
    }
}