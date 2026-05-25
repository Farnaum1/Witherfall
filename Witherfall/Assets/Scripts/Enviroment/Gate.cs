using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Gate : MonoBehaviour
{
    private Key key;
    private Animator animator;
    public bool KeyCheck;
    public bool IsPlayerCollided = false;

    [Header("Door Lock")]
    [SerializeField] GameObject lockedImage;
    [SerializeField] GameObject unlockedImage;

    [Header("Text")]
    [SerializeField] GameObject holdE;

    [Header("Key Icon HUD")]
    [SerializeField] GameObject keyIcon;


    void Start()
    {

        // Find the script Key in the scene
        key = FindObjectOfType<Key>();

        // Get the Animator component
        animator = GetComponent<Animator>();

        // Set the locked image active and unlocked image inactive at the start
        lockedImage.SetActive(true);
        unlockedImage.SetActive(false);
    }

    private void FixedUpdate()
    {
        // Get the haskey boolean from the Key script
        KeyCheck = key.hasKey;

        DoorCondition();

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            IsPlayerCollided = true;
            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            IsPlayerCollided = false;
            holdE.gameObject.SetActive(false);
        }
    }

    private void DoorCondition()
    {
        if (KeyCheck)
        {
            // If the player has the key, show the unlocked image and hide the locked image
            unlockedImage.SetActive(true);
            lockedImage.SetActive(false);

            // Show the key icon on the HUD
            keyIcon.SetActive(true);

            if (IsPlayerCollided)
            {
                // Show the hold E text when the player is near the door
                holdE.gameObject.SetActive(true);

                if (Input.GetKey(KeyCode.E))
                {
                    // Open the door when the player presses E
                    animator.SetBool("isOpen", true);

                    // Hide the key icon on the HUD
                    keyIcon.SetActive(false);
                }
                
            }
        }

    }

}



