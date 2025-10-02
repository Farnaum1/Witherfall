using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class HoldToLoad : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;
    private Animator animator;
    private Gate gate;
    private bool isCollided;
    private bool hasKey;
    private bool isHolding = false;

    public static event Action OnHoldComplete;

    // Start is called before the first frame update
    void Start()
    {
        // Get the Animator component
        animator = GetComponent<Animator>();

        // Find the Gate script in the scene
        gate = FindObjectOfType<Gate>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the player is colliding with the gate
        isCollided = gate.IsPlayerCollided;

        // Check if the player has the key
        hasKey = gate.KeyCheck;

        // Check if the player is at the door and has the key
        if (isHolding && isCollided && hasKey)
        {
            sr.enabled = true;
            animator.SetTrigger("isHolding");

            // Invoke the OnHoldComplete event after the animation duration (assuming 1 second here)
            OnHoldComplete.Invoke();

        }
        else 
        {
            sr.enabled = false;
        }
    }

    public void OnHold(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isHolding = true;
        }
        else if (context.canceled)
        {
            ResetHold();
        }
    }
    private void ResetHold()
    {
        isHolding = false;
    }


}
