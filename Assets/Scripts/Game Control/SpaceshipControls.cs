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
    private float _yawTorque = 250f,
        _pitchTorque = 1000f,
        _rollTorque = 250f,
        _thrust = 1000f,
        _upThrust = 250f,
        _strafeThrust = 250f,
        _maxSpaceshipMagnitude = 180f;

    //Show in ispector, limit range, glide controls
    [SerializeField, Range(.01f, .99f)]
    private float _thrustGlideReduction = .5f,
        _upDownGllideReduction = .5f,
        _leftRightGlideReduction = .5f;
    private float _glide,
        _verticalGlide,
        _horizontalGlide = 0f;

    public float _perfectTapMultiplier = 8f,
        _goodTapMultiplier = 4f,
        _poorTapMultiplier = 2f;
    private float _tapMultiplier = 1;

    [SerializeField]
    private AudioManager _audioManager;

    public Slider _speedRight,
        _speedLeft;
    public float _laserPulseLength = 0.4f;

    [SerializeField]
    private GameObject _laserLeft,
        _laserRight;

    //applied above scene index > 0
    public float _randomPortalTurbulance = 10f;
    public GameObject _whiteFlashParticlePrefab; // transvers potral effect

    //spaceship
    Rigidbody _rbSpaceShip;
    public static Vector3 _localVelocity;
    public static Vector3 _Vector3Spaceship;
    public static float _magnitude = 0f;

    //Input Values for controller / keyboard
    private float _thrust1D,
        _upDown1D,
        _strafe1D,
        _roll1D;
    public static Vector2 _pitchYaw;

    private float _pitchYawMultiplier;

    //Rocket1
    private float _fire1;
    private float _readyToFire1 = 1f;

    //Laser
    private bool _readyToFireLaser = true;
    public static bool _laserFiringLeftReduceValue = false,
        _laserFiringRightReduceValue = false;

    public static bool _LaserFiringStartShake = false,
        _laserFiringAddBlur = false;
    public ParticleSystem[] _laserParticleSystems;

    public static bool _doWelcomeSpaceShipControls = false;

    //stop input triggers after every half beat
    private float _turnOffInOneHalfBeat;

    //limit spaceship movement to tapped beats
    public static bool _allowMovement = false;

    //add portal turbulance
    private float _addPortalTurbulanceMultiplier;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _rbSpaceShip = GetComponent<Rigidbody>();
        //_rbSpaceShip.AddRelativeForce(Vector3.forward  * 1000);

        //No multiplier turbulance on awake
        _addPortalTurbulanceMultiplier = 1;
    }

    void Start()
    {
        //prevent control activation after 0.5 beats
        _turnOffInOneHalfBeat = BeatController.instance._secondsPerBeat / 2f;
        //Debug.Log("Start() turnOffInOneHalfBeat: " + _turnOffInOneHalfBeat);

        //start engine audio on 0
        _audioManager.PlayEngineRumble(0f);
    }

    // FixedUpdate so that its framerate independant
    void FixedUpdate()
    {
        Movement();
        //Fire1();
        FireLaser();

        _speedRight.value = _magnitude / 200f; //200 set as max speed
        _speedLeft.value = _magnitude / 200f; //200 set as max speed

        float _volume = _magnitude / 100f;
        _audioManager.AdjustEngineRumbleVolume(_volume);

        if (_doWelcomeSpaceShipControls == true)
        {
            StartCoroutine(_DoWelcomeSpaceshipActions());
            _doWelcomeSpaceShipControls = false;
        }
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
        //reset fire when key / button released. 0.1 values to compensate to controller off centre
        if (_fire1 < 0.1f && _readyToFireLaser == false)
            _readyToFireLaser = true;
        //fire
        if (_fire1 > 0.1f && _readyToFireLaser == true)
        {
            if (
                _DEV_AllowLaserFire
                || TapLeft._leftSliderValue >= 0.25f
                || TapRight._rightSliderValue >= 0.25f
            ) //fire when either quater full
            {
                _laserFiringLeftReduceValue = true; //for Tap scripts (reduce Laser value)
                _laserFiringRightReduceValue = true; //for Tap scripts (reduce Laser value)
                _LaserFiringStartShake = true; //for camera shake
                _laserFiringAddBlur = true; //for camera blur

                _audioManager.PlayLaser();

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

    IEnumerator _DoWelcomeSpaceshipActions()
    {
        //move toward target for doWelcome
        _thrust1D = 0.5f;
        _pitchYaw.y = 0.01f;
        _pitchYaw.x = 0.01f;
        _allowMovement = true;
        yield return new WaitForSeconds(5.7f);
        _pitchYaw.y = -0.001f;
        _pitchYaw.x = -0.001f;
        _laserFiringLeftReduceValue = true; //for Tap scripts (reduce Laser value)
        _laserFiringRightReduceValue = true; //for Tap scripts (reduce Laser value)
        _LaserFiringStartShake = true; //for camera shake
        _laserFiringAddBlur = true; //for camera blur
        _laserLeft.SetActive(true);
        _laserRight.SetActive(true);
        StartCoroutine(FireLaserParticleSystems());
        _readyToFireLaser = false;
        StartCoroutine(TurnOffLasers());
        yield return new WaitForSeconds(3f);
        _audioManager.PlayWelcome();
        yield return new WaitForSeconds(4f);
        //Add back amounts
        _roll1D = 0.05f;
        _thrust1D = 3f;
        _pitchYaw.y = -0.01f;
        _pitchYaw.x = -0.01f;
        yield return new WaitForSeconds(10f);
        //disable thrust
        _allowMovement = false;
    }

    IEnumerator FireLaserParticleSystems()
    {
        foreach (ParticleSystem _ps in _laserParticleSystems)
        {
            _ps.Stop(); //Stop in case it is still playing
            _ps.Play();
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

        if (_allowMovement && (_roll1D > 0.05f || _roll1D < -0.05f))
        {
            //roll (AD) Roll over axis * buttonpress (±1) * how fast * deltaTime)
            _rbSpaceShip.AddRelativeTorque(Vector3.back * _roll1D * _rollTorque * Time.deltaTime);
            // Debug.Log("Allow Roll: " + Time.deltaTime);
        }

        //Pitch Yaw allowed to move all the time
        //pitch (UP/Down Mouse delta) (clamp to prevent exceed 1)
        _rbSpaceShip.AddRelativeTorque(
            Vector3.right * Mathf.Clamp(-_pitchYaw.y, -1f, 1f) * _pitchTorque * Time.deltaTime
        );
        // Yaw (LR) (clamp to prevent exceed 1)
        _rbSpaceShip.AddRelativeTorque(
            Vector3.up * Mathf.Clamp(_pitchYaw.x, -1f, 1f) * _yawTorque * Time.deltaTime
        );

        // THRUST - if pressing a thrust key or move controller slightly above minimum amount
        //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 100, Color.red);
        if (_allowMovement && (_thrust1D > 0.1f || _thrust1D < -0.1f))
        {
            float _currentThrust = _thrust;
            // Debug.Log("Allow Thrust");
            _rbSpaceShip.AddRelativeForce(
                Vector3.forward * _thrust1D * _currentThrust * Time.deltaTime
            );

            //Clamp Velocity Magnitude
            if (_rbSpaceShip.velocity.magnitude > _maxSpaceshipMagnitude)
                _rbSpaceShip.velocity = Vector3.ClampMagnitude(
                    _rbSpaceShip.velocity,
                    _maxSpaceshipMagnitude
                );

            //_glide = _thrust;
        }
        else //nothing pressed
        {
            //add negative velocity relative force to all three vertex over glide time
            //Debug.Log("Slow Down");
            _rbSpaceShip.AddRelativeForce(
                Vector3.forward * -_localVelocity.z * _thrustGlideReduction
            );
            _rbSpaceShip.AddRelativeForce(
                Vector3.right * -_localVelocity.x * _thrustGlideReduction
            );
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
            _rbSpaceShip.AddRelativeForce(
                Vector3.right * _strafe1D * _strafeThrust * Time.fixedDeltaTime
            );
            _horizontalGlide = _strafe1D * _strafeThrust;
        }
        else //nothing pressed
        {
            _rbSpaceShip.AddRelativeForce(Vector3.right * _horizontalGlide * Time.fixedDeltaTime);
            //reduce glide to reduce force per frame
            _horizontalGlide *= _leftRightGlideReduction;
        }

        //for scenes > 0
        if (OnCollidePortal._addPortalTurbulanceNow == true)
        //if( _thrust1D != 0) //swap out for testing
        {
            OnCollidePortal._addPortalTurbulanceNow = false;
            AddPortalTurbulance();
            StartCoroutine(WhiteflashEffect());
        }
    }

    void AddPortalTurbulance()
    {
        //add random roll turbulance to tranversing portal (plus/minus 0.3f to 0.3f plus random)
        _addPortalTurbulanceMultiplier =
            Mathf.Sign(Random.Range(-1, 1))
            * Random.Range(_randomPortalTurbulance, 0.2f + _randomPortalTurbulance);
        // Debug.Log("Random Turbulance: " + _addPortalTurbulanceMultiplier);
        _allowMovement = true;
        if (GameManagerDDOL._doWelcome == true)
        {
            _addPortalTurbulanceMultiplier = 0.06f;
            _pitchYaw.y = 0.01f;
        }
        _roll1D = _addPortalTurbulanceMultiplier;
        TurnOffMovement();
    }

    IEnumerator WhiteflashEffect()
    {
        // Debug.Log("White Flash Effect Called");
        GameObject _particleObject3 = Instantiate(_whiteFlashParticlePrefab, transform);
        _particleObject3.transform.localPosition = new Vector3(0, 0, 100);

        // Enable the particle effect component on the particle object
        ParticleSystem _particleSystem3 = _particleObject3.GetComponent<ParticleSystem>();
        if (_particleSystem3 != null)
        {
            _particleSystem3.Play();
        }
        yield return new WaitForSeconds(2f);
        Destroy(_particleSystem3.gameObject);
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
        //Apply any movement for a short time only
        yield return new WaitForSeconds(_turnOffInOneHalfBeat);
        _thrust1D = 0;
        _strafe1D = 0;
        _upDown1D = 0;
        _roll1D = 0;
        _allowMovement = false;
        // Debug.Log("MOVEMENT END");
    }

    //pass through values from buttons / controller
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
        //PitchYaw added to player controls in UI menu
        _pitchYaw = _context.ReadValue<Vector2>() * GameManagerDDOL._pitchYawSliderValue;
        //Debug.Log("pitchYaw: " + _pitchYaw);
    }

    public void onFire1(InputAction.CallbackContext _context)
    {
        _fire1 = _context.ReadValue<float>();
        // Debug.Log("Fire1");
    }

    public void onCancel(InputAction.CallbackContext _context)
    {
        //if in game
        if (GameManagerDDOL._currentMode == GameManagerDDOL.GameMode.Game)
        {
            if (_context.performed)
            {
                // Debug.Log("_switchInputMaps = true");
                //switching HUD is set to true in input map switch for HUD animations
                InputMapSwitch._switchInputMaps = true; //switch to UI Menu
            }
        }
    }

    //anykeypressed callbacks are in target scripts.
}
