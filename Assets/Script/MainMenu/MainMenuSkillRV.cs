using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class SkillSlotUI :
    MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    SkillData currentSkill;

    public Image icon;

    public TMP_Text skillName;
    public TMP_Text skillType;

    public SkillIconDatabase iconDatabase;

    public void Setup(SkillData skill)
    {
        currentSkill = skill;

        skillName.text = skill.name;

        skillType.text =
            skill.Type.ToString();

        Sprite skillIcon = iconDatabase.GetIconById(skill.id);

        if (skillIcon != null)
        {
            icon.sprite = skillIcon;
            icon.enabled = true;
        }
        else
        {
            icon.enabled = false;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipManager.Instance.ShowTooltip(
            currentSkill,
            transform.position
        );
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Instance.HideTooltip();
    }
}