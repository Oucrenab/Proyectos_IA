using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVTarget : MonoBehaviour
{
    SpriteRenderer _spriteR;

    protected virtual void Awake()
    {
        _spriteR = GetComponent<SpriteRenderer>();
    }

    public void ChangeColor(Color newColor)
    {
        _spriteR.material.color = newColor;
    }
}
