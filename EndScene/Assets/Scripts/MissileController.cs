using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileController : MonoBehaviour
{
    public float moveSpeed = 400f;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Setting velocity directly on the Rigidbody
        rb.linearVelocity = transform.forward * moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        // Destroy the missile if it passes a certain z-position
        if (transform.position.z > AsteroidManager.Instance.asteroidSpawnDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Asteroid"))
        {
            other.gameObject.GetComponent<AsteroidController>().DestroyAsteroid();
            Destroy(gameObject);  // Destroy the missile on collision
        }
    }
}
