using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour, Iitem
{
    // Define a static event to notify when a key is collected
    public static event Action<bool> OnKeyCollect;
    public bool hasKey;

    public void Collect()
    {
        // Invoke the event to notify subscribers and pass the key status
        hasKey = true;
        OnKeyCollect?.Invoke(hasKey);
        Destroy(gameObject);
    }
}

