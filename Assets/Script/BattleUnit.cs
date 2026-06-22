using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class BattleUnit : MonoBehaviour
{
    public CharacterStats stats;
    public List<SkillData> skills = new List<SkillData>();
    public List<RuntimeSkill> runtimeSkills = new List<RuntimeSkill>();
    public List<StatusEffect> activeEffects = new List<StatusEffect>();

    [HideInInspector] public Transform statusIconHolder;
    [HideInInspector] public GameObject statusIconPrefab;
    [HideInInspector] public StatusIconDatabase statusIconDB;

    private Dictionary<string, StatusIconUI> statusIcons
        = new Dictionary<string, StatusIconUI>();

    public string aiProfile;
    public BattleAI.UtilityProfile utilityProfile;

    public Slider hpBar;

    public bool isPlayer;

    public int enemyID;

    public Material normalMat;
    public Material highlightMat;

    public Vector3 originalPosition;
    private BoxCollider2D boxCol;
    private SpriteRenderer sr;
    private Animator animator;

    public CharacterSpriteDatabase spriteDB;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        boxCol = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    public void Setup(CharacterStats s, bool player)
    {
        stats = s;
        isPlayer = player;

        activeEffects.Clear();

        foreach (var icon in statusIcons.Values)
        {
            if (icon != null)
                Destroy(icon.gameObject);
        }

        statusIcons.Clear();

        ApplyVisualById(stats.id);
    }
    void ResizeColliderToSprite()
    {
        if (sr.sprite == null || boxCol == null)
            return;

        Bounds bounds = sr.sprite.bounds;

        Vector3 scale = transform.localScale;

        boxCol.size = new Vector2(
            bounds.size.x / Mathf.Abs(scale.x),
            bounds.size.y / Mathf.Abs(scale.y)
        );

        boxCol.offset = bounds.center;
    }

    public void ClearAllEffects()
    {
        activeEffects.Clear();

        RefreshStatusIcons();
    }
    void RefreshStatusIcons()
    {
        // remove icon không còn effect
        List<string> removeList = new List<string>();

        foreach (var pair in statusIcons)
        {
            bool stillExist = activeEffects.Any(
                e => e.effectType == pair.Key
            );

            if (!stillExist)
            {
                Destroy(pair.Value.gameObject);
                removeList.Add(pair.Key);
            }
        }

        foreach (var key in removeList)
        {
            statusIcons.Remove(key);
        }

        // create/update icon
        foreach (var effect in activeEffects)
        {
            if (!statusIcons.ContainsKey(effect.effectType))
            {
                GameObject obj =
                    Instantiate(
                        statusIconPrefab,
                        statusIconHolder
                    );

                Debug.Log("Spawn icon: " + obj.name);

                StatusIconUI ui =
                    obj.GetComponent<StatusIconUI>();

                if (ui == null)
                {
                    Debug.LogError(
                        "StatusIconUI component missing on prefab!"
                    );

                    return;
                }

                if (ui.iconImage == null)
                {
                    Debug.LogError(
                        "iconImage missing on: " + obj.name
                    );
                }

                ui.Setup(
                    statusIconDB.GetIcon(effect.effectType),
                    effect.duration
                );

                statusIcons.Add(effect.effectType, ui);
            }
            else
            {
                statusIcons[effect.effectType]
                    .UpdateDuration(effect.duration);
            }
        }
    }
    public void AddEffect(StatusEffect effect)
    {
        var existing =
       activeEffects.FirstOrDefault(
           e => e.effectType == effect.effectType
       );

        // refresh duration
        if (existing != null)
        {
            existing.duration =
                Mathf.Max(
                    existing.duration,
                    effect.duration
                );
            RefreshStatusIcons();
            return;
        }

        activeEffects.Add(effect);

        Debug.Log(
            stats.name +
            " Have effect " +
            effect.effectType
        );
        RefreshStatusIcons();
    }
    public bool HasEffect(string type)
    {
        return activeEffects.Any(
            e => e.effectType == type
        );
    }
    void ApplyVisualById(int id)
    {
        Debug.Log(
       $"ApplyVisualById {name} | spriteDB={(spriteDB == null ? "NULL" : spriteDB.name)}" );

        var sp = spriteDB.GetSpriteById(id);
        Debug.Log( $"Sprite found = {(sp == null ? "NULL" : sp.name)}");
        if (sp != null)
        {
            sr.sprite = sp;
            ResizeColliderToSprite();
        }

        var anim = spriteDB.GetAnimatorById(id);
        if (anim != null && animator != null)
            animator.runtimeAnimatorController = anim;
    }
    public void ShowHeal(int amount)
    {
        Vector3 popupPos = transform.position + new Vector3(0, 2f, 0);

        PopupManager.Instance.ShowDamage(
            popupPos,
            amount,
            DamagePopup.PopupType.Heal
        );
    }

    public void TakeDamage(
      int amount,
      DamagePopup.PopupType type =
          DamagePopup.PopupType.Normal
  )
    {
        // =========================
        // SHIELD ABSORB
        // =========================

        var shields = activeEffects
            .Where(e => e.effectType == "Shield")
            .ToList();

        foreach (var shield in shields)
        {
            if (amount <= 0)
                break;

            int absorbed =
                Mathf.Min(shield.currentShield, amount);

            shield.currentShield -= absorbed;
            amount -= absorbed;

            Debug.Log(
                $"{stats.name} shield absorb {absorbed}"
            );

            PopupManager.Instance.ShowDamage(
                transform.position + Vector3.up * 2f,
                absorbed,
                DamagePopup.PopupType.Shield
            );

            if (shield.currentShield <= 0)
            {
                activeEffects.Remove(shield);

                Debug.Log(
                    $"{stats.name} shield broken"
                );
            }
        }

        // refresh icon n?u shield b? remove
        RefreshStatusIcons();

        // =========================
        // FULLY BLOCKED
        // =========================

        if (amount <= 0)
        {
            PlayAnim("Block");

            UpdateHPBar();
            return;
        }

        // =========================
        // DEFENDING REDUCTION
        // =========================

        if (stats.isDefending)
        {
            amount =
                Mathf.RoundToInt(
                    amount * stats.defenseMultiplier
                );
        }

        // =========================
        // DEFENSE REDUCTION
        // =========================

        int finalDamage =Mathf.RoundToInt(
        amount *
        (100f / (100f + stats.defense)) );

        finalDamage = Mathf.Max(1, finalDamage);
        // =========================
        // APPLY DAMAGE
        // =========================

        stats.TakeDamage(finalDamage);

        if (stats.currentHP < 0)
            stats.currentHP = 0;

        PlayAnim("Hurt");

        Vector3 popupPos =
            transform.position +
            new Vector3(0, 2f, 0);

        PopupManager.Instance.ShowDamage(
            popupPos,
            finalDamage,
            type
        );

        Debug.Log(
            $"[TakeDamage] " +
            $"{stats.name} finalDamage={finalDamage} type={type}"
        );

        // =========================
        // DEATH
        // =========================

        if (stats.IsDead())
        {
            PlayAnim("Dead");

            Debug.Log(
                $"{stats.name} has died"
            );
        }

        UpdateHPBar();
    }
    public void UpdateHPBar()
    {
        if (hpBar != null && stats != null)
        {
            hpBar.value = (float)stats.currentHP / stats.maxHP;
        }
    }

    public void Highlight(bool active)
    {
        StopAllCoroutines();

        if (active)
        {
            StartCoroutine(BlinkEffect());
        }
        else
        {
            sr.color = Color.white; // reset 
        }
    }
    IEnumerator BlinkEffect()
    {
        while (true)
        {
            sr.color = Color.yellow;
            yield return new WaitForSeconds(0.2f);
            sr.color = Color.white;
            yield return new WaitForSeconds(0.2f);
        }
    }
    public void PlayAnim(string state)
    {
        if (animator == null) return;

        animator.Play(state);
    }

    public void SetAnimBool(string name, bool value)
    {
        if (animator == null) return;

        animator.SetBool(name, value);
    }

    public void TriggerAnim(string name)
    {
        if (animator == null) return;

        animator.SetTrigger(name);
    }
    public void ProcessEffects()
    {
        foreach (var effect in activeEffects.ToList())
        {
            switch (effect.effectType)
            {
                case "Poison":

                    int poisonDamage =
                        Mathf.RoundToInt(
                            stats.currentHP *
                            (effect.power / 100f)
                        );

                    poisonDamage =
                        Mathf.Max(poisonDamage, 1);

                    TakeDamage(
                        poisonDamage,
                        DamagePopup.PopupType.DOT
                    );

                    break;

                case "Burn":

                    int burnDamage =
                        Mathf.RoundToInt(
                            effect.power +
                            (stats.maxHP * 0.03f)
                        );

                    TakeDamage(burnDamage);

                    break;

                case "Bleed":

                    int bleedDamage =
                        Mathf.RoundToInt(
                            stats.GetMaxHP() *
                            (effect.power / 100f)
                        );

                    bleedDamage =
                        Mathf.Max(bleedDamage, 1);

                    TakeDamage(
                        bleedDamage,
                        DamagePopup.PopupType.DOT
                    );

                    break;

                case "Regen":

                    stats.currentHP =
                        Mathf.Min(
                            stats.currentHP + effect.power,
                            stats.maxHP
                        );

                    break;

                case "Weak":

                    if (!effect.applied)
                    {
                        stats.attack -= effect.power;
                        effect.applied = true;
                    }

                    break;

                case "DefBreak":

                    if (!effect.applied)
                    {
                        stats.defense -= effect.power;
                        effect.applied = true;
                    }

                    break;

                case "Fortify":

                    if (!effect.applied)
                    {
                        stats.defense += effect.power;
                        effect.applied = true;
                    }

                    break;

                case "Haste":

                    if (!effect.applied)
                    {
                        stats.speed += effect.power;
                        effect.applied = true;
                    }

                    break;

                case "Shield":

                    if (!effect.applied)
                    {
                        effect.currentShield =
                            effect.power +
                            Mathf.RoundToInt(stats.defense * 0.75f);

                        effect.applied = true;

                        Debug.Log(
                            stats.name +
                            " gain shield " +
                            effect.currentShield
                        );
                    }

                    break;
            }

            effect.duration--;
        }

        // remove expired
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            var e = activeEffects[i];

            if (e.duration > 0)
                continue;

            switch (e.effectType)
            {
                case "Weak":
                    stats.attack += e.power;
                    break;

                case "DefBreak":
                    stats.defense += e.power;
                    break;

                case "Fortify":
                    stats.defense -= e.power;
                    break;

                case "Haste":
                    stats.speed -= e.power;
                    break;
            }

            activeEffects.RemoveAt(i);
        }

        RefreshStatusIcons();
    }
}