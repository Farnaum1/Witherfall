using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    private Key key;
    private Animator animator;
    private bool KeyCheck;

    public bool IsPlayerCollided = false;


    void Start()
    {
        // Subscribe to the OnKeyCollect event
        Key.OnKeyCollect += OpenDoor;

        // Find the script Key in the scene
        key = FindObjectOfType<Key>();

        // Get the haskey boolean from the Key script
        KeyCheck = key.hasKey;

        // Get the Animator component
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            IsPlayerCollided = true;
            //Debug.Log(IsPlayerCollided);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player is out of range of the door");
            IsPlayerCollided = false;
        }
    }

    private void OpenDoor(bool KeyCheck)
   
    {
        if (KeyCheck)
        {
            Debug.Log(KeyCheck);

            if (IsPlayerCollided)
            {
                Debug.Log("Door is opening");
                animator.SetBool("isOpen", true);
                Debug.Log("player is within range");
            }
        }

    }
}



