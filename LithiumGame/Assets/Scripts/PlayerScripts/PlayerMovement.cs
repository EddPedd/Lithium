using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    public float moveSpeed = 5f;
    private Vector3 targetPosition;
    private bool isMoving;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Get the mouse position in world coordinates
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = 0f; // Ensure the z-coordinate is set to 0 for 2D movement

            // Calculate the direction to move towards the target position
            Vector3 direction = (targetPosition - transform.position).normalized;

            // Start moving the character
            isMoving = true;
        }

        if (isMoving)
        {
            // Calculate the distance to the target position
            float distance = Vector3.Distance(transform.position, targetPosition);

            if (distance > 0.1f)
            {
                // Move the character towards the target position
                transform.position += (targetPosition - transform.position).normalized * moveSpeed * Time.deltaTime;
            }
            else
            {
                // Stop moving when the target position is reached
                isMoving = false;
            }
        }
    }



}
