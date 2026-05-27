using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static SkillData;

public static class BattleAI
{
    // ================================
    // 1. Utility Profile 
    // ================================
    [System.Serializable]
    public class UtilityProfile
    {
        public float attackWeight = 1f;
        public float debuffWeight = 1f;
        public float defendWeight = 1f;
        public float healWeight = 1f;
        public float aoeWeight = 1f;
        public float finisherWeight = 1f;

        public float hpBias = 1f;     
        public float threatBias = 1f;
        public float controlWeight = 1f;
    }

    // ================================
    // 2. SCORE FUNCTIONS
    // ================================
    public static float ScoreAttack(BattleUnit caster, BattleUnit target, Dictionary<int, int> threat)
    {
        if (target == null || target.stats.IsDead()) return 0f;
        float hpFactor = 1f - target.stats.HPPercent(); // máu càng th?p càng ?u tiên
        float threatFactor = threat.ContainsKey(target.stats.id) ? threat[target.stats.id] / 100f : 0f;
        return hpFactor * 0.7f + threatFactor * 0.3f;
    }

    public static float ScoreAOE(List<BattleUnit> targets)
    {
        var living = targets.Where(u => !u.stats.IsDead()).ToList();
        if (living.Count == 0) return 0f;
        float avgHpLoss = living.Average(u => 1f - u.stats.HPPercent());
        return (living.Count / 5f) + avgHpLoss * 0.5f;
    }

    public static float ScoreHeal(List<BattleUnit> allies)
    {
        var lowest = allies.Where(a => !a.stats.IsDead())
                           .OrderBy(a => a.stats.HPPercent())
                           .FirstOrDefault();
        return lowest != null ? 1f - lowest.stats.HPPercent() : 0f;
    }

    public static float ScoreDefend(BattleUnit caster)
    {
        return caster.stats.HPPercent() < 0.35f ? 1f : 0.2f;
    }

    public static float ScoreDebuff(BattleUnit target)
    {
        if (target == null) return 0f;
        return target.stats.defense / 100f;
    }
    // ================================
    // 3. TARGET SELECTION
    // ================================
    public static BattleUnit SelectUtilityTarget(
    BattleUnit enemyUnit,
    List<BattleUnit> players,
    Dictionary<int, int> threatTable,
    Dictionary<int, int> focusCount
)
    {

        var alive = players.Where(p => !p.stats.IsDead()).ToList();
        Debug.Log($"[SelectUtilityTarget] S? player còn s?ng: {alive.Count}");
        if (alive.Count == 0) return null;

        var util = enemyUnit.utilityProfile;

        // ================================
        // NEMESIS MODE – OVERRIDE LOGIC
        // ================================
        if (util.threatBias >= 3.0f && threatTable.Count > 0)
        {
            int highestThreatId = threatTable
                .OrderByDescending(t => t.Value)
                .First().Key;

            var nemesis = alive.FirstOrDefault(p => p.stats.id == highestThreatId);

            if (nemesis != null)
            {
                Debug.Log($"[AI NEMESIS] {enemyUnit.stats.name} hunts {nemesis.stats.name}");
                return nemesis;
            }
        }
        float bestScore = float.MinValue;
        BattleUnit best = alive[0];

        foreach (var p in alive)
        {
            var stats = p.stats;

            // 1) HP SCORE Low Hp deteck
            float hpScore = (1f - stats.HPPercent()) * 0.50f;

            // 2) THREAT SCORE Ain't low check threat
            float threatRaw = threatTable.ContainsKey(stats.id)
                ? (float)threatTable[stats.id] / stats.maxHP
                : 0f;

            float threatScore = Mathf.Clamp(threatRaw, 0f, 0.40f);

            // 3) Too much hit ! Stop
            int fc = focusCount.ContainsKey(stats.id) ? focusCount[stats.id] : 0;
            float focusPenalty =
                (fc >= 3) ? 0.15f :
                (fc == 2) ? 0.40f :
                (fc == 1) ? 0.75f : 1f;

            // 4) FINISHER BONUS – too low HP must focus 
            float finisher = (stats.HPPercent() < 0.25f) ? 0.20f : 0f;

            // 5) random 
            float noise = Random.Range(0.0f, 0.15f);

            // 6) FINAL SCORE - yes
            float finalScore =
            (
                hpScore * util.hpBias +
                threatScore * util.threatBias +
                finisher * util.finisherWeight +
                noise
            ) * focusPenalty;
            Debug.Log(
                    $"[AI DEBUG] Target: {p.stats.name} | " +
                    $"HP%: {p.stats.HPPercent():0.00} | " +
                    $"HPScore: {hpScore:0.00} | " +
                    $"ThreatRaw: {threatRaw:0.00} | ThreatScore: {threatScore:0.00} | " +
                    $"Finisher: {finisher:0.00} | Noise: {noise:0.00} | " +
                    $"Focus: {fc} | Penalty: {focusPenalty:0.00} | " +
                    $"FINAL: {finalScore:0.000}"
                );
            if (finalScore > bestScore)
            {
                bestScore = finalScore;
                best = p;
            }
        }
        if (best != null)
        {
            Debug.Log($"[SelectUtilityTarget] ? Ch?n target: {best.stats.name} v?i score {bestScore:0.000}");
        }
        else
        {
            Debug.LogWarning("[SelectUtilityTarget] Cant chose any target");
        }
        return best;
    }
    public static BattleUnit SelectTargetForSkill(
    BattleUnit caster,
    RuntimeSkill skill,
    List<BattleUnit> playerUnits,
    List<BattleUnit> enemyUnits,
    Dictionary<int, int> threatTable,
    Dictionary<int, int> focusCount
)
    {
        switch (skill.targetType)
        {
            case TargetType.Self:
                return caster;

            case TargetType.Ally:
                var allyList = caster.isPlayer ? playerUnits : enemyUnits;
                return allyList.Where(u => !u.stats.IsDead())
                               .OrderBy(u => u.stats.HPPercent())
                               .FirstOrDefault() ?? caster;
            case TargetType.Single:
                return SelectUtilityTarget(caster, playerUnits, threatTable, focusCount);
            case TargetType.AOE:
                return playerUnits.FirstOrDefault(p => !p.stats.IsDead());
        }
        return caster;
    }

    // ================================
    // 4. SKILL SELECTION
    // ================================
    public static RuntimeSkill SelectSkill(
        BattleUnit enemyUnit,
        List<BattleUnit> playerUnits,
        List<BattleUnit> enemyUnits
    )
    {
        foreach (var s in enemyUnit.runtimeSkills)
        {
            Debug.Log(
                $"{s.name} | cd={s.currentCooldown}"
            );
        }
        var availableSkills = enemyUnit.runtimeSkills.Where(s => s.currentCooldown == 0).ToList();
        Debug.Log($"[SelectSkill] {enemyUnit.stats.name} có {availableSkills.Count} skill kh? d?ng");
        foreach (var skill in availableSkills)
        {
            Debug.Log($"[SelectSkill] Skill {skill.name} | CD={skill.currentCooldown} | targetType={skill.targetType}");
        }
        if (availableSkills.Count == 0) return null;

        float bestScore = -1f;
        RuntimeSkill bestSkill = null;
        var util = enemyUnit.utilityProfile;
        foreach (var skill in availableSkills)
        {
            float score = 0f;
            bool hasValidTarget = true;

            switch (skill.Type)
            {
                case SkillData.SkillType.Attack:
                case SkillData.SkillType.DoT:
                case SkillData.SkillType.Stun:
                case SkillData.SkillType.Debuff:
                    if (skill.targetType == SkillData.TargetType.AOE)
                    {
                        int aliveCount = playerUnits.Count(p => !p.stats.IsDead());
                        hasValidTarget = aliveCount > 0;
                        float aoeScore = aliveCount > 2 ? 1f : 0.5f;
                        score = aoeScore * util.aoeWeight;
                    }
                    else
                    {
                        var target = playerUnits.Where(p => !p.stats.IsDead()).FirstOrDefault();
                        hasValidTarget = target != null;
                        if (target != null)
                            score = (1f - target.stats.HPPercent()) * util.attackWeight;
                    }
                    break;

                case SkillData.SkillType.Heal:
                case SkillData.SkillType.Cure:
                    hasValidTarget = enemyUnits.Any(u => !u.stats.IsDead());
                    score = ScoreHeal(enemyUnits) * util.healWeight;
                    break;

                case SkillData.SkillType.Defense:
                case SkillData.SkillType.Taunt:
                case SkillData.SkillType.Shield:
                    hasValidTarget = true;
                    score = util.defendWeight;
                    break;
            }

            if (!hasValidTarget)
            {
                Debug.Log($"[SelectSkill] B? qua skill {skill.name} vì không có target h?p l?");
                continue;
            }

            score += Random.Range(-0.05f, 0.05f);

            if (score > bestScore)
            {
                bestScore = score;
                bestSkill = skill;
            }
        }
        return bestSkill;
    }
    private static void ApplyStatusEffect(
      RuntimeSkill skill,
      BattleUnit caster,
      BattleUnit target,
      BattleUI ui
  )
    {
        if (
            skill.status_effect_id <= 0 ||
            target == null ||
            target.stats.IsDead()
        )
        {
            return;
        }

        StatusEffect effect =
            StatusEffectFactory.Create(
                skill.status_effect_id,
                caster
            );

        if (effect == null)
            return;

        target.AddEffect(effect);

        ui.ShowMessage(
            target.stats.name +
            " take " +
            effect.effectType
        );

        Debug.Log(
            "[AI Status] " +
            target.stats.name +
            " take effect " +
            effect.effectType
        );
    }
    // ================================
    // 5. Use Skill
    // ================================
    public static int UseSkill(BattleUnit caster, BattleUnit targetUnit, RuntimeSkill skill,
                                List<BattleUnit> playerUnits, List<BattleUnit> enemyUnits,
                                BattleUI ui, List<UnitUI> playerUIItems, List<UnitUI> enemyUIItems)
    {
        var attacker = caster.stats;
        int threatGenerated = 0;

        if (skill == null)
        {
            Debug.LogWarning($"[UseSkill] {attacker.name} không có skill kh? d?ng ? fallback ?ánh th??ng");
            if (targetUnit != null && !targetUnit.stats.IsDead())
            {
                int damage = attacker.attack;
                targetUnit.TakeDamage(damage);
                ui.ShowMessage($"{attacker.name} ?ánh th??ng {targetUnit.stats.name} gây {damage} sát th??ng!");

                int idx = playerUnits.IndexOf(targetUnit);
                if (idx >= 0 && idx < playerUIItems.Count)
                    playerUIItems[idx].UpdateHP();

                threatGenerated += damage;
            }
            return threatGenerated;
        }

        Debug.Log($"[UseSkill] {attacker.name} dùng skill {skill.name} | CD={skill.cooldown}, currentCD={skill.currentCooldown}");

        ui.ShowMessage(attacker.name + " dùng " + skill.name + "!");

        switch (skill.Type)
        {
            case SkillData.SkillType.Attack:
                if (skill.targetType == SkillData.TargetType.Single)
                {
                    int damage = Mathf.RoundToInt(attacker.attack * (skill.power / 100f));
                    targetUnit.TakeDamage(damage);

                    ApplyStatusEffect(skill, caster, targetUnit, ui);

                    BattleManager.Instance.AddThreat(targetUnit.stats, damage);

                    int idx = playerUnits.IndexOf(targetUnit);
                    if (idx >= 0 && idx < playerUIItems.Count)
                        playerUIItems[idx].UpdateHP();

                    threatGenerated += damage;
                    Debug.Log($"[UseSkill] {attacker.name} gây {damage} sát th??ng lên {targetUnit.stats.name}");
                }
                else if (skill.targetType == SkillData.TargetType.AOE)
                {
                    foreach (var opp in playerUnits)
                    {
                        if (opp.stats.IsDead()) continue;

                        int damage = Mathf.RoundToInt(attacker.attack * (skill.power / 100f));
                        //opp.stats.TakeDamage(damage);
                        opp.TakeDamage(damage);

                        ApplyStatusEffect(skill, caster, opp, ui);

                        int idx = playerUnits.IndexOf(opp);
                        if (idx >= 0 && idx < playerUIItems.Count)
                            playerUIItems[idx].UpdateHP();
                        BattleManager.Instance.AddThreat(attacker, damage);
                        Debug.Log($"[UseSkill] {attacker.name} AOE gây {damage} sát th??ng lên {opp.stats.name}");
                    }
                    //return 0;
                }
                break;

            case SkillData.SkillType.Heal:
                var allyLowHP = enemyUnits.Where(u => !u.stats.IsDead())
                                          .OrderBy(u => u.stats.currentHP)
                                          .FirstOrDefault();

                if (allyLowHP != null)
                {
                    int before = allyLowHP.stats.currentHP;

                    allyLowHP.stats.currentHP = Mathf.Min(
                        allyLowHP.stats.currentHP + skill.power,
                        allyLowHP.stats.maxHP
                    );

                    int actualHeal = allyLowHP.stats.currentHP - before;

                    allyLowHP.ShowHeal(actualHeal);

                    ApplyStatusEffect( skill, caster, allyLowHP, ui);

                    int idx = enemyUnits.IndexOf(allyLowHP);
                    if (idx >= 0 && idx < enemyUIItems.Count)
                        enemyUIItems[idx].UpdateHP();

                    ui.ShowMessage($"{attacker.name} heal {actualHeal} HP cho {allyLowHP.stats.name}!");

                    Debug.Log($"[UseSkill] {attacker.name} heal {allyLowHP.stats.name} +{actualHeal} HP");
                }
                break;

            case SkillData.SkillType.Defense:
                attacker.isDefending = true;
                attacker.defenseMultiplier = Mathf.Clamp(1f - (skill.power / 100f), 0.2f, 1f);
                ui.ShowMessage($"{attacker.name} Defense {skill.power}%!");
                break;

            case SkillData.SkillType.Buff:
                attacker.attack += skill.power;

                ApplyStatusEffect( skill, caster, caster, ui);

                ui.ShowMessage($"{attacker.name} ATK+ {skill.power}!");
                Debug.Log($"[UseSkill] {attacker.name} buff ATK +{skill.power}");
                break;

            //case SkillData.SkillType.DoT:
            //    if (targetUnit != null)
            //    {
            //        int dotDamage = Mathf.RoundToInt(attacker.attack * (skill.power / 100f));
            //        targetUnit.stats.AddDOT(dotDamage, turns: 3, source: attacker);
            //        ui.ShowMessage($"{targetUnit.stats.name} b? Poisoned");
            //        Debug.Log($"[UseSkill] {attacker.name} gây DoT {dotDamage} lên {targetUnit.stats.name}");
            //    }
            //    break;
        }
        skill.currentCooldown = skill.cooldown;
        Debug.Log($"[UseSkill] {skill.name} vào cooldown {skill.cooldown} turn");

        ui.UpdateTeamHP(playerUnits.Select(p => p.stats).ToList(), enemyUnits.Select(e => e.stats).ToList());

        return threatGenerated;
    }

    // ================================
    // 6. ENTRY POINT
    // ================================
    public static BattleUnit SelectTarget(
        BattleUnit enemy,
        List<BattleUnit> playerUnits,
        Dictionary<int, int> threat,
        Dictionary<int, int> focusCount)
    {
        return SelectUtilityTarget(enemy, playerUnits, threat, focusCount);
    }

}