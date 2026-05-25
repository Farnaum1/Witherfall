using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collided object has a component that implements Iitem
        Iitem item = collision.GetComponent<Iitem>();

        // If it does, call the Collect method
        if (item != null)
        {
            item.Collect();
        }
    }
}
