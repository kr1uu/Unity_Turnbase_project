using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    public CharacterStats stats;
    public List<SkillData> skills = new List<SkillData>();

    public string aiProfile;
    public BattleAI.UtilityProfile utilityProfile;

    public Slider hpBar;

    public bool isPlayer;

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
        ApplyVisualById(stats.id);
    }
    void ResizeColliderToSprite()
    {
        if (sr.sprite == null || boxCol == null)
            return;

        boxCol.size = sr.sprite.bounds.size;
        boxCol.offset = sr.sprite.bounds.center;
    }


    void ApplyVisualById(int id)
    {
        if (spriteDB == null)
        {
            Debug.LogError("aint'n have CharacterSpriteDatabase!");
            return;
        }

        var sp = spriteDB.GetSpriteById(id);
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

    public void TakeDamage(int amount, DamagePopup.PopupType type = DamagePopup.PopupType.Normal)
    {
        stats.TakeDamage(amount);
        if (stats.currentHP < 0) stats.currentHP = 0;

        PlayAnim("Hurt");

        Vector3 popupPos = transform.position + new Vector3(0, 2f, 0);

        PopupManager.Instance.ShowDamage(
            popupPos,
            amount,
            type
        );
        Debug.Log($"[TakeDamage] {stats.name} amount={amount} type={type}");

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
}