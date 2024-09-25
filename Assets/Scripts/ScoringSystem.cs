using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoringSystem : MonoBehaviour
{
    public GameObject[] raycastOrigins; // Array of empty GameObjects to cast rays from
    public int score = 0;
    public float rayDistance = 0.5f; // Distance of raycast from the bean bag to the ground
    public LayerMask boardLayer; // Layer of the Cornhole board for raycast detection
    private bool hasLanded = false;
    void Update()
    {
        if (!hasLanded)
        {
            CheckForBoard();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Board"))
        {
            Debug.Log("Bean bag landed on the board!");
            score += 1;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hole") && this.CompareTag("BeanBag"))
        {
            Debug.Log("Bean bag scored through the hole!");
            score += 3;
        }
    }
    void CheckForBoard()
    {
        foreach (GameObject origin in raycastOrigins)
        {
            RaycastHit hit;

            // Get the direction the ray should cast in (based on the GameObject's forward direction)
            Vector3 rayDirection = origin.transform.forward;

            // Cast the ray from the position of the empty GameObject in the direction of its forward vector
            if (Physics.Raycast(origin.transform.position, rayDirection, out hit, rayDistance, boardLayer))
            {
                if (hit.collider.CompareTag("Board"))
                {
                    Debug.Log("Raycast hit the board!");
                    score += 1; // Add to score if the bean bag is on the board
                    hasLanded = true; // Prevent additional scoring
                    break;
                }
            }
        }
    }

    // Draw Gizmos to visualize the raycasts
    void OnDrawGizmos()
    {
        if (raycastOrigins != null)
        {
            Gizmos.color = Color.green; // Color for the ray gizmos

            foreach (GameObject origin in raycastOrigins)
            {
                // Get the direction the ray should cast in (based on the GameObject's forward direction)
                Vector3 rayDirection = origin.transform.forward;

                // Draw a ray from each empty GameObject in its forward direction
                Gizmos.DrawRay(origin.transform.position, rayDirection * rayDistance);
            }
        }
    }
}



