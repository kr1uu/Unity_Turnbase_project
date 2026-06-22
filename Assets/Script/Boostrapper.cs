using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    [SerializeField]
    private GameObject managerPrefab;

    void Awake()
    {
        if (PartyManager.Instance == null)
        {
            Instantiate(managerPrefab);
        }
    }
}