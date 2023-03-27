using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BreakApart : MonoBehaviour
{
    // Public variables 
    public float _disableAfterTime = 15f;
    public float _explosionForce = 10f;
    public float _constantForce = 5f, _constantTorque = 5f;
    public GameObject _explosionParticlePrefab; // the particle effect prefab
    public GameObject _explosion2ParticlePrefab; // shorting out effect


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
        foreach (Transform _child in transform)
        {
            // Check if the child has a MeshRenderer component
            //MeshRenderer _meshRenderer0 = child.gameObject.GetComponent<MeshRenderer>();
            // if (child.gameObject != null && _meshRenderer0 != null && child.gameObject.activeSelf)
            if (_child.gameObject != null && _child.gameObject.activeSelf)

            {
                _spaceshipChildObjects.Add(_child);
                //child.gameObject.SetActive(false);
            }

            foreach (Transform _grandchild in _child)
            {
                // Check if the child has a MeshRenderer component
                //MeshRenderer _meshRenderer1 = grandchild.gameObject.GetComponent<MeshRenderer>();
                // if (grandchild.gameObject != null && _meshRenderer1 != null && grandchild.gameObject.activeSelf)
                if (_grandchild.gameObject != null && _grandchild.gameObject.activeSelf)

                {
                    _spaceshipChildObjects.Add(_grandchild);
                    //grandchild.gameObject.SetActive(false);
                }
            }
        }
    }
    void OnTriggerEnter(Collider other)
    {
        // Check if the collision was with a laser fired by the player's spaceship
        if (!_isBreakingApart && other.gameObject.CompareTag("Laser"))
        {
            Debug.Log("BA - Targat Spaceship is breaking Apart");
            // Mark the spaceship as breaking apart
            _isBreakingApart = true;
        }
    }

    void Update()
    {
        if (_isBreakingApart)
        {
            _isBreakingApart = false;
            _finalTargetDestroyed = true;
            StartCoroutine(HandleBreakingApart());
        }
    }

    IEnumerator HandleBreakingApart()
    {
        //Add particle effects first
        StartCoroutine(AddExplosionEffect());
        // Add a Rigidbody component to allow addforce. No Gravity required.
        AddRigidbodies();

        yield return new WaitForSeconds(2.5f);
        PushObjectsApart();

        yield return new WaitForSeconds(0.5f);
        StartCoroutine(AddExplosion2Effect());

        yield return new WaitForSeconds(2f);
        BigExplosionForce();

        yield return new WaitForSeconds(_disableAfterTime);
        StartCoroutine(DisableObjects());
    }

    void AddRigidbodies()
    {
        foreach (Transform _child in _spaceshipChildObjects)
        {
            //check child has rigidbody
            Rigidbody _rb = _child.gameObject.GetComponent<Rigidbody>();
            if (_rb == null)
            {
                //add rb if child does not have one
                _rb = _child.gameObject.AddComponent<Rigidbody>();
            }
            _rb.useGravity = false;
        }
    }


    void PushObjectsApart()
    {
        // Move each child object away from the spaceship's center
        // though not a while loop it iterates enough times to move objects apart slowly
        foreach (Transform _child in _spaceshipChildObjects)
        {
            Vector3 _direction = _child.position - transform.position;
            float _distance = _direction.magnitude;
            Vector3 moveAmount = _direction.normalized * (_distance * Time.deltaTime);

            _child.position += moveAmount;

            // Apply a constant torque to the objects
            Vector3 torqueDirection = Random.insideUnitSphere.normalized;
            _child.GetComponent<Rigidbody>().AddTorque(torqueDirection * _constantTorque, ForceMode.Impulse);

            // Apply a constant force to push the objects apart
            Vector3 forceDirection = _direction.normalized;
            _child.GetComponent<Rigidbody>().AddForce(forceDirection * _constantForce, ForceMode.Impulse);
        }
    }

    void BigExplosionForce()
    {
        // Add an explosive force to child objects 
        foreach (Transform _child in _spaceshipChildObjects)
        {
            Rigidbody _rb = _child.gameObject.GetComponent<Rigidbody>();
            if (_rb != null)
            {
                _rb.AddExplosionForce(_explosionForce + Random.Range(-20f, 20f), transform.position, 100f);

            }
        }
    }

    IEnumerator DisableObjects()
    {
        Debug.Log("BA - Disable Objects");
        foreach (Transform _obj in _spaceshipChildObjects)
        {
            yield return new WaitForSeconds(0.3f);
            _obj.gameObject.SetActive(false);
        }

    }
    IEnumerator AddExplosionEffect()
    {
        // Cycle through the children of this object and enable a particle effect on each child
        foreach (Transform _child in _spaceshipChildObjects)
        {
            // Instantiate the particle effect prefab as a child of the current child object
            GameObject _particleObject = Instantiate(_explosionParticlePrefab, _child);
            _particleObject.transform.localPosition = Vector3.zero;

            // Enable the particle effect component on the particle object
            ParticleSystem _particleSystem = _particleObject.GetComponent<ParticleSystem>();
            if (_particleSystem != null)
            {
                _particleSystem.Play();
            }

            //gradually add explosion effects
            yield return new WaitForSeconds(0.4f);
        }
    }
    IEnumerator AddExplosion2Effect()
    {
        // Instantiate the particle effect prefab 
        GameObject _particleObject = Instantiate(_explosion2ParticlePrefab, transform);
        _particleObject.transform.localPosition = Vector3.zero;

        // Enable the particle effect component on the particle object
        ParticleSystem _particleSystem = _particleObject.GetComponent<ParticleSystem>();
        if (_particleSystem != null)
        {
            _particleSystem.Play();
        }
        yield return new WaitForSeconds(2f);
        _particleSystem.Stop();
    }
}

