using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImage : MonoBehaviour
{
    public float lifeTime = 0.25f;

    SpriteRenderer sr;
    Color startColor;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        startColor = sr.color;
    }

    void Update()
    {
        float alpha = sr.color.a - (Time.deltaTime / lifeTime);
        sr.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

        if (alpha <= 0)
        {
            Destroy(gameObject);
        }
    }
}
