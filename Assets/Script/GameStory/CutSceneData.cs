using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    menuName = "RPG/Cutscene"
)]
public class CutsceneData :
    ScriptableObject
{
    public List<CutsceneAction> actions =
        new();
}