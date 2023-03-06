using UnityEngine;
using System.Collections.Generic;

public class BreakApart : MonoBehaviour
{
    public float _breakApartTime = 5f;
    public float _explosionForce = 10f;
    public float _constantForce = 5f;

    private List<Transform> _spaceshipChildObjects;
    private float _startTime;
    private bool _isBreakingApart = false;

    void Start()
    {
        _spaceshipChildObjects = new List<Transform>();

        //add child and grandchild transforms
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
            float t = (Time.time - _startTime) / _breakApartTime;

            Debug.Log("Breaking Apart");

            foreach (Transform child in _spaceshipChildObjects)
            {
                Vector3 direction = child.position - transform.position;
                float distance = direction.magnitude;
                Vector3 moveAmount = direction.normalized * (distance * t);
                child.position += moveAmount * Time.deltaTime;
                child.Rotate(Random.insideUnitSphere * Time.deltaTime * 100f); // add some random rotation to the objects

                Vector3 forceDirection = direction.normalized;
                child.GetComponent<Rigidbody>().AddForce(forceDirection * _constantForce * Time.deltaTime, ForceMode.Impulse);
            }

            // disable the objects after 15 seconds
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
        Debug.Log("Laser Collider");
        if (!_isBreakingApart)
        {
            if (other.gameObject.CompareTag("Laser")) // check if the collision was with an explosion object
            {
                _isBreakingApart = true;
                _startTime = Time.time;

                //ad rb so that force can be applied. No Gravity required.
                foreach (Transform child in _spaceshipChildObjects)
                {
                    Rigidbody rb = child.gameObject.AddComponent<Rigidbody>();
                    rb.useGravity = false;
                    rb.AddExplosionForce(_explosionForce, transform.position, 10f); // add explosive force
                }
            }
        }
    }
}
