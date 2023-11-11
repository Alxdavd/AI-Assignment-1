using UnityEngine;

public class AsteroidGenerator : MonoBehaviour
{
    public GameObject asteroidPrefab;
    public GameObject spaceship; 

    public int numberOfAsteroidsToSpawn = 50;
    public float minYPosition = 5f;
    
    public float minDistanceFromSpaceship = 20f;
    public float minDistanceBetweenAsteroids = 10f; 

    public float minAsteroidScale = 3f;
    public float maxAsteroidScale = 10f;

    private void Start()
    {
        // Get the size of the plane GameObject
        Vector3 planeSize = GetComponent<Renderer>().bounds.size;

        for (int i = 0; i < numberOfAsteroidsToSpawn; i++)
        {
            Vector3 randomPosition = GenerateRandomPosition(planeSize);

            // Ensure that the asteroid is not too close to the spaceship
            while (Vector3.Distance(randomPosition, spaceship.transform.position) < minDistanceFromSpaceship ||
                   IsTooCloseToOtherAsteroid(randomPosition))
            {
                randomPosition = GenerateRandomPosition(planeSize);
            }

            randomPosition.y = minYPosition; // Set the Y position to 5 units

            // Randomize the scale of the asteroid
            float randomScale = Random.Range(minAsteroidScale, maxAsteroidScale);
            Vector3 asteroidScale = new Vector3(randomScale, randomScale, randomScale);

            GameObject asteroid = Instantiate(asteroidPrefab, randomPosition, Quaternion.identity);
            asteroid.transform.localScale = asteroidScale;
        }
    }

    private Vector3 GenerateRandomPosition(Vector3 planeSize)
    {
        Vector3 randomPosition = new Vector3(
            Random.Range(-planeSize.x / 2, planeSize.x / 2), // X position within the plane
            minYPosition, // Y position at 5 units
            Random.Range(-planeSize.z / 2, planeSize.z / 2) // Z position within the plane
        );

        return randomPosition;
    }

    private bool IsTooCloseToOtherAsteroid(Vector3 position)
    {
        // Check if the new asteroid position is too close to any existing asteroid
        GameObject[] asteroids = GameObject.FindGameObjectsWithTag("Asteroid");

        foreach (GameObject asteroid in asteroids)
        {
            if (Vector3.Distance(position, asteroid.transform.position) < minDistanceBetweenAsteroids)
            {
                return true;
            }
        }

        return false;
    }
}