using UnityEngine;
using System.Collections.Generic;

public class BreakApart : MonoBehaviour
{
    // Public variables 
    public float _breakApartTime = 5f;
    public float _explosionForce = 10f;
    public float _constantForce = 5f;

    // A list of all the child objects that will be broken apart
    private List<Transform> _spaceshipChildObjects;

    // The time at which the object starts breaking apart
    private float _startTime;
    private bool _isBreakingApart = false;
    public static bool _finalTargetDestroyed = false;

    void Start()
    {
        _spaceshipChildObjects = new List<Transform>();

        // Add each child and grandchild transform to the list
        foreach (Transform child in transform)
        {
            _spaceshipChildObjects.Add(child);
            foreach (Transform grandchild in child)
            {
                _spaceshipChildObjects.Add(grandchild);
            }
        }
    }

    void Update()
    {
        if (_isBreakingApart)
        {
            // Calculate how far along the breaking apart process is (0-1)
            float t = (Time.time - _startTime) / _breakApartTime;

            // Move each child object away from the spaceship's center
            foreach (Transform child in _spaceshipChildObjects)
            {
                Vector3 direction = child.position - transform.position;
                float distance = direction.magnitude;
                Vector3 moveAmount = direction.normalized * (distance * t);
                child.position += moveAmount * Time.deltaTime;

                // Add some random rotation to the objects
                child.Rotate(Random.insideUnitSphere * Time.deltaTime * 100f); 

                // Apply a constant force to push the objects apart
                Vector3 forceDirection = direction.normalized;
                child.GetComponent<Rigidbody>().AddForce(forceDirection * _constantForce * Time.deltaTime, ForceMode.Impulse);
            }

            // Disable the objects after 15 seconds
            if (Time.time - _startTime > 15f)
            {
                foreach (Transform child in _spaceshipChildObjects)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!_isBreakingApart)
        {
            // Check if the collision was with a laser fired by the player's spaceship
            if (other.gameObject.CompareTag("Laser"))
            {
                // Mark the spaceship as breaking apart
                _isBreakingApart = true;
                _startTime = Time.time;
                _finalTargetDestroyed = true;

                // Add a Rigidbody component to allow addforce. No Gravity required.
                foreach (Transform child in _spaceshipChildObjects)
                {
                    Rigidbody _rb = child.gameObject.AddComponent<Rigidbody>();
                    _rb.useGravity = false;

                    // Add an explosive force to child objects 
                    _rb.AddExplosionForce(_explosionForce, transform.position, 10f); 
                }
            }
        }
    }
}
