using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class PlayerState
{
    public int characterId;

    public int level = 1;

    public int currentExp = 0;

    public int gold = 0;

    public int currentHP;
    public float posX;
    public float posY;
    public float posZ;

    public List<int> unlockedCharacters =
    new List<int>();
}