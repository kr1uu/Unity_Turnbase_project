using UnityEngine;

public class ClickableUnit : MonoBehaviour
{
    private BattleUnit unit;

    void Awake()
    {
        unit = GetComponent<BattleUnit>();
    }
    void OnMouseDown()
    {
        Debug.Log("CLICKED ON: " + gameObject.name);
        Debug.Log("Clicked: " + unit.stats.name);
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.OnUnitClicked(unit);
        }
    }
}