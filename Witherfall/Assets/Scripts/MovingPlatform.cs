using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("MP Attributes")]
    [SerializeField] Transform pointA;
    [SerializeField] Transform pointB;
    [SerializeField] float moveSpeed;

    private Vector3 nextPosition;
    private Vector3 lastPosition;
    private Transform playerOnPlatform;
    ContactPoint2D[] contactBuffer = new ContactPoint2D[4];

    void Start()
    {
        // Initialize nextPosition to pointB at the start
        nextPosition = pointB.position;

        lastPosition = transform.position;
    }

    void Update()
    {
        lastPosition = transform.position;

        // Move Platform
        transform.position = Vector3.MoveTowards(transform.position, nextPosition, moveSpeed * Time.deltaTime);

        // Switch Target Logic
        if (Vector3.Distance(transform.position, nextPosition) < 0.1f)
        {
            // Switch target position
            // if nextPosition is pointA, set it to pointB, else set it to pointA
            nextPosition = (nextPosition == pointA.position) ? pointB.position : pointA.position;
        }

        Vector3 delta = transform.position - lastPosition;

        if (playerOnPlatform != null)
        {
            playerOnPlatform.position += delta;
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;
        {
            int count = collision.GetContacts(contactBuffer);

            if (count > 0)
            {
                // Loop through contacts and check if ANY contact is on top
                for (int i = 0; i < count; i++)
                {
                    if (contactBuffer[i].normal.y < -0.5f)
                    {
                        playerOnPlatform = collision.transform;
                        return;
                    }
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOnPlatform = null;
        }
    }

}
