using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    public float speed = 200f;
    public int damage = 1;
    public float pushForce = 10f;

    void Update()
    {
        transform.Rotate(0f, 0f, speed * Time.deltaTime);
    }

}
