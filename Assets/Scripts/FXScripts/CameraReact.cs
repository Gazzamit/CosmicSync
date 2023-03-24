using UnityEngine;

public class CameraReact : MonoBehaviour
{
    [SerializeField] private float _thrustMaxOffset = 0.1f;
    [SerializeField] private float _thrustLerpSpeed = 0.1f;
    [SerializeField] private float _rollLerpSpeed = 0.1f;
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
        HandleMovement();
    }

    private void HandleMovement()
    {
        //Slowly lerp ship back to original position so each thrust in time has some movement
        if (SpaceshipControls._allowMovement == false)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, _originalLocalPosition, 0.001f);
        }
        else //ship controls activated 'in-time' with beat so move ship
        {
            // Calculate offset based on local velocity
            Vector3 offset = -Vector3.ClampMagnitude(SpaceshipControls._localVelocity, _thrustMaxOffset);

            // Add offset to target position
            _targetLocalPosition = _originalLocalPosition + offset;

            // Lerp to target position using animation curve
            float lerpAmount = _thrustLerpCurve.Evaluate(_thrustLerpSpeed);
            transform.localPosition = Vector3.Lerp(transform.localPosition, _targetLocalPosition, lerpAmount);

            // Update rotation to target rotation
            transform.localRotation = _targetLocalRotation;
        }
    }
}
