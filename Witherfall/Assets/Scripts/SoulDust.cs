using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulDust : MonoBehaviour, Iitem
{
    // Define a static event to notify when a soul is collected      
    public static event Action<int> OnSoulCollect;
    public int soulValue = 3;

    public void Collect()
    {
        // Invoke the event to notify subscribers and pass the soul value
        OnSoulCollect.Invoke(soulValue);
        Destroy(gameObject);
    }
}
