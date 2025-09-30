using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    private Key key;
    private Animator animator;
    [SerializeField] bool KeyCheck;
    [SerializeField] bool IsPlayerCollided = false;


    void Start()
    {

        // Find the script Key in the scene
        key = FindObjectOfType<Key>();

        // Get the Animator component
        animator = GetComponent<Animator>();
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
        }
    }

    private void DoorCondition()
    {
        if (KeyCheck)
        {
            if (IsPlayerCollided && Input.GetKey(KeyCode.E))
            {
                animator.SetBool("isOpen", true);
            }
        }

    }

}



