using UnityEngine;
using UnityEngine.InputSystem;

public class ClickManager : MonoBehaviour
{
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                var unit = hit.collider.GetComponent<BattleUnit>();
                if (unit != null)
                {
                    Debug.Log("Raycast hit unit: " + unit.stats.name);
                    BattleManager.Instance.OnUnitClicked(unit);
                }
            }
        }
    }
}