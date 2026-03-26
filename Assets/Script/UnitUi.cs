using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UnitUI : MonoBehaviour
{
    public TextMeshProUGUI nameText; // Text show name
    public Slider hpBar;// Thanh HP trong UI
    [SerializeField] public TextMeshProUGUI rankText;
    private BattleUnit unit;

    public void Setup(BattleUnit battleUnit)
    {
        this.unit = battleUnit;
        unit.hpBar = hpBar; // Gán thanh HP cho BattleUnit
        UpdateHP();         



        if (unit == null)
        {
            Debug.LogError("UnitUI.Setup: battleUnit is null!");
            return;
        }

        if (unit.stats == null)
        {
            Debug.LogError("UnitUI.Setup: battleUnit.stats is null!");
            return;
        }

        if (nameText != null)
        {
            nameText.text = unit.stats.name;
        }
        else
        {
            Debug.LogWarning("UnitUI.Setup: nameText is not assigned in Inspector.");
        }

        if (hpBar != null)
        {
            hpBar.value = (float)unit.stats.currentHP / unit.stats.maxHP;
            unit.hpBar = hpBar; // Gán thanh HP UI vào BattleUnit
        }
        else
        {
            Debug.LogWarning("UnitUI.Setup: hpBar is not assigned in Inspector.");
        }
    }

    public void UpdateHP()
    {
        if (unit != null && unit.stats != null && hpBar != null)
        {
            float targetValue = (float)unit.stats.currentHP / unit.stats.maxHP;
            StopAllCoroutines();
            StartCoroutine(SmoothHPBar(targetValue));
        }
    }

    IEnumerator SmoothHPBar(float target)
    {
        float speed = 3f;
        while (Mathf.Abs(hpBar.value - target) > 0.01f)
        {
            hpBar.value = Mathf.Lerp(hpBar.value, target, Time.deltaTime * speed);
            yield return null;
        }
        hpBar.value = target;
    }
    public void SetRankTag(BattleTrigger.EnemyRank rank)
    {
        if (rankText == null)
        {
            Debug.LogWarning("[UnitUI] rankText not yet attach!");
            return;
        }

        Debug.Log($"[UnitUI] attach: {rank}");

        switch (rank)
        {
            case BattleTrigger.EnemyRank.Elite:
                rankText.text = "[Elite]";
                rankText.color = Color.yellow;
                break;
            case BattleTrigger.EnemyRank.Boss:
                rankText.text = "[Boss]";
                rankText.color = Color.red;
                break;
            default:
                rankText.text = "";
                break;
        }
    }
}
