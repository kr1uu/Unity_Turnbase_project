using UnityEngine;
using UnityEngine.EventSystems;

public class SkillButton : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    public SkillData skill;

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipManager.Instance.ShowTooltip(
            skill,
            transform.position + new Vector3(120, 0, 0)
        );
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Instance.HideTooltip();
    }
}
