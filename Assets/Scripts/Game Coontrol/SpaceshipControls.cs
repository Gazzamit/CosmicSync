using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class SpaceshipControls : MonoBehaviour
{
    public bool _DEV_AllowLaserFire = false;
    //add header to inspector
    [Header("REQUIRED SETTINGS")]

    //public Rigidbody _rocketRB;

    //show private floats in inspector to allow game play adjustments like how fast to Yaw / Pitch etc
    [SerializeField]
    private float _yawTorque = 250f, _pitchTorque = 1000f, _rollTorque = 250f, _thrust = 1000f, _upThrust = 250f, _strafeThrust = 250f, _maxSpaceshipMagnitude = 180f;

    //Show in ispector, limit range, glide controls
    [SerializeField, Range(.01f, .99f)]
    private float _thrustGlideReduction = .5f, _upDownGllideReduction = .5f, _leftRightGlideReduction = .5f;
    private float _glide, _verticalGlide, _horizontalGlide = 0f;

    public float _perfectTapMultiplier = 8f, _goodTapMultiplier = 4f, _poorTapMultiplier = 2f;
    private float _tapMultiplier = 1;

    public Slider _speedRight, _speedLeft;
    public float _laserPulseLength = 0.4f;
    [SerializeField] private GameObject _laserLeft, _laserRight;

    //spaceship
    Rigidbody _rbSpaceShip;
    public static Vector3 _localVelocity;
    public static Vector3 _Vector3Spaceship;
    public static float _magnitude = 0f;

    //Input Values for controller / keyboard
    private float _thrust1D, _upDown1D, _strafe1D, _roll1D;
    public static Vector2 _pitchYaw;

    //Rocket1
    private float _fire1;
    private float _readyToFire1 = 1f;

    //Laser
    private bool _readyToFireLaser = true;
    public static bool _laserFiringLeft = false, _laserFiringRight = false;
    public ParticleSystem[] _laserParticleSystems;

    //stop input triggers after every half beat
    private float _turnOffInOneHalfBeat;

    //limit spaceship movement to tapped beats    
    public static bool _allowMovement = false;

    void Awake()

    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _rbSpaceShip = GetComponent<Rigidbody>();
        //_rbSpaceShip.AddRelativeForce(Vector3.forward  * 1000);   
    }

    void Start()
    {
        //prevent control activation after 0.5 beats
        _turnOffInOneHalfBeat = BeatController.instance._secondsPerBeat / 2f;
        //Debug.Log("Start() turnOffInOneHalfBeat: " + _turnOffInOneHalfBeat);
    }

    // FixedUpdate so that its framerate independant
    void FixedUpdate()
    {
        Movement();
        //Fire1();
        FireLaser();

        _speedRight.value = _magnitude / 200f; //200 set as max speed
        _speedLeft.value = _magnitude / 200f; //200 set as max speed
    }

    /*
    void Fire1()
    {
        //reset fire when key / button released 
        if (_fire1 < 0.1f && _readyToFire1 == 0)
            _readyToFire1 = 1;
        //fire
        if (_fire1 > 0.1f && _readyToFire1 == 1)
            {
                    {
                        Rigidbody clonedRocket;
                        //clonedRocket = Instantiate(_rocketRB, transform.position, transform.rotation);
                        clonedRocket = Instantiate(_rocketRB, transform.position, transform.rotation * Quaternion.Euler(90, 0, 0));
                        clonedRocket.velocity = transform.TransformDirection((Vector3.forward) * 150f) + _rbSpaceShip.velocity;
                        
                        _readyToFire1 = 0;
                    }
            }
    }
    */

    void FireLaser()
    {
        //reset fire when key / button released 
        if (_fire1 < 0.1f && _readyToFireLaser == false)
            _readyToFireLaser = true;
        //fire
        if (_fire1 > 0.1f && _readyToFireLaser == true)
        {
            if (_DEV_AllowLaserFire || TapLeft._leftSliderValue >= 0.25f || TapRight._rightSliderValue >= 0.25f) //fire when either quater full
            {
                _laserFiringLeft = true; //for Tap scripts (reduce Laser value)
                _laserFiringRight = true; //for Tap scripts (reduce Laser value)
                _laserLeft.SetActive(true);
                _laserRight.SetActive(true);
                StartCoroutine(FireLaserParticleSystems());
                _readyToFireLaser = false;
                StartCoroutine(TurnOffLasers());
            }
            else
            {
                //can't fire
            }
        }
    }


    IEnumerator FireLaserParticleSystems()
    {
        foreach (ParticleSystem ps in _laserParticleSystems)
        {
            ps.Stop();//Stop in case it is still playing
            ps.Play();
        }
        yield return null;
    }


    void Movement()
    {
        //for distance calculations
        _Vector3Spaceship = gameObject.transform.position;
        //Debug.Log(_Vector3Spaceship);

        _localVelocity = transform.InverseTransformDirection(_rbSpaceShip.velocity);
        //Debug.Log("Local Velocity: " + _localVelocity.ToString());     

        if (_allowMovement && (_roll1D > 0.1f || _roll1D < -0.1f))
        {
            //roll (AD) Roll over axis * buttonpress (±1) * how fast * deltaTime)
            _rbSpaceShip.AddRelativeTorque(Vector3.back * _roll1D * _rollTorque * Time.deltaTime);
            // Debug.Log("Allow Roll: " + Time.deltaTime);
        }

        //pitch (UP/Down Mouse delta) (clamp to prevent exceed 1)
        _rbSpaceShip.AddRelativeTorque(Vector3.right * Mathf.Clamp(-_pitchYaw.y, -1f, 1f) * _pitchTorque * Time.deltaTime);
        // Yaw (LR) (clamp to prevent exceed 1)
        _rbSpaceShip.AddRelativeTorque(Vector3.up * Mathf.Clamp(_pitchYaw.x, -1f, 1f) * _yawTorque * Time.deltaTime);

        // THRUST - if pressing a thrust key or move controller slightly above minimum amount
        //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 100, Color.red);
        if (_allowMovement && (_thrust1D > 0.1f || _thrust1D < -0.1f))
        {
            float _currentThrust = _thrust;
            // Debug.Log("Allow Thrust");
            _rbSpaceShip.AddRelativeForce(Vector3.forward * _thrust1D * _currentThrust * Time.deltaTime);

            //Clamp Velocity Magnitude
            if (_rbSpaceShip.velocity.magnitude > _maxSpaceshipMagnitude)
                _rbSpaceShip.velocity = Vector3.ClampMagnitude(_rbSpaceShip.velocity, _maxSpaceshipMagnitude);

            //_glide = _thrust;
        }
        else //nothing pressed
        {
            //add negative velocity relative force to all three vertex over glide time
            //Debug.Log("Slow Down");
            _rbSpaceShip.AddRelativeForce(Vector3.forward * -_localVelocity.z * _thrustGlideReduction);
            _rbSpaceShip.AddRelativeForce(Vector3.right * -_localVelocity.x * _thrustGlideReduction);
            _rbSpaceShip.AddRelativeForce(Vector3.up * -_localVelocity.y * _thrustGlideReduction);
            //reduce glide to reduce force per frame
            //_glide *= _thrustGlideReduction;   

            //untested alternative way to lerp velocity to zero
            //rigidbody.velocity = Vector3.Lerp(rigidbody.velocity, Vector3.zero, Time.fixedTimeDelta);
        }

        //Getvelocity after velocity controlling if statements above
        _magnitude = Mathf.Round(_localVelocity.magnitude);

        // UP / DOWN - if pressing a up/down key or move controller slightly above minimum amount
        if (_allowMovement && (_upDown1D > 0.1f || _upDown1D < -0.1f))
        {
            // Debug.Log("Allow up/down thrust");
            _rbSpaceShip.AddRelativeForce(Vector3.up * _upDown1D * _upThrust * Time.fixedDeltaTime);
            _verticalGlide = _upDown1D * _upThrust;
        }
        else //nothing pressed
        {
            _rbSpaceShip.AddRelativeForce(Vector3.up * _verticalGlide * Time.fixedDeltaTime);
            //reduce glide to reduce force per frame
            _verticalGlide *= _upDownGllideReduction;
        }


        // STRAFE - (QE) if pressing a strafe key or move controller slightly above minimum amount
        if (_allowMovement && (_strafe1D > 0.1f || _strafe1D < -0.1f))
        {
            // Debug.Log("Allow Strafe");
            _rbSpaceShip.AddRelativeForce(Vector3.right * _strafe1D * _strafeThrust * Time.fixedDeltaTime);
            _horizontalGlide = _strafe1D * _strafeThrust;
        }
        else //nothing pressed
        {
            _rbSpaceShip.AddRelativeForce(Vector3.right * _horizontalGlide * Time.fixedDeltaTime);
            //reduce glide to reduce force per frame
            _horizontalGlide *= _leftRightGlideReduction;
        }
    }

    private void setTapMultiuplier()
    {
        if (TapLeft._isPerfectHit || TapRight._isPerfectHit)
        {
            _tapMultiplier = _perfectTapMultiplier; //perfect
            _allowMovement = true;
            // Debug.Log("SC - Perfect Multiplier");
        }
        else if (TapLeft._isGoodHit || TapRight._isGoodHit)
        {
            _tapMultiplier = _goodTapMultiplier; //good
            _allowMovement = true;
            // Debug.Log("SC - Good Multiplier");
        }
        else if (TapLeft._isPoorHit || TapRight._isPoorHit)
        {
            _tapMultiplier = _poorTapMultiplier; //poor
            _allowMovement = true;
            // Debug.Log("SC - Poor Multiplier");
        }
        TapLeft._isPerfectHit = false;
        TapLeft._isGoodHit = false;
        TapLeft._isPoorHit = false;
        TapLeft._isMissHit = false;
        TapRight._isPerfectHit = false;
        TapRight._isGoodHit = false;
        TapRight._isPoorHit = false;
        TapRight._isMissHit = false;
    }

    IEnumerator TurnOffLasers()
    {
        yield return new WaitForSeconds(_laserPulseLength);
        _laserLeft.SetActive(false);
        _laserRight.SetActive(false);
        _readyToFireLaser = false;
    }

    IEnumerator TurnOffMovement()
    {
        yield return new WaitForSeconds(_turnOffInOneHalfBeat);
        _thrust1D = 0;
        _strafe1D = 0;
        _upDown1D = 0;
        _roll1D = 0;
        _allowMovement = false;
        // Debug.Log("MOVEMENT END");
    }

    //pass through values from buttons / controller
    #region Input Methods
    public void onThrust(InputAction.CallbackContext _context)
    {
        setTapMultiuplier(); //perfect/good/poor
        _thrust1D = _context.ReadValue<float>() * _tapMultiplier;
        StartCoroutine(TurnOffMovement());
        //Debug.Log("thrust");

    }
    public void onStrafe(InputAction.CallbackContext _context)
    {
        setTapMultiuplier(); //perfect/good/poor
        _strafe1D = _context.ReadValue<float>() * _tapMultiplier;
        StartCoroutine(TurnOffMovement());
        //Debug.Log("strafe");
    }
    public void onUpDown(InputAction.CallbackContext _context)
    {

        setTapMultiuplier(); //perfect/good/poor
        _upDown1D = _context.ReadValue<float>() * _tapMultiplier;
        StartCoroutine(TurnOffMovement());
        //Debug.Log("upDown");
    }
    public void onRoll(InputAction.CallbackContext _context)
    {
        setTapMultiuplier(); //perfect/good/poor
        _roll1D = _context.ReadValue<float>() * _tapMultiplier;
        StartCoroutine(TurnOffMovement());
        //Debug.Log("roll");
    }
    public void onPitchYaw(InputAction.CallbackContext _context)
    {
        _pitchYaw = _context.ReadValue<Vector2>();
        //Debug.Log("pitchYaw: " + _pitchYaw);
    }
    public void onFire1(InputAction.CallbackContext _context)
    {
        _fire1 = _context.ReadValue<float>();
        // Debug.Log("Fire1");
    }

    public void onCancel(InputAction.CallbackContext _context)
    {
        //_cancel = _context.ReadValue<float>();
        if (_context.performed)
        {
            // Debug.Log("Cancel - _switchInputMaps = true");
            InputMapSwitch._switchInputMaps = true; //switch to UI / SpaceshipControls
        }
    }

    //anykeypressed callbacks are in target scripts.
    #endregion 
}