using UnityEngine;

public class PitchYawReact : MonoBehaviour
{
    // Thrust spaceship control
    public Transform _joystick;
    public float lerpSpeed = 10f; // smoothness

    public float multiplier = 1f; //enhance pitchYaw movement

    void Start()
    {
        _joystick = transform;
    }

void Update()
{
    // Get the pitchYaw values for x, y
    float _pitch = SpaceshipControls._pitchYaw.y;
    float _yaw = SpaceshipControls._pitchYaw.x;

    // Calculate rotation based on x, y
    Vector3 joystickRotation = new Vector3(_pitch * multiplier, 0f, -_yaw * multiplier);
    Quaternion targetRotation = Quaternion.Euler(joystickRotation);

    // Lerp joystick
    _joystick.localRotation = Quaternion.Lerp(_joystick.localRotation, targetRotation, Time.deltaTime * lerpSpeed);
}
}
