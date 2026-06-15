using UnityEngine;

public class ActiveDebug : MonoBehaviour
{
    private void OnEnable()
    {
        Debug.Log($"{name} ENABLED");
    }

    private void OnDisable()
    {
        Debug.Log($"{name} DISABLED");
        Debug.Log(System.Environment.StackTrace);
    }
}