using UnityEngine;

public class CharacterView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private CharacterSpriteDatabase spriteDatabase;
    public float targetHeight = 200f;
    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    public void Init(int id)
    {
        Debug.Log("Init called, ID = " + id);

        if (spriteRenderer == null)
        {
            Debug.LogError("spriteRenderer = NULL");
            return;
        }

        if (spriteDatabase == null)
        {
            Debug.LogError("spriteDatabase = NULL");
            return;
        }

        Sprite sp = spriteDatabase.GetSpriteById(id);

        if (sp == null)
        {
            Debug.LogError("Sprite NULL cho ID = " + id);
            return;
        }

        spriteRenderer.sprite = sp;
        Debug.Log("Sprite assigned: " + sp.name);
        ResizeToHeight();
    }
    void ResizeToHeight()
    {
        if (spriteRenderer.sprite == null) return;

        float spritePixelHeight = spriteRenderer.sprite.rect.height;
        float ppu = spriteRenderer.sprite.pixelsPerUnit;

        float spriteWorldHeight = spritePixelHeight / ppu;

        float scale = targetHeight / spriteWorldHeight;

        transform.localScale = Vector3.one * scale;
    }

}
