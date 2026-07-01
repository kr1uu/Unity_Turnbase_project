using UnityEngine;

public class ParalaxMenu : MonoBehaviour
{
    public float offsetMultiplier = 1f;
    public float smoothTime = .3f;

    private Vector2 StartPosition;
    private Vector3 velocity ;
    void Start()
    {
        StartPosition = transform.position;
    }

    void Update()
    {
        Vector2 offset = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        transform.position = Vector3.SmoothDamp(transform.position, StartPosition + (offset * offsetMultiplier), ref velocity, smoothTime);
    }
}
