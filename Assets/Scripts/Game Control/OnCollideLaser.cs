using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class OnCollideLaser : MonoBehaviour
{
    // Public variables 
    [SerializeField] private float _disableAfterTime = 15f;
    [SerializeField] private float _explosionForce = 10f;
    [SerializeField] private float _constantForce = 5f, _constantTorque = 5f;
    [SerializeField] private GameObject _explosionParticlePrefab; // the particle effect prefab
    [SerializeField] private GameObject _explosion2ParticlePrefab; // shorting out effect
    [SerializeField] private AudioManager _audioManager;

    // A list of all the child objects that will be broken apart
    private List<Transform> _spaceshipChildObjects;

    // The time at which the object starts breaking apart
    private float _startTime;
    [SerializeField] private bool _isBreakingApart = false;

    void Awake()
    {
        _isBreakingApart = false;
    }

    void Start()
    {
        //list for target spaceship child and grandchild game objects
        _spaceshipChildObjects = new List<Transform>();

        // Add each child and grandchild transform to the list
        foreach (Transform _child in transform)
        {
            if (_child.gameObject != null && _child.gameObject.activeSelf)

            {
                _spaceshipChildObjects.Add(_child);
                //child.gameObject.SetActive(false); //for testing
            }

            foreach (Transform _grandchild in _child)
            {
                if (_grandchild.gameObject != null && _grandchild.gameObject.activeSelf)

                {
                    _spaceshipChildObjects.Add(_grandchild);
                    //grandchild.gameObject.SetActive(false); //for testing
                }
            }
        }
    }
    void OnTriggerEnter(Collider other)
    {
        // Check if the collision was with a laser fired by the player's spaceship
        if (_isBreakingApart == false && other.gameObject.CompareTag("Laser"))
        {
            Debug.Log("OCL 0 - Laser Hit Trigger by collider");
            // Mark the spaceship as breaking apart
            _isBreakingApart = true;
            ScoreManager._instance.TargetDestroyed();

            //Increment index if target already inactive, unless at end of game
            //can only happen if destroyed in wrong order by players
            if (transform.GetSiblingIndex() == NextTargetIndex._nextTargetIndex)
            {
                Debug.Log("OCL - laser Bool Set Destroyed: " + NextTargetIndex._nextTargetIndex);
                //Log destroyed in bool and increment next target index
                NextTargetIndex._targetsDestoryedStaticVar[NextTargetIndex._nextTargetIndex] = true;
                NextTargetIndex._nextTargetIndex++;
                ScoreManager._instance.AddPoints("laserRightOrder");
            }
            else
            {
                //Log destroyed in bool and do not increment next target index
                NextTargetIndex._targetsDestoryedStaticVar[transform.GetSiblingIndex()] = true;
                Debug.Log("OCL - Laser Bool Set, but Target Destroyed Wrong Order: " + transform.GetSiblingIndex());
                ScoreManager._instance.AddPoints("laserWrongOrder");
            }
            StartCoroutine(HandleBreakingApart());

        }
    }

    IEnumerator HandleBreakingApart()
    {
        yield return new WaitForSeconds(0.2f);
        _audioManager.PlayExplosion();
        
        //Add particle effects first
        StartCoroutine(AddExplosionEffect());
        // Add a Rigidbody component to allow addforce. No Gravity required.
        AddRigidbodies();

        yield return new WaitForSeconds(2.5f);
        PushObjectsApart();

        yield return new WaitForSeconds(0.5f);
        StartCoroutine(AddExplosion2Effect());

        yield return new WaitForSeconds(0.2f);
        BigExplosionForce();

        //this will also disable instantiated particle effects
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

    void BigExplosionForce()
    {
        // Add an explosive force to child objects 
        foreach (Transform _child in _spaceshipChildObjects)
        {
            Vector3 _direction = _child.position - transform.position;
            float _distance = _direction.magnitude;
            Vector3 moveAmount = _direction.normalized * (_distance * Time.deltaTime);

            _child.position += moveAmount;

            // Apply a constant torque to the objects
            Vector3 torqueDirection = Random.insideUnitSphere.normalized;
            _child.GetComponent<Rigidbody>().AddTorque(torqueDirection * _constantTorque * 3f, ForceMode.Impulse);

            // Apply a constant force to push the objects apart
            Vector3 forceDirection = _direction.normalized;
            _child.GetComponent<Rigidbody>().AddForce(forceDirection * _explosionForce, ForceMode.Impulse);
        }
    }
    IEnumerator AddExplosion2Effect()
    {
        // Instantiate the particle effect prefab 
        GameObject _particleObject2 = Instantiate(_explosion2ParticlePrefab, transform);
        _particleObject2.transform.localPosition = Vector3.zero;

        // Enable the particle effect component on the particle object
        ParticleSystem _particleSystem2 = _particleObject2.GetComponent<ParticleSystem>();
        if (_particleSystem2 != null)
        {
            _particleSystem2.Play();
        }
        yield return new WaitForSeconds(2f);
        _particleSystem2.Stop();
    }
    IEnumerator DisableObjects()
    {
        // Debug.Log("BA - Disable Objects");
        foreach (Transform _obj in _spaceshipChildObjects)
        {
            yield return new WaitForSeconds(0.3f);
            _obj.gameObject.SetActive(false);

            //disable particle effects
            ParticleSystem _particleSystem = _obj.gameObject.GetComponentInChildren<ParticleSystem>();
            if (_particleSystem != null)
            {
                _particleSystem.Stop();
                Destroy(_particleSystem.gameObject, _particleSystem.main.duration);
            }
        }
        //disable parent spaceship after child objects disabled
        gameObject.SetActive(false);

    }
}

