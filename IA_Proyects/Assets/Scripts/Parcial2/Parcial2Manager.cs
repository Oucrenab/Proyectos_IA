using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parcial2Manager : MonoBehaviour
{
    public static Parcial2Manager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
}
