using UnityEngine;

[System.Serializable]
public class InstantEffect
{
    public InstantEffectType type;

    public int value;

    public InstantEffect(
        InstantEffectType type,
        int value
    )
    {
        this.type = type;
        this.value = value;
    }

    public void Apply(BattleUnit target)
    {
        if (target == null)
            return;

        switch (type)
        {
            // =====================
            // HEAL HP
            // =====================

            case InstantEffectType.HealHP:

                int beforeHP =
                    target.stats.currentHP;

                target.stats.currentHP =
                    Mathf.Min(
                        target.stats.currentHP + value,
                        target.stats.maxHP
                    );

                int healed =
                    target.stats.currentHP - beforeHP;

                target.ShowHeal(healed);

                target.UpdateHPBar();

                Debug.Log(
                    $"{target.stats.name} heal {healed}"
                );

                break;

            // =====================
            // DAMAGE
            // =====================

            case InstantEffectType.DamageHP:

                target.TakeDamage(value);

                break;

            // =====================
            // CLEANSE
            // =====================

            case InstantEffectType.Cleanse:

                target.ClearAllEffects();

                Debug.Log(
                    $"{target.stats.name} cleansed"
                );

                break;

            // =====================
            // REVIVE
            // =====================

            case InstantEffectType.Revive:

                if (target.stats.IsDead())
                {
                    target.stats.currentHP = value;

                    target.UpdateHPBar();

                    Debug.Log(
                        $"{target.stats.name} revived"
                    );
                }

                break;
        }
    }
}