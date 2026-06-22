using SQLite4Unity3d;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.SceneManagement;
using static SkillData;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    private SQLiteConnection db;

    void Awake()
    {
        Instance = this;
    }
    private bool battleStarted = false;
    private bool battleEnded = false;


    private BattleUnit selectedTarget;
    public BattleUnit SelectedTarget => selectedTarget;

    [Header("Prefabs")]
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    [Header("Spawn Positions (max 3 each)")]
    public Transform[] playerSpawnPoints;
    public Transform[] enemySpawnPoints;

    public List<CharacterStats> playerTeam = new List<CharacterStats>();
    public List<CharacterStats> enemyTeam = new List<CharacterStats>();

    private List<BattleUnit> playerUnits = new List<BattleUnit>();
    private List<BattleUnit> enemyUnits = new List<BattleUnit>();

    private List<UnitUI> playerUIItems = new List<UnitUI>();
    private List<UnitUI> enemyUIItems = new List<UnitUI>();

    public Transform BottomPanel;
    public GameObject PlayerInfoItem;
    public Transform[] PlayerUIAnchor;

    public GameObject EnemyInfoItem;
    public Transform[] EnemyUIAnchor;

    public BattleUI ui;

    private Queue<BattleUnit> turnQueue = new Queue<BattleUnit>();
    private BattleUnit currentUnit;
    public BattleUnit CurrentUnit => currentUnit;
    private Dictionary<int, int> focusCount = new Dictionary<int, int>();


    void Start()
    {
        LoadCharactersFromDB();
        SpawnCharacters();
        //InitTurnOrder();
        StartCoroutine(BattleIntro());
    }

    // ----------------------------------------------------------
    // 1. Load nhân vật từ database
    // ----------------------------------------------------------
    void LoadCharactersFromDB()
    {
        string dbPath = System.IO.Path.Combine(Application.streamingAssetsPath, "Datagame.db");
        db = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite);
        Debug.Log("Đang mở DB tại: " + dbPath);

        playerTeam.Clear();

        var partyStats = PartyManager.Instance.PartyStats;

        if (partyStats == null || partyStats.Count == 0)
        {
            Debug.LogWarning("Party rỗng → fallback lấy 3 player mặc định");

            var unlocked = PlayerProgression.Instance.player.unlockedCharacters;

            foreach (var id in unlocked.Take(3))
            {
                var p = db.Find<CharacterData>(id);

                if (p == null)
                {
                    Debug.LogWarning(
                        $"Character {id} không tồn tại trong DB"
                    );
                    continue;
                }

                playerTeam.Add(
                    new CharacterStats(
                        p.id,
                        p.faction_id,
                        p.name,
                        p.hp,
                        p.atk,
                        p.def,
                        p.spd,
                        p.baseLevel,
                        p.expReward,
                        p.goldReward,
                        p.ai_profile_id
                    )
                );
            }

            Debug.Log(
                $"Fallback team count = {playerTeam.Count}"
            );
        }
        else
        {
            playerTeam = new List<CharacterStats>(partyStats);

            Debug.Log("Load player từ PartyManager (giữ equipment)");
        }

        // Enemy team — lấy từ EncounterData
        var carrier = BattleEncounterData.Instance;
        if (carrier == null)
        {
            Debug.LogError("[BattleManager] EncounterData không tồn tại trong BattleScene!");
        }
        else if (carrier.SelectedEnemyIDs == null || carrier.SelectedEnemyIDs.Count == 0)
        {
            Debug.LogWarning("[BattleManager] Encounter rỗng — fallback lấy 3 enemy mặc định.");
        }
        else
        {
            Debug.Log("[BattleManager] Đang load enemy từ EncounterData: " +
                      string.Join(",", carrier.SelectedEnemyIDs));
        }
        if (carrier == null || carrier.SelectedEnemyIDs == null || carrier.SelectedEnemyIDs.Count == 0)
        {
            Debug.LogWarning("Encounter rỗng — fallback lấy 3 enemy mặc định.");
            var fallback = db.Table<CharacterData>().Where(c => c.faction_id == 2).Take(3).ToList();
            foreach (var e in fallback)
                enemyTeam.Add(new CharacterStats(e.id, e.faction_id, e.name, e.hp, e.atk, e.def, e.spd, e.baseLevel, e.expReward, e.goldReward,e.ai_profile_id));
        }
        else
        {
            foreach (var id in carrier.SelectedEnemyIDs)
            {
                var e = db.Table<CharacterData>().FirstOrDefault(c => c.id == id && c.faction_id == 2);
                if (e != null)
                    enemyTeam.Add(new CharacterStats(e.id, e.faction_id, e.name, e.hp, e.atk, e.def, e.spd, e.baseLevel, e.expReward, e.goldReward, e.ai_profile_id));
                else
                    Debug.LogWarning($"Enemy id {id} không tìm thấy trong DB hoặc không thuộc faction 2.");
            }
        }
    }

    private IEnumerator BattleIntro()
    {
        Debug.Log("Battle Start!");
        yield return new WaitForSeconds(2f);

        InitTurnOrder();   // khởi tạo queue trước
        battleStarted = true;

        Debug.Log("[BattleIntro] ✅ Setup xong → bắt đầu lượt đầu tiên");
        NextTurn();
    }
    // ----------------------------------------------------------
    // 2. Spawn ra field bằng prefab 
    // ----------------------------------------------------------
    public string GetAIProfileById(int id)
    {
        if (id <= 0) return null; // Player hoặc enemy không có AI

        var profile = db.Table<AIProfile>().FirstOrDefault(p => p.id == id);
        return profile != null ? profile.name : null;
    }

    void SpawnCharacters()
    {
        Debug.Log("PartyManager count: " + PartyManager.Instance.PartyStats.Count);

        // Player side
        for (int i = 0; i < playerTeam.Count && i < playerSpawnPoints.Length; i++)
        {
            var obj = Instantiate(playerPrefab, playerSpawnPoints[i]);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;

            var unit = obj.GetComponent<BattleUnit>();
            unit.Setup(playerTeam[i], true);
            unit.originalPosition = unit.transform.position;
            playerUnits.Add(unit);

            unit.runtimeSkills = GetSkillsForCharacter(unit.stats.id)
            .Select(s => new RuntimeSkill(s))
            .ToList();

            var uiItem = Instantiate(PlayerInfoItem, PlayerUIAnchor[i]);
            uiItem.transform.localPosition = Vector3.zero;
            uiItem.transform.localRotation = Quaternion.identity;
            uiItem.transform.localScale = Vector3.one;

            var uiComp = uiItem.GetComponent<UnitUI>();
            uiComp.Setup(unit);
            var view = obj.GetComponent<CharacterView>();
            if (view != null)
            {
                view.Init(unit.stats.id);
            }
            else
            {
                Debug.LogError("Player prefab thiếu CharacterView!");
            }
            playerUIItems.Add(uiComp);
        }

        // Enemy side
        for (int i = 0; i < enemyTeam.Count && i < enemySpawnPoints.Length; i++)
        {
            var obj = Instantiate(enemyPrefab, enemySpawnPoints[i]);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;

            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.flipX = true;
            }
            // 1) Lấy stats gốc từ danh sách enemyTeam
            var stats = enemyTeam[i]; // đây là CharacterStats

            // 2) Lấy rank từ EncounterData bằng ID
            BattleTrigger.EnemyRank rank = BattleTrigger.EnemyRank.Normal; // default
            int level = 1;

            if (BattleEncounterData.Instance != null)
            {
                rank = BattleEncounterData.Instance.GetRank(stats.id);
                level = BattleEncounterData.Instance.GetLevel(stats.id);    
            }
            else
            {
                Debug.LogWarning("[SpawnCharacters] EncounterData null, dùng rank mặc định Normal và level 1");
            }

            // 3) Áp dụng multiplier theo rank TRƯỚC khi setup unit
            ApplyEnemyScaling(stats, rank, level);

            // 4) Setup BattleUnit với stats đã scale
            var unit = obj.GetComponent<BattleUnit>();
            unit.Setup(stats, false); // false = isEnemy

            unit.enemyID = stats.id;

            unit.originalPosition = unit.transform.position;
            enemyUnits.Add(unit);

            // 5) Load skills từ DB
            unit.runtimeSkills = GetSkillsForCharacter(unit.stats.id)
            .Select(s => new RuntimeSkill(s))
            .ToList();

            // 6) Gán AI profile từ UtilityPresets
            if (stats.ai_profile_id > 0)
            {
                unit.utilityProfile = UtilityPresets.GetProfile(stats.ai_profile_id);
                if (unit.utilityProfile != null)
                    Debug.Log($"[SpawnEnemy] {unit.stats.name} gán AI profile id={stats.ai_profile_id}");
                else
                    Debug.LogWarning($"[SpawnEnemy] {unit.stats.name} ai_profile_id={stats.ai_profile_id} nhưng không tìm thấy profile!");
            }
            else
            {
                Debug.Log($"[SpawnEnemy] {unit.stats.name} không có AI profile (id={stats.ai_profile_id})");
            }

            // 7) UI hiển thị cho enemy
            var uiItem = Instantiate(EnemyInfoItem, EnemyUIAnchor[i]);
            uiItem.transform.localPosition = Vector3.zero;
            uiItem.transform.localRotation = Quaternion.identity;
            uiItem.transform.localScale = Vector3.one;

            var uiComp = uiItem.GetComponent<UnitUI>();
            uiComp.Setup(unit);
            var view = obj.GetComponent<CharacterView>();
            if (view != null)
            {
                view.Init(unit.stats.id);
            }
            else
            {
                Debug.LogError("Enemy prefab thiếu CharacterView!");
            }

            // Optional: hiển thị nhãn rank trên UI
            uiComp.SetRankTag(rank); // viết hàm này trong UnitUI nếu muốn hiển thị "Elite"/"Boss"

            enemyUIItems.Add(uiComp);
        }

        // Cập nhật HP ban đầu
        ui.UpdateTeamHP(playerTeam, enemyTeam);

    }
    //set rank cho enemy
    public static void ApplyEnemyScaling(
     CharacterStats stats,
     BattleTrigger.EnemyRank rank,
     int level)
    {
        float levelScale =
            1f + ((level - 1) * 0.1f);

        stats.maxHP =
            Mathf.RoundToInt(
                stats.maxHP *
                levelScale);

        stats.attack =
            Mathf.RoundToInt(
                stats.attack *
                levelScale);

        stats.defense =
            Mathf.RoundToInt(
                stats.defense *
                levelScale);

        switch (rank)
        {
            case BattleTrigger.EnemyRank.Elite:

                stats.maxHP =
                    Mathf.RoundToInt(
                        stats.maxHP * 1.6f);

                stats.attack =
                    Mathf.RoundToInt(
                        stats.attack * 1.2f);

                break;

            case BattleTrigger.EnemyRank.Boss:

                stats.maxHP =
                    Mathf.RoundToInt(
                        stats.maxHP * 2.2f);

                stats.attack =
                    Mathf.RoundToInt(
                        stats.attack * 1.5f);

                break;
        }

        stats.currentHP = stats.maxHP;
    }

    // ----------------------------------------------------------
    // 3. Turn Order
    // ----------------------------------------------------------
    void InitTurnOrder()
    {
        var allUnits = playerUnits.Concat(enemyUnits)
                              .Where(u => !u.stats.IsDead())
                              .OrderByDescending(u => u.stats.speed)
                              .ToList();

        Debug.Log("==== INIT TURN ORDER ====");
        foreach (var u in allUnits)
            Debug.Log($"Unit: {u.stats.name} | isPlayer={u.isPlayer} | HP={u.stats.currentHP}");

        turnQueue.Clear();
        foreach (var u in allUnits)
            turnQueue.Enqueue(u);
    }

    public void NextTurn()
    {
        ui.ResetPanels();
        if (!battleStarted) return;

        if (turnQueue.Count == 0)
        {
            DecayFocusEndOfRound();
            InitTurnOrder();
            if (turnQueue.Count == 0)
            {
                Debug.LogError("[NextTurn] ❌ TurnQueue vẫn rỗng sau khi init!");
                return;
            }
        }
        currentUnit = turnQueue.Dequeue();
        Debug.Log($"========== TỚI LƯỢT: {currentUnit.stats.name} | isPlayer={currentUnit.isPlayer} ==========");

        bool stunned =currentUnit.activeEffects.Any( e => e.effectType == "Stun");
        if (stunned)
        {
            Debug.Log(
                currentUnit.stats.name +
                " bị stun -> skip turn"
            );

            currentUnit.ProcessEffects();

            NextTurn();

            return;
        }
        //currentUnit.stats.ProcessDOT(currentUnit);
        currentUnit.ProcessEffects();
        while (currentUnit.stats.IsDead())
        {
            if (turnQueue.Count == 0)
            {
                InitTurnOrder();
                if (turnQueue.Count == 0) return;
            }
            currentUnit = turnQueue.Dequeue();
        }

        currentUnit.stats.isDefending = false;
        currentUnit.stats.defenseMultiplier = 1f;

        foreach (var s in currentUnit.runtimeSkills)
        {
            if (s.currentCooldown > 0) s.currentCooldown--;
        }

        CheckBattleEnd();
        if (battleEnded) return;

        if (currentUnit.isPlayer)
        {
            Debug.Log($"[NextTurn] ✅ Tới lượt player: {currentUnit.stats.name}");
            ui.SetupPlayerTurn(currentUnit);   
        }
        else
        {
            if (battleEnded) return;
            if (currentUnit == null || currentUnit.stats == null)
            {
                Debug.LogWarning("[NextTurn] ❌ currentUnit null → skip");
                return;
            }

            Debug.Log($"[NextTurn] ✅ Tới lượt enemy: {currentUnit.stats.name}");
            EnemyAction(currentUnit);
        }
    }

    // check target clicked , ally enemy or not u 
    bool TryValidateTarget(BattleUnit caster, BattleUnit target, RuntimeSkill skill)
    {
        // Self skill → luôn hợp lệ
        if (skill.targetType == TargetType.Self)
            return true;
        // Cần target nhưng chưa chọn
        if (target == null)
        {
            ui.ShowMessage("Hãy chọn mục tiêu!");
            return false;
        }
        // ===== Attack / DoT =====
        if (skill.Type == SkillType.Attack || skill.Type == SkillType.DoT)
        {
            if (target.isPlayer == caster.isPlayer)
            {
                ui.ShowMessage("Không thể tấn công đồng minh, vui lòng chọn lại!");
                return false;
            }
        }
        // ===== Heal / Buff / Cure =====
        if (skill.Type == SkillType.Heal ||
            skill.Type == SkillType.Buff ||
            skill.Type == SkillType.Cure)
        {
            if (target.isPlayer != caster.isPlayer)
            {
                ui.ShowMessage("Skill này chỉ dùng cho đồng minh!");
                return false;
            }
        }
        return true;
    }
    public void OnUnitClicked(BattleUnit unit)
    {
        // Chỉ cho phép chọn khi là lượt player
        if (currentUnit == null || !currentUnit.isPlayer) return;
        if (unit.stats.IsDead()) return;

        // Cho phép chọn cả enemy (tấn công) hoặc ally 
        if (selectedTarget != null)
            selectedTarget.Highlight(false);

        selectedTarget = unit;
        ui.ShowMessage("Đã chọn: " + unit.stats.name);
        unit.Highlight(true);
    }
    private IEnumerator MoveAndAttack(BattleUnit attackerUnit, BattleUnit targetUnit, System.Action onAttackDone)
    {
        Vector3 startPos = attackerUnit.originalPosition;
        Vector3 targetPos = targetUnit.transform.position
                            + new Vector3(attackerUnit.isPlayer ? -1.5f : 1.5f, 0, 0);

        // Di chuyển tới gần mục tiêu
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            attackerUnit.transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        // Placeholder animation
        Debug.Log(attackerUnit.stats.name + " Attack Animation!");

        // Gây sát thương
        onAttackDone?.Invoke();

        yield return new WaitForSeconds(0.5f);

        // Quay về vị trí gốc
        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            attackerUnit.transform.position = Vector3.Lerp(targetPos, startPos, t);
            yield return null;
        }
        CheckBattleEnd();
        if (!battleEnded)
            // Chỉ chuyển lượt sau khi quay về xong
            NextTurn();
    }
    public void PlayerAttack()
    {
        if (currentUnit == null || !currentUnit.isPlayer) return;
        if (selectedTarget == null)
        {
            ui.ShowMessage("Hãy chọn mục tiêu trước!");
            return;
        }
        if (selectedTarget.isPlayer)
        {
            ui.ShowMessage("<color=yellow>Không thể tấn công đồng minh!</color>");
            return;
        }
        StartCoroutine(MoveAndAttack(currentUnit, selectedTarget, () =>
        {
            var attacker = currentUnit.stats;
            var target = selectedTarget.stats;
            Debug.Log("ATTACK BASE: " + attacker.attack);
            int damage = attacker.GetAttack();
            if (attacker.weaponID != -1)
            {
                ItemEntity weapon =
                    ItemDatabase.Instance.GetItem(attacker.weaponID);

                if (weapon != null)
                {
                    Debug.Log(
                        "WEAPON BONUS: " +
                        weapon.bonusATK
                    );

                }
            }

            Debug.Log("FINAL ATTACK: " + attacker.GetAttack());
            selectedTarget.TakeDamage(damage);

            TryApplyWeaponEffect(
                currentUnit,
                selectedTarget
            );

            AddThreat(attacker, damage);

            ui.UpdateTeamHP(playerTeam, enemyTeam);

            int idx = enemyTeam.IndexOf(target);
            if (idx >= 0 && idx < enemyUIItems.Count)
                enemyUIItems[idx].UpdateHP();

            if (target.IsDead())
            {
                ui.ShowMessage("Enemy Defeated!");
            }

            selectedTarget.Highlight(false);
            selectedTarget = null;
            ui.HideArtsList();

            CheckBattleEnd();
            if (battleEnded) return; 
        }
        ));
    }

    public void PlayerDefend()
    {
        if (currentUnit == null || !currentUnit.isPlayer) return;

        currentUnit.stats.isDefending = true;
        ui.ShowMessage(currentUnit.stats.name + " Defends!");

        ui.HideArtsList();
        NextTurn();
    }

    // Lưu threat cho từng player
    Dictionary<int, int> threatTable = new Dictionary<int, int>();

    public void AddThreat(CharacterStats player, int amount)
    {
        if (!threatTable.ContainsKey(player.id))
            threatTable[player.id] = 0;

        threatTable[player.id] += amount;
    }

    private void OnEnemyHitsTarget(CharacterStats target)
    {
        if (!focusCount.ContainsKey(target.id))
            focusCount[target.id] = 0;

        focusCount[target.id]++;
    }

    private void DecayFocusEndOfRound()
    {
        var keys = new List<int>(focusCount.Keys);
        foreach (var id in keys)
            focusCount[id] = Mathf.Max(0, focusCount[id] - 1);
    }

    Coroutine delayedNextTurnCoroutine;
    void EnemyAction(BattleUnit enemyUnit)
    {
        if (enemyUnit == null || enemyUnit.stats == null || enemyUnit.isPlayer)
        {
            Debug.LogWarning("[EnemyAction] Enemy không hợp lệ");
            NextTurn();
            return;
        }

        var skill = BattleAI.SelectSkill(enemyUnit, playerUnits, enemyUnits);

        if (skill == null)
        {
            Debug.LogWarning(
                $"[EnemyAction] {enemyUnit.stats.name} không có skill khả dụng -> đánh thường"
            );

            BattleUnit fallbackTarget =
                BattleAI.SelectUtilityTarget(
                    enemyUnit,
                    playerUnits,
                    threatTable,
                    focusCount
                );

            if (fallbackTarget == null)
            {
                Debug.LogWarning(
                    "[EnemyAction] Không có target fallback"
                );

                NextTurn();
                return;
            }

            StartCoroutine(
                MoveAndAttack(
                    enemyUnit,
                    fallbackTarget,
                    () =>
                    {
                        int damage =
                            enemyUnit.stats.attack;

                        fallbackTarget.TakeDamage(damage);

                        FinishEnemyAction(
                            fallbackTarget,
                            true
                        );
                    }
                )
            );

            return;
        }

        BattleUnit target = BattleAI.SelectTargetForSkill(
            enemyUnit,
            skill,
            playerUnits,
            enemyUnits,
            threatTable,
            focusCount
        );

        bool needsMovement =
            skill.Type == SkillType.Attack &&
            skill.rangeType == RangeType.Melee;

        if (needsMovement && target != null)
        {
            StartCoroutine(MoveAndAttack(enemyUnit, target, () =>
            {
                BattleAI.UseSkill(
                    enemyUnit,
                    target,
                    skill,
                    playerUnits,
                    enemyUnits,
                    ui,
                    playerUIItems,
                    enemyUIItems
                );

                FinishEnemyAction(target , true);
            }));

            return;
        }

        // Cast tại chỗ
        BattleAI.UseSkill(
            enemyUnit,
            target,
            skill,
            playerUnits,
            enemyUnits,
            ui,
            playerUIItems,
            enemyUIItems
        );

        FinishEnemyAction(target);
    }

    void FinishEnemyAction(
    BattleUnit target,
    bool alreadyHandledByMovement = false
)
    {
        if (target != null)
        {
            OnEnemyHitsTarget(target.stats);

            int idx = playerTeam.IndexOf(target.stats);

            if (idx >= 0 && idx < playerUIItems.Count)
                playerUIItems[idx].UpdateHP();
        }

        ui.UpdateTeamHP(playerTeam, enemyTeam);

        CheckBattleEnd();

        // Nếu chưa được MoveAndAttack xử lý turn
        if (!battleEnded && !alreadyHandledByMovement)
        {
            NextTurn();
        }
    }
    public List<SkillData> GetSkillsForCharacter(int characterId)
    {
        string dbPath = System.IO.Path.Combine(Application.streamingAssetsPath, "Datagame.db");
        var db = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite);

        var charSkills = db.Table<CharacterSkillData>()
                           .Where(cs => cs.characterId == characterId)
                           .ToList();

        var skillIds = charSkills.Select(cs => cs.skillId).ToList();
        var skills = db.Table<SkillData>()
                       .Where(s => skillIds.Contains(s.id))
                       .ToList();

        return skills;
    }
    void TryApplyWeaponEffect(
      BattleUnit attacker,
      BattleUnit target
  )
    {
        if (attacker == null || target == null)
            return;

        // =========================
        // GET WEAPON
        // =========================

        if (attacker.stats.weaponID == -1)
            return;

        ItemEntity weapon =
            ItemDatabase.Instance.GetItem(
                attacker.stats.weaponID
            );

        if (weapon == null)
            return;

        // =========================
        // NO EFFECT
        // =========================

        if (weapon.statusEffectID <= 0)
            return;

        // =========================
        // PROC CHANCE
        // =========================

        float roll =
            UnityEngine.Random.value;

        Debug.Log(
            $"[WeaponProc] roll={roll} chance={weapon.effectChance}"
        );

        if (roll > weapon.effectChance)
            return;

        // =========================
        // CREATE EFFECT
        // =========================

        StatusEffect effect =
            StatusEffectFactory.Create(
                weapon.statusEffectID,
                attacker
            );

        if (effect == null)
            return;

        // =========================
        // APPLY
        // =========================

        target.AddEffect(effect);

        Debug.Log(
            $"{weapon.name} applied {effect.effectType}"
        );

        ui.ShowMessage(
            $"{target.stats.name} bị {effect.effectType}!"
        );
    }
    void TryApplyStatusEffect(
        RuntimeSkill skill,
        BattleUnit caster,
        BattleUnit target)
    {
        if (
            skill.status_effect_id <= 0 ||
            target == null ||
            target.stats.IsDead()
        )
        {
            return;
        }

        StatusEffect template =
            StatusEffectFactory.Create(
                skill.status_effect_id,
                caster
            );

        if (template == null)
            return;

        StatusEffect effect = template.Clone();

        target.AddEffect(effect);

        ui.ShowMessage(
            target.stats.name +
            " bị " +
            effect.effectType
        );

        Debug.Log(
            $"[StatusEffect] " +
            $"{target.stats.name} nhận " +
            $"{effect.effectType}"
        );
    }
    public void UseSkill(SkillData uiSkill)
    {
        if (currentUnit == null || !currentUnit.isPlayer)
        {
            Debug.LogWarning($"[UseSkill] Bỏ qua: currentUnit null hoặc không phải người chơi. uiSkillId={(uiSkill != null ? uiSkill.id : -1)}");
            return;
        }

        var skill = currentUnit.runtimeSkills.FirstOrDefault(s => s.id == uiSkill.id);
        if (skill == null)
        {
            Debug.LogError($"[UseSkill] Không tìm thấy skill id={uiSkill?.id} trong danh sách skill của {currentUnit.stats.name}");
            return;
        }

        var attacker = currentUnit.stats;
        var targetUnit = selectedTarget;
        var caster = currentUnit;
        

        // Kiểm tra cooldown
        if (skill.currentCooldown > 0)
        {
            ui.ShowMessage(skill.name + " chưa hồi, còn " + skill.currentCooldown + " lượt!");
            Debug.Log($"[UseSkill] Skill {skill.name} bị chặn do cooldown còn {skill.currentCooldown}");
            return;
        }

        ui.ShowMessage(attacker.name + " dùng " + skill.name + "!");

        //void ApplyDOT(SkillData skill, CharacterStats attacker, BattleUnit target)
        //{
        //    if (target == null) return;

        //    int dotDamage = Mathf.RoundToInt(attacker.GetAttack() * (skill.power / 100f));

        //    target.stats.AddDOT(
        //      damagePerTurn: dotDamage,
        //      turns: 3,
        //      source: attacker );

        //    ui.ShowMessage($"{attacker.name} gây Độc lên {target.stats.name}!");
        //}

        void ApplyCure(RuntimeSkill skill, CharacterStats attacker, BattleUnit target)
        {
            //if (target == null) return;
            //target.stats.dots.Clear();
        }
        if (!TryValidateTarget(caster, targetUnit, skill))
            return;

        ui.ShowMessage($"{attacker.name} dùng {skill.name}!");

        switch (skill.Type)
        {
            case SkillType.Attack:
                if (skill.targetType == TargetType.Single)
                {
                    Debug.Log("[UseSkill] Nhánh: Attack + Single");
                    if (targetUnit == null)
                    {
                        ui.ShowMessage("Hãy chọn mục tiêu trước!");
                        Debug.LogWarning("[UseSkill] Attack Single nhưng chưa chọn mục tiêu");
                        return;
                    }

                    if (skill.rangeType == RangeType.Melee)
                    {
                        Debug.Log("[UseSkill] Sub-nhánh: Melee > MoveAndAttack");
                        StartCoroutine(MoveAndAttack(currentUnit, targetUnit, () =>
                        {
                            int damage = Mathf.RoundToInt(attacker.GetAttack() * (skill.power / 100f));
                            var target = targetUnit.stats;
                            targetUnit.TakeDamage(damage);

                            TryApplyStatusEffect(skill, caster, targetUnit);

                            BattleManager.Instance.AddThreat(attacker, damage);
                            enemyUIItems[enemyTeam.IndexOf(target)].UpdateHP();

                            ui.ShowMessage(attacker.name + " gây " + damage + " sát thương bằng " + skill.name + "!");
                            Debug.Log($"[UseSkill] Damage (melee single): {damage} lên {target.name}");
                            //NextTurn();
                        }));
                    }
                    else // Ranged
                    {
                        Debug.Log("[UseSkill] Sub-nhánh: Ranged → không di chuyển");
                        int damage = Mathf.RoundToInt(attacker.GetAttack() * (skill.power / 100f));
                        var target = targetUnit.stats;
                        targetUnit.TakeDamage(damage);

                        TryApplyStatusEffect(skill, caster, targetUnit);

                        BattleManager.Instance.AddThreat(attacker, damage);
                        enemyUIItems[enemyTeam.IndexOf(target)].UpdateHP();

                        ui.ShowMessage(attacker.name + " gây " + damage + " sát thương tầm xa bằng " + skill.name + "!");
                        Debug.Log($"[UseSkill] Damage (ranged single): {damage} lên {target.name}");
                        NextTurn();
                    }
                }
                else if (skill.targetType == TargetType.AOE)
                {
                    Debug.Log("[UseSkill] Nhánh: Attack + AOE");
                    int hits = 0;
                    int totalDamage = 0;
                    foreach (var enemy in enemyUnits)
                    {
                        if (enemy.stats.IsDead()) continue;
                        int damage = Mathf.RoundToInt(attacker.GetAttack() * (skill.power / 100f));
                        enemy.TakeDamage(damage);

                        TryApplyStatusEffect(skill, caster, enemy);

                        totalDamage += damage;
                        enemyUIItems[enemyTeam.IndexOf(enemy.stats)].UpdateHP();
                        hits++;
                        Debug.Log($"[UseSkill] AOE hit: {enemy.stats.name} nhận {damage}");
                    }
                    BattleManager.Instance.AddThreat(attacker, totalDamage);

                    ui.ShowMessage(attacker.name + $" tung {skill.name}, trúng {hits} mục tiêu!");
                    NextTurn();
                }
                break;

            case SkillType.Heal:
                if (skill.targetType == TargetType.Single)
                {
                    Debug.Log("[UseSkill] Nhánh: Heal + Single");
                    if (targetUnit == null || !targetUnit.isPlayer)
                    {
                        ui.ShowMessage("Hãy chọn đồng đội để hồi máu!");
                        Debug.LogWarning("[UseSkill] Heal Single nhưng target null hoặc không phải đồng minh");
                        return;
                    }

                    var healTarget = targetUnit.stats;
                    int before = healTarget.currentHP;

                    healTarget.currentHP = Mathf.Min(
                        healTarget.currentHP + skill.power,
                        healTarget.maxHP
                    );
                    TryApplyStatusEffect(skill, caster, targetUnit);

                    int actualHeal = healTarget.currentHP - before;

                    targetUnit.ShowHeal(actualHeal);

                    playerUIItems[playerTeam.IndexOf(healTarget)].UpdateHP();

                    ui.ShowMessage(attacker.name + " hồi phục " + skill.power + " HP cho " + healTarget.name + "!");
                    ui.UpdateTeamHP(playerTeam, enemyTeam);
                    Debug.Log($"[UseSkill] Heal Single: {healTarget.name} {before} -> {healTarget.currentHP}");

                    targetUnit.Highlight(false);
                    selectedTarget = null;
                    NextTurn();

                }
                else if (skill.targetType == TargetType.AOE)
                {
                    Debug.Log("[UseSkill] Nhánh: Heal + AOE");
                    int healed = 0;
                    foreach (var ally in playerUnits)
                    {
                        if (ally.stats.IsDead()) continue;

                        int before = ally.stats.currentHP;

                        ally.stats.currentHP = Mathf.Min(
                            ally.stats.currentHP + skill.power,
                            ally.stats.maxHP
                        );

                        int actualHeal = ally.stats.currentHP - before;

                        ally.ShowHeal(actualHeal);

                        playerUIItems[playerTeam.IndexOf(ally.stats)].UpdateHP();
                        healed++;
                        Debug.Log($"[UseSkill] Heal AOE: {ally.stats.name} {before} -> {ally.stats.currentHP}");
                    }

                    ui.UpdateTeamHP(playerTeam, enemyTeam);
                    ui.ShowMessage(attacker.name + $" tung {skill.name}, hồi cho {healed} đồng minh!");
                    NextTurn();
                }
                else if (skill.targetType == TargetType.Self)
                {
                    Debug.Log("[UseSkill] Nhánh: Heal + Self");
                    int before = attacker.currentHP;

                    attacker.currentHP = Mathf.Min(
                        attacker.currentHP + skill.power,
                        attacker.maxHP
                    );

                    int actualHeal = attacker.currentHP - before;
                    currentUnit.ShowHeal(actualHeal);

                    playerUIItems[playerTeam.IndexOf(attacker)].UpdateHP();

                    ui.ShowMessage(attacker.name + " tự hồi phục " + skill.power + " HP!");
                    ui.UpdateTeamHP(playerTeam, enemyTeam);
                    Debug.Log($"[UseSkill] Heal Self: {attacker.name} {before} -> {attacker.currentHP}");
                    NextTurn();
                }
                break;

            case SkillType.Defense:
                Debug.Log("[UseSkill] Nhánh: Defense");
                attacker.isDefending = true;
                attacker.defenseMultiplier = Mathf.Clamp(1f - (skill.power / 100f), 0.2f, 1f);

                ui.ShowMessage(attacker.name + " dựng Khiên Thép, giảm sát thương nhận " + skill.power + "%!");
                Debug.Log($"[UseSkill] Defense: multiplier={attacker.defenseMultiplier}");
                NextTurn();
                break;

            case SkillType.Buff:
                Debug.Log("[UseSkill] Nhánh: Buff");
                int beforeAtk = attacker.attack;
                attacker.attack += skill.power;

                ui.ShowMessage(attacker.name + " tăng sức mạnh tấn công thêm " + skill.power + "!");
                Debug.Log($"[UseSkill] Buff: attack {beforeAtk} -> {attacker.attack}");
                NextTurn();
                break;
            case SkillType.Cure:
                {
                    if (targetUnit == null)
                    {
                        ui.ShowMessage("Hãy chọn đồng đội!");
                        return;
                    }
                    ApplyCure(skill, attacker, targetUnit);
                    ui.ShowMessage($"{targetUnit.stats.name} được giải hiệu ứng!");
                    NextTurn();
                    break;
                }
        }

        // Đặt cooldown
        skill.currentCooldown = skill.cooldown;
        Debug.Log($"[UseSkill] Đặt cooldown: {skill.name} cd={skill.cooldown}, curCd={skill.currentCooldown}");

        ui.UpdateTeamHP(playerTeam, enemyTeam);

        if (selectedTarget != null && skill.Type != SkillType.Attack)
        {
            selectedTarget.Highlight(false);
            selectedTarget = null;
            Debug.Log("[UseSkill] Xóa selectedTarget sau skill không phải Attack");
        }
        CheckBattleEnd();
        ui.HideArtsList();
    }
    void CheckBattleEnd()
    {
        if (battleEnded) return;

        bool allEnemyDead = enemyUnits.All(u => u.stats.IsDead());
        bool allPlayerDead = playerUnits.All(u => u.stats.IsDead());

        if (allEnemyDead)
        {
            battleEnded = true;
            foreach (var enemy in enemyUnits)
            {
                QuestManager.Instance.AddProgress(
                    "Kill",
                    enemy.enemyID
                );
            }

            GiveBattleReward();
            StartCoroutine(HandleWin());

        }
        else if (allPlayerDead)
        {
            battleEnded = true;
            StartCoroutine(HandleLose());
        }
    }

    IEnumerator HandleWin()
    {
        ui.ShowMessage("Victory!");
        yield return new WaitForSeconds(1.5f);

        BattleResult.lastResult = BattleResultType.Win;
        SceneManager.LoadScene("WinScene");
    }

    IEnumerator HandleLose()
    {
        ui.ShowMessage("Defeat...");
        yield return new WaitForSeconds(1.5f);

        BattleResult.lastResult = BattleResultType.Lose;
        SceneManager.LoadScene("LossScene");
    }
    void GiveBattleReward()
    {
        int totalEXP = 0;
        int totalGold = 0;

        foreach (var enemy in enemyTeam)
        {
            totalEXP += enemy.expReward;
            totalGold += enemy.goldReward;
        }

        PlayerProgression.Instance.AddEXP(
            totalEXP
        );

        PlayerProgression.Instance.AddGold(
            totalGold
        );

        ui.ShowMessage(
            $"Received {totalEXP} EXP and {totalGold} Gold"
        );
    }
}
