using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RocketController : MonoBehaviour
{
    private float speed = 20f;
    private float rotationSpeed = 20f;
    private float raycastDistance = 50f; // Increase the raycast distance

    public TMP_Text timerText;
    public Transform targetLocation;
    public GameObject spaceObject; // Reference to the space GameObject

    private bool hasReachedTarget = false;
    private float timeRemaining = 30f;
    private bool hasWon = false;

    void Update()
    {
        Vector3 directionToTarget = (targetLocation.position - transform.position).normalized;

        // Perform raycasts in multiple directions
        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, raycastDistance);
        bool asteroidInFront = false;

        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("Asteroid"))
            {
                asteroidInFront = true;
                break;
            }
        }

        hits = Physics.RaycastAll(transform.position, -transform.right, raycastDistance);
        bool asteroidToLeft = false;

        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("Asteroid"))
            {
                asteroidToLeft = true;
                break;
            }
        }

        hits = Physics.RaycastAll(transform.position, transform.right, raycastDistance);
        bool asteroidToRight = false;

        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("Asteroid"))
            {
                asteroidToRight = true;
                break;
            }
        }

        if (!hasReachedTarget)
        {
            // No asteroid detected, rotate towards the target
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Move forward at a constant speed
            transform.Translate(Vector3.forward * speed * Time.deltaTime);

            // Restrict movement within the spaceObject
            Vector3 clampedPosition = new Vector3(
                Mathf.Clamp(transform.position.x, spaceObject.GetComponent<Renderer>().bounds.min.x, spaceObject.GetComponent<Renderer>().bounds.max.x),
                transform.position.y,
                Mathf.Clamp(transform.position.z, spaceObject.GetComponent<Renderer>().bounds.min.z, spaceObject.GetComponent<Renderer>().bounds.max.z)
            );

            transform.position = clampedPosition;
        }

        if (asteroidInFront)
        {
            // Rotate away from the asteroid
            Vector3 avoidAsteroidDirection = Vector3.Cross(Vector3.up, Vector3.one);
            Quaternion targetRotation = Quaternion.LookRotation(avoidAsteroidDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else if (asteroidToLeft && !asteroidToRight)
        {
            // Rotate to the right to avoid the asteroid on the left
            Quaternion targetRotation = Quaternion.LookRotation(transform.right);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else if (!asteroidToLeft && asteroidToRight)
        {
            // Rotate to the left to avoid the asteroid on the right
            Quaternion targetRotation = Quaternion.LookRotation(-transform.right);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (timeRemaining > 0f && !hasWon)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerText();
        }
        if (timeRemaining <= 0f && !hasWon)
        {
            timerText.text = "Lose";
            hasReachedTarget = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == targetLocation.gameObject)
        {
            // Cube has collided with the target location, stop the movement
            hasReachedTarget = true;
            hasWon = true;
            timerText.text = "Win";
        }
    }

    private void UpdateTimerText()
    {
        int seconds = Mathf.CeilToInt(timeRemaining);
        timerText.text = seconds.ToString();
    }
}