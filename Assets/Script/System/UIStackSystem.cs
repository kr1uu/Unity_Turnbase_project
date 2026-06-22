using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private Stack<GameObject> uiStack = new Stack<GameObject>();

    void Awake()
    {
        Instance = this;
    }

    public void Push(GameObject panel)
    {
        if (uiStack.Count > 0)
        {
            uiStack.Peek().SetActive(false);
        }

        panel.SetActive(true);
        uiStack.Push(panel);

        Time.timeScale = 0f;
    }

    public void Pop()
    {
        if (uiStack.Count == 0) return;

        GameObject top = uiStack.Pop();
        top.SetActive(false);

        if (uiStack.Count > 0)
        {
            uiStack.Peek().SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}
