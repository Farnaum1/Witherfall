using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashUI : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement player;
    [SerializeField] Image dashFill;
    [SerializeField] Image dashGlow;
    [SerializeField] Image spinGlow;
    [SerializeField] Image spinGlow2;
    

    [Header("Visual")]
    public float glowPulseSpeed = 3f;
    public float glowMinAlpha = 0.3f;
    public float glowMaxAlpha = 0.8f;
    [SerializeField] float spinGlowSpeed;

    void Update()
    {
        if (player == null) return;

        // cooldown progress from player
        float fill = player.GetDashFill();
        dashFill.fillAmount = fill;

        if (fill >= 0.999f)
        {
            dashGlow.enabled = true;
            spinGlow.enabled = true;

            float t = (Mathf.Sin(Time.time * glowPulseSpeed) + 1f) * 0.5f;
            Color c = dashGlow.color;
            c.a = Mathf.Lerp(glowMinAlpha, glowMaxAlpha, t);
            dashGlow.color = c;

            spinGlow.transform.Rotate(0f, 0f, spinGlowSpeed * Time.deltaTime);
            Color spinColor = spinGlow.color;
            spinColor.a = Mathf.Lerp(glowMinAlpha, glowMaxAlpha, t);

            spinGlow2.transform.Rotate(0f, 0f, spinGlowSpeed * Time.deltaTime);
            Color spinColor2 = spinGlow2.color;
            spinColor.a = Mathf.Lerp(glowMinAlpha, glowMaxAlpha, t);
        }
        else
        {
            dashGlow.enabled = false;
        }
    }
}
