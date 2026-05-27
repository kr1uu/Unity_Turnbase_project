using System;
using UnityEngine;

[Serializable]
public class LootEntry
{
    public int itemID;

    [Range(0f, 100f)]
    public float dropChance = 100f;

    public int minAmount = 1;
    public int maxAmount = 1;
}