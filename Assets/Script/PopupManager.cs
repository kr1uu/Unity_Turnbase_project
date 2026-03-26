using UnityEngine;
using static DamagePopup;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;

    public DamagePopup popupPrefab;
    public Canvas canvas;

    void Awake()
    {
        Instance = this;
    }

    public void ShowDamage(Vector3 worldPos, int amount,DamagePopup.PopupType type)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        DamagePopup popup =
            Instantiate(popupPrefab, canvas.transform);

        popup.transform.position = screenPos;
        popup.Setup(amount, type);
    }
}
