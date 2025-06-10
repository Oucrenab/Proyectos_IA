using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVTarget : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    protected virtual void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ChangeColor(Color newColor)
    {
        _spriteRenderer.material.color = newColor;
    }
}
