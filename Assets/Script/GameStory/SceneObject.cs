using System.Collections.Generic;
using UnityEngine;

public class SceneObjectID : MonoBehaviour
{
    public string objectID;
    public static Dictionary<string, SceneObjectID> registry =
    new();
    private void Awake()
    {
        registry[objectID] = this;
    }
    private void OnDestroy()
    {
        if (registry.TryGetValue(
            objectID,
            out var obj) &&
            obj == this)
        {
            registry.Remove(objectID);
        }
    }
}