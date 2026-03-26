using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;

    public GameObject panel;

    public TextMeshProUGUI txtName;
    public TextMeshProUGUI txtType;
    public TextMeshProUGUI txtTarget;
    public TextMeshProUGUI txtRange;
    public TextMeshProUGUI txtPower;
    public TextMeshProUGUI txtCooldown;
    public Image iconImage;
    public SkillIconDatabase skillIconDB;
    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void ShowTooltip(SkillData skill, Vector3 position)
    {
        if (skill == null)
        {
            Debug.LogError("Skill NULL trong ShowTooltip!");
            return;
        }

        if (panel == null)
        {
            Debug.LogError("Tooltip PANEL ch?a ???c gán!");
            return;
        }

        Debug.Log("Show tooltip cho skill: " + skill.name);

        txtName.text = skill.name;

        txtType.text = $"Type: {skill.Type}";
        txtTarget.text = $"Target: {skill.targetType}";
        txtRange.text = $"Range: {skill.rangeType}";
        txtPower.text = $"Power: {skill.power}";
        txtCooldown.text = $"Cooldown: {skill.cooldown}";

        if (skillIconDB != null && iconImage != null)
        {
            var icon = skillIconDB.GetIconById(skill.id);

            if (icon != null)
            {
                iconImage.sprite = icon;
                iconImage.enabled = true;
            }
            else
            {
                iconImage.enabled = false;
                Debug.LogWarning($"Tooltip: không tìm th?y icon cho id={skill.id}");
            }
        }

        panel.transform.position = Input.mousePosition + new Vector3(140, 40, 0);

        panel.SetActive(true);
    }


    public void HideTooltip()
    {
        panel.SetActive(false);
    }
}
