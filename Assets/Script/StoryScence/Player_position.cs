using UnityEngine;

public class PlayerPosition : MonoBehaviour
{
    public static PlayerPosition Instance;

    public Vector3 returnPosition;
    public string returnChunkID;
    internal string currentChunkID;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
}
