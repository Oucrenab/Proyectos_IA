using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalManager : MonoBehaviour
{
    public FinalManager Instance;

    private void Awake()
    {
        Instance = this;
    }


}
