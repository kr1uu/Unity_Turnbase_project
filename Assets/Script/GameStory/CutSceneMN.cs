using System.Collections;
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

    IEnumerator ExecuteAction(
        CutsceneAction action
    )
    {
        switch (action.type)
        {
            case CutsceneActionType.Wait:

                yield return new WaitForSeconds(
                    action.duration
                );

                break;

            case CutsceneActionType.MoveObject:

                GameObject obj =
                    GameObject.Find(
                        action.targetName
                    );

                if (obj != null)
                {
                    yield return MoveTo(
                        obj,
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
                    GameObject.Find(
                        action.targetName
                    );

                if (enableObj != null)
                {
                    enableObj.SetActive(true);
                }

                break;

            case CutsceneActionType.DisableObject:

                GameObject disableObj =
                    GameObject.Find(
                        action.targetName
                    );

                if (disableObj != null)
                {
                    disableObj.SetActive(false);
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
}