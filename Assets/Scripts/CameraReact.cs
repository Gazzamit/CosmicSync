using UnityEngine;

public class CameraReact : MonoBehaviour
{
    [SerializeField] private float _thrustMaxOffset = 0.01f;
    [SerializeField] private float _thrustLerpSpeed = 0.01f;
    [SerializeField] private float _rollLerpSpeed = 0.01f;
    [SerializeField] private float _rollMaxAngle = 15f;
    [SerializeField] private AnimationCurve _thrustLerpCurve;

    private Vector3 _originalLocalPosition;
    private Vector3 _targetLocalPosition;
    private Quaternion _targetLocalRotation;
    private Transform _spaceshipTransform;

    private void Start()
    {
        _originalLocalPosition = transform.localPosition;
        _targetLocalRotation = transform.localRotation;
        _spaceshipTransform = transform.parent;
    }

    private void LateUpdate()
    {
        //HandleRoll();
        HandleThrust();
    }

    private void HandleRoll()
    {
        Vector3 spaceshipLocalVelocity = _spaceshipTransform.InverseTransformDirection(_spaceshipTransform.GetComponent<Rigidbody>().velocity);

        // Calculate roll angle based on spaceship's local velocity
        float rollAngle = Mathf.Clamp(Mathf.Rad2Deg * Mathf.Atan2(spaceshipLocalVelocity.z, spaceshipLocalVelocity.x), -_rollMaxAngle, _rollMaxAngle);

        // Calculate rotation based on roll angle
        Quaternion rollRotation = Quaternion.Euler(0f, -rollAngle, 0f);

        // Lerp to target rotation
        _targetLocalRotation = Quaternion.Lerp(_targetLocalRotation, rollRotation, _rollLerpSpeed);
    }

    private void HandleThrust()
    {
        Vector3 spaceshipLocalVelocity = _spaceshipTransform.InverseTransformDirection(_spaceshipTransform.GetComponent<Rigidbody>().velocity);

        // Calculate offset based on local velocity
        Vector3 offset = -Vector3.ClampMagnitude(spaceshipLocalVelocity, _thrustMaxOffset);

        // Add offset to target position
        _targetLocalPosition = _originalLocalPosition + offset;

        // Lerp to target position using animation curve
        float lerpAmount = _thrustLerpCurve.Evaluate(_thrustLerpSpeed);
        transform.localPosition = Vector3.Lerp(transform.localPosition, _targetLocalPosition, lerpAmount);

        // Update rotation to target rotation
        transform.localRotation = _targetLocalRotation;
    }
}
