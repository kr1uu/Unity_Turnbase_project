using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    private float disappearTimer;
    private Color textColor;
    public enum PopupType
    {
        Normal,
        DOT,
        Heal
    }
    public void Setup(int amount, PopupType type)
    {
        if (textMesh == null) return;

        switch (type)
        {
            case PopupType.Normal:
                textMesh.color = Color.white;
                textMesh.text = amount.ToString();
                break;

            case PopupType.DOT:
                textMesh.color = new Color(0.7f, 0.2f, 1f); // pp
                textMesh.text = "-" + amount;
                break;

            case PopupType.Heal:
                textMesh.color = new Color(0.3f, 1f, 0.3f); // green
                textMesh.text = "+" + amount;
                break;
        }

        textColor = textMesh.color;
        disappearTimer = 1f;
    }


    private void Update()
    {
        // up
        transform.position += new Vector3(0, 1f) * Time.deltaTime;

        // dis alpha
        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            textColor.a -= 2f * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}