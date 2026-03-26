using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using static CharacterStats;
using static UnityEngine.RuleTile.TilingRuleOutput;

[System.Serializable]
public class CharacterStats
{
    public int id;
    public int faction_id;
    public string name;
    public int maxHP;
    public int currentHP;
    public int attack;
    public int defense;
    public int speed;
    public bool isDefending = false;
    public float defenseMultiplier = 0.7f;
    public int ai_profile_id;

    public CharacterStats(int id, int faction_id, string name, int hp, int atk, int def, int spd, int ai_profile_id)
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
    public void TakeDamage(int damage)
    {
        if (isDefending)
        {
            damage = Mathf.RoundToInt(damage * defenseMultiplier);
        }

        currentHP -= damage;
        if (currentHP < 0) currentHP = 0;
    }
    public void Heal(int amount)
    {
        currentHP += amount;
        if (currentHP > maxHP) currentHP = maxHP;
    }
    // DOT dame
    public void ProcessDOT(BattleUnit owner)
    {
        for (int i = dots.Count - 1; i >= 0; i--)
        {
            var dot = dots[i];

            int before = currentHP;

            owner.TakeDamage(dot.damagePerTurn, DamagePopup.PopupType.DOT);

            int actualDamage = before - currentHP;

            if (actualDamage > 0)
            {
                Debug.Log($"[DOT] {name} take {actualDamage} DOT damage");

                BattleManager.Instance?.ui?.ShowMessage(
                    $"{name} take {actualDamage} dame"
                );
            }

            dot.remainingTurns--;

            if (dot.remainingTurns <= 0)
            {
                Debug.Log($"[DOT] DOT on {name} end");
                dots.RemoveAt(i);
            }
        }
    }

    [System.Serializable]
    public class DOTInstance
    {
        public int damagePerTurn;
        public int remainingTurns;
        public CharacterStats source;

        public DOTInstance(int damage, int turns, CharacterStats source)
        {
            damagePerTurn = damage;
            remainingTurns = turns;
            this.source = source;
        }
    }


    [NonSerialized] 
    public List<DOTInstance> dots = new List<DOTInstance>();

    public void AddDOT(int damagePerTurn, int turns, CharacterStats source)
    {
        dots.Add(new DOTInstance(damagePerTurn, turns, source));
        Debug.Log(
        $"[DOT] AddDOT on {name} | dmg={damagePerTurn} | turns={turns}"
);
    }

    //-----------------------------------------------------------

    public float HPPercent()
    {
        return (float)currentHP / maxHP;
    }
    public bool IsDead()
    {
        return currentHP <= 0;
    }
}

