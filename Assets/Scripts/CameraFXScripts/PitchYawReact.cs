using UnityEngine;

public class PitchYawReact : MonoBehaviour
{
    private Transform _cameraTransform;
    public float _lerpSpeed = 10f, _clampRotation = 30f; // smoothness, clamp

    public float _multiplier = 1f; //enhance pitchYaw movement

    void Start()
    {
        _cameraTransform = transform;
    }

    void Update()
    {
        // Get the pitchYaw values for x, y
        float _pitch = SpaceshipControls._pitchYaw.y;
        float _yaw = SpaceshipControls._pitchYaw.x;

        // Clamp pitch and yaw 
        _pitch = Mathf.Clamp(_pitch, -_clampRotation, _clampRotation);
        _yaw = Mathf.Clamp(_yaw, -_clampRotation, _clampRotation);

        // Calculate rotation based on x, y
        Vector3 _cameraRotation = new Vector3(_pitch * _multiplier, 0f, -_yaw * _multiplier);
        Quaternion _targetRotation = Quaternion.Euler(_cameraRotation);

        // Lerp camera to new location
        _cameraTransform.localRotation = Quaternion.Lerp(_cameraTransform.localRotation, _targetRotation, Time.deltaTime * _lerpSpeed);
    }
}
