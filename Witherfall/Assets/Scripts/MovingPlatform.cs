using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] Transform pointA;
    [SerializeField] Transform pointB;
    [SerializeField] float moveSpeed;
    [SerializeField] Rigidbody2D platformRb;
    [SerializeField] Rigidbody2D playerRb;

 
    private Vector3 nextPosition;

    void Start()
    {
        // Initialize nextPosition to pointB at the start
        nextPosition = pointB.position;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, nextPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, nextPosition) < 0.1f)
        {
            // Switch target position
            // if nextPosition is pointA, set it to pointB, else set it to pointA
            nextPosition = (nextPosition == pointA.position) ? pointB.position : pointA.position;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            if (playerRb != null)
            {
                playerRb.velocity = new Vector2(platformRb.velocity.x, platformRb.velocity.y);
            }
            
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.collider.transform.SetParent(null);
        }
    }

}
