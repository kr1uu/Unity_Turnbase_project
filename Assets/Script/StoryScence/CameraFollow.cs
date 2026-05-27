using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;
    public BoxCollider2D cameraBounds;

    private float minX, maxX, minY, maxY;
    private float camHalfWidth, camHalfHeight;

    void Start()
    {
        camHalfHeight = Camera.main.orthographicSize;
        camHalfWidth = camHalfHeight * Camera.main.aspect;

        UpdateBounds();

        if (target == null)
        {
            GameObject player =
                GameObject.FindGameObjectWithTag(
                    "Player"
                );

            if (player != null)
                target = player.transform;
        }
    }

    public void UpdateBounds()
    {
        if (cameraBounds == null) return;

        Bounds b = cameraBounds.bounds;

        minX = b.min.x + camHalfWidth;
        maxX = b.max.x - camHalfWidth;
        minY = b.min.y + camHalfHeight;
        maxY = b.max.y - camHalfHeight;

        // chunk smaller cam scene
        if (minX > maxX)
            minX = maxX = b.center.x;

        if (minY > maxY)
            minY = maxY = b.center.y;
    }

    void LateUpdate()
    {
        if (target == null) return;

        float x = Mathf.Clamp(target.position.x, minX, maxX);
        float y = Mathf.Clamp(target.position.y, minY, maxY);

        Vector3 desiredPos = new Vector3(x, y, transform.position.z);

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPos,
            smoothSpeed * Time.deltaTime
        );
    }
}
