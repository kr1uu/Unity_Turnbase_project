using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "IntroDatabase",
    menuName = "RPG/Intro Database"
)]
public class IntroDatabase : ScriptableObject
{
    public List<IntroData> intros =
        new List<IntroData>();

    public IntroData GetIntro(
        int starterID
    )
    {
        return intros.Find(
            x => x.starterID == starterID
        );
    }
}