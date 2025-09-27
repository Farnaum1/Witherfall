using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour, Iitem
{
    public static event Action<int> OnGemCollect;
    public int gemValue = 3;

    public void Collect()
    {
        OnGemCollect.Invoke(gemValue);
        Destroy(gameObject);
    }
}
