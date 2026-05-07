using System;
using System.Collections.Generic;
using UnityEngine;
using static CharacterStats;

[System.Serializable]
public class CharacterStats
{
    // =====================================================
    // EQUIPMENT IDs
    // =====================================================

    public int weaponID = -1;
    public int armorID = -1;
    public int accessoryID = -1;

    // =====================================================
    // BASE STATS
    // =====================================================

    public int id;
    public int faction_id;

    public string name;

    public int maxHP;
    public int currentHP;

    public int attack;
    public int defense;
    public int speed;

    public int ai_profile_id;

    // =====================================================
    // DEFEND
    // =====================================================

    public bool isDefending = false;

    public float defenseMultiplier = 0.7f;

    // =====================================================
    // CONSTRUCTOR
    // =====================================================

    public CharacterStats(
        int id,
        int faction_id,
        string name,
        int hp,
        int atk,
        int def,
        int spd,
        int ai_profile_id
    )
    {
        this.id = id;
        this.faction_id = faction_id;

        this.name = name;

        maxHP = hp;
        currentHP = hp;

        attack = atk;
        defense = def;
        speed = spd;

        this.ai_profile_id = ai_profile_id;
    }

    // =====================================================
    // DAMAGE
    // =====================================================

    public void TakeDamage(int damage)
    {
        if (isDefending)
        {
            damage =
                Mathf.RoundToInt(
                    damage * defenseMultiplier
                );
        }

        currentHP -= damage;

        if (currentHP < 0)
            currentHP = 0;
    }

    // =====================================================
    // HEAL
    // =====================================================

    public void Heal(int amount)
    {
        currentHP += amount;

        if (currentHP > GetMaxHP())
        {
            currentHP = GetMaxHP();
        }
    }

    // =====================================================
    // DOT SYSTEM
    // =====================================================

    [System.Serializable]
    public class DOTInstance
    {
        public int damagePerTurn;
        public int remainingTurns;

        public CharacterStats source;

        public DOTInstance(
            int damage,
            int turns,
            CharacterStats source
        )
        {
            damagePerTurn = damage;
            remainingTurns = turns;

            this.source = source;
        }
    }

    [NonSerialized]
    public List<DOTInstance> dots =
        new List<DOTInstance>();

    public void AddDOT(
        int damagePerTurn,
        int turns,
        CharacterStats source
    )
    {
        dots.Add(
            new DOTInstance(
                damagePerTurn,
                turns,
                source
            )
        );

        Debug.Log(
            $"[DOT] AddDOT on {name} | dmg={damagePerTurn} | turns={turns}"
        );
    }

    public void ProcessDOT(BattleUnit owner)
    {
        for (int i = dots.Count - 1; i >= 0; i--)
        {
            var dot = dots[i];

            int before = currentHP;

            owner.TakeDamage(
                dot.damagePerTurn,
                DamagePopup.PopupType.DOT
            );

            int actualDamage =
                before - currentHP;

            if (actualDamage > 0)
            {
                Debug.Log(
                    $"[DOT] {name} take {actualDamage} DOT damage"
                );

                BattleManager.Instance?.ui?.ShowMessage(
                    $"{name} take {actualDamage} damage"
                );
            }

            dot.remainingTurns--;

            if (dot.remainingTurns <= 0)
            {
                Debug.Log(
                    $"[DOT] DOT on {name} ended"
                );

                dots.RemoveAt(i);
            }
        }
    }

    // =====================================================
    // FINAL STATS
    // =====================================================

    public int GetAttack()
    {
        int total = attack;

        if (weaponID != -1)
        {
            ItemEntity weapon =
                ItemDatabase.Instance.GetItem(weaponID);

            if (weapon != null)
                total += weapon.bonusATK;
        }

        if (accessoryID != -1)
        {
            ItemEntity accessory =
                ItemDatabase.Instance.GetItem(accessoryID);

            if (accessory != null)
                total += accessory.bonusATK;
        }

        return total;
    }
    public int GetDefense()
    {
        int total = defense;

        if (armorID != -1)
        {
            ItemEntity armor =
                ItemDatabase.Instance.GetItem(armorID);

            if (armor != null)
                total += armor.bonusDEF;
        }

        if (accessoryID != -1)
        {
            ItemEntity accessory =
                ItemDatabase.Instance.GetItem(accessoryID);

            if (accessory != null)
                total += accessory.bonusDEF;
        }

        return total;
    }

    public int GetMaxHP()
    {
        int total = maxHP;

        if (accessoryID != -1)
        {
            ItemEntity item =
                ItemDatabase.Instance.GetItem(accessoryID);

            if (item != null)
            {
                total += item.bonusHP;
            }
        }

        return total;
    }

    // =====================================================
    // UTILITY
    // =====================================================

    public float HPPercent()
    {
        return (float)currentHP / GetMaxHP();
    }

    public bool IsDead()
    {
        return currentHP <= 0;
    }
}