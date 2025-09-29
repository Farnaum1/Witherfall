using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour, Iitem
{
    // Define a static event to notify when a gem is collected      
    public static event Action<int> OnGemCollect;
    public int gemValue = 3;

    public void Collect()
    {
        // Invoke the event to notify subscribers and pass the gem value
        OnGemCollect.Invoke(gemValue);
        Destroy(gameObject);
    }
}
