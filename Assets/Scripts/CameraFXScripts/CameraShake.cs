using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float shakeIntensity = 0.1f;
    [SerializeField] private float shakeLerpSpeed = 3f;
    [SerializeField] private float _minimunShakeDuration = 0.5f;

    private bool isShaking = false;
    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.localPosition;
    }

    private void Update()
    {
        if (SpaceshipControls._laserFiringLeft && !isShaking)
        {
            isShaking = true;
            StartCoroutine(Shake(_minimunShakeDuration)); // Shake for 2 seconds
        }
    }

    IEnumerator Shake(float duration)
    {
        float endTime = Time.time + duration;

        while (Time.time < endTime && isShaking)
        {
            Vector3 targetPosition = initialPosition + Random.insideUnitSphere * shakeIntensity;
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * shakeLerpSpeed);

            yield return null;
        }

        transform.localPosition = initialPosition;
        isShaking = false;
    }
}
