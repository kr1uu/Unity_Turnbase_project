using System.Collections.Generic;
using UnityEngine;

public class ChestStateManager : MonoBehaviour
{
    public static ChestStateManager Instance;

    private HashSet<string> opened =
        new HashSet<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void MarkOpened(string id)
    {
        opened.Add(id);
    }

    public bool IsOpened(string id)
    {
        return opened.Contains(id);
    }
    public void ResetAll()
    {
        opened.Clear();

        Debug.Log("RESET ALL CHESTS");
    }
}