using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BulletCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    private void Awake()
    {
    }
    private void OnEnable()
    {
        GameController.OnAmmoChanged += UpdateAmmoUI;
    }

    private void OnDisable()
    {
        GameController.OnAmmoChanged -= UpdateAmmoUI;
    }
    private void Start()
    {
        // Initialize UI with current ammo
        UpdateAmmoUI(GameController.Instance.projectileAmount);
    }

    private void UpdateAmmoUI(int currentAmmo)
    {
        text.text = "X" + currentAmmo;
    }
}
