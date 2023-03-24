using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BreakApart : MonoBehaviour
{
    // Public variables 
    public float _breakApartTime = 5f;
    public float _explosionForce = 10f;
    public float _constantForce = 5f;
    public GameObject particlePrefab; // the particle effect prefab


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
            StartCoroutine(AddExplosionEffect());

            // Calculate how far along the breaking apart process is (0-1)
            float t = (Time.time - _startTime) / _breakApartTime;

            // Disable the objects after 15 seconds
            if (Time.time - _startTime > 15f)
            {

                foreach (Transform child in _spaceshipChildObjects)
                {
                    child.gameObject.SetActive(false);
                }
            }
            else
            {
                // Move each child object away from the spaceship's center
                foreach (Transform child in _spaceshipChildObjects)
                {
                    Vector3 _direction = child.position - transform.position;
                    float _distance = _direction.magnitude;
                    Vector3 moveAmount = _direction.normalized * (_distance * t);

                    child.position += moveAmount * Time.deltaTime;

                    // Add some random rotation to the objects
                    child.Rotate(Random.insideUnitSphere * Time.deltaTime * 100f);

                    // Apply a constant force to push the objects apart
                    Vector3 forceDirection = _direction.normalized;
                    child.GetComponent<Rigidbody>().AddForce(forceDirection * _constantForce * Time.deltaTime, ForceMode.Impulse);
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
                    _rb.AddExplosionForce(_explosionForce + Random.Range(20f,-20f), transform.position, 10f);
                }
            }
        }
    }

    IEnumerator AddExplosionEffect()
    {
        // Cycle through the children of this object and enable a particle effect on each child
        foreach (Transform child in transform)
        {
            // Instantiate the particle effect prefab as a child of the current child object
            GameObject particleObject = Instantiate(particlePrefab, child);
            particleObject.transform.localPosition = Vector3.zero;

            // Enable the particle effect component on the particle object
            ParticleSystem particleSystem = particleObject.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                particleSystem.Play();
            }

            //gradually add explosion effects
            yield return new WaitForSeconds(0.8f);
        }
    }

}
