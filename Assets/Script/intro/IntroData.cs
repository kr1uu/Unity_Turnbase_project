using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "IntroData",
    menuName = "RPG/Intro Data"
)]
public class IntroData : ScriptableObject
{
    public int starterID;

    public string introName;

    public List<IntroPage> pages =
        new List<IntroPage>();
}