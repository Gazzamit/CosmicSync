using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float _shakeIntensity = 0.1f;
    [SerializeField] private float _shakeLerpSpeed = 3f;
    [SerializeField] private float _minimunShakeDuration = 0.5f;
    public Transform _cameraTransform;

    private bool _isShaking = false;
    private Vector3 _initialPosition;

    void Start()
    {
        _cameraTransform = transform;
        _initialPosition = _cameraTransform.localPosition; // Set the initial position
    }


    void Update()
    {
        //for startDialogue
        if (SpaceshipControls._LaserFiringStartShake == true && _isShaking == false && GameManagerDDOL._doWelcome == true)
        {
            SpaceshipControls._LaserFiringStartShake = false;
            _initialPosition = _cameraTransform.localPosition; // Set the initial position again if needed
            _isShaking = true;
            StartCoroutine(Shake(13f)); // Shake for required duration for startDialogue
        }
        else if (SpaceshipControls._LaserFiringStartShake == true && _isShaking == false)
        {
            SpaceshipControls._LaserFiringStartShake = false;
            _initialPosition = _cameraTransform.localPosition; // Set the initial position again if needed
            _isShaking = true;
            StartCoroutine(Shake(_minimunShakeDuration)); // Shake for the specified duration
        }


    }

    IEnumerator Shake(float _duration)
    {
        Debug.Log("Camera Shaking Now for: " + _duration);
        float _endTime = Time.time + _duration;

        while (Time.time < _endTime && _isShaking)
        {
            Vector3 _targetPosition = _initialPosition + new Vector3(0f, 0f, Random.Range(_shakeIntensity, _shakeIntensity * 1.25f));
            _cameraTransform.localPosition = Vector3.Lerp(_cameraTransform.localPosition, _targetPosition, Time.deltaTime * _shakeLerpSpeed);

            yield return null;
        }

        _cameraTransform.localPosition = _initialPosition;
        _isShaking = false;
    }

}
