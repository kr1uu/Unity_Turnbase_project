using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoopBackground : MonoBehaviour
{
    public float speed;
    [SerializeField]
    private Renderer bgRenderer;
    void Update()
    {
        bgRenderer.material.mainTextureOffset += new Vector2(speed * Time.deltaTime, 0);
    }
}
