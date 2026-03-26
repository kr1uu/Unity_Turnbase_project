using UnityEngine;

public class CameraZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
        if (cam != null)
        {
            cam.cameraBounds = GetComponent<BoxCollider2D>();
            cam.UpdateBounds();
        }
    }
}
