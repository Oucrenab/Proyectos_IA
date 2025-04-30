using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : FOVTarget
{
    public event Action OnFoodDestroy = delegate { };

    protected void Start()
    {
        //base.Start();

        GameManager.Instance.allFood.Add(this);
    }

    public void GetEat()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        OnFoodDestroy();
        GameManager.Instance.allFood.Remove(this);

    }
}
