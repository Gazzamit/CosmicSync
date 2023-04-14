using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float shakeIntensity = 0.1f;
    [SerializeField] private float shakeLerpSpeed = 3f;
    [SerializeField] private float _minimunShakeDuration = 0.5f;
    public Transform _cameraTransform;

    private bool isShaking = false;
    private Vector3 initialPosition;

    void Start()
    {
        _cameraTransform = transform;
        initialPosition = _cameraTransform.localPosition; // Set the initial position
    }


    void Update()
    {
        if (SpaceshipControls._LaserFiringStartShake == true && isShaking == false)
        {
            SpaceshipControls._LaserFiringStartShake = false;
            initialPosition = _cameraTransform.localPosition; // Set the initial position again if needed
            isShaking = true;
            StartCoroutine(Shake(_minimunShakeDuration)); // Shake for the specified duration
        }
    }

    IEnumerator Shake(float duration)
    {

        float endTime = Time.time + duration;

        while (Time.time < endTime && isShaking)
        {
            Vector3 _targetPosition = initialPosition + new Vector3(0f,0f,Random.Range(shakeIntensity, shakeIntensity*1.25f));
            _cameraTransform.localPosition = Vector3.Lerp(_cameraTransform.localPosition, _targetPosition, Time.deltaTime * shakeLerpSpeed);

            yield return null;
        }

        _cameraTransform.localPosition = initialPosition;
        isShaking = false;
    }

}
