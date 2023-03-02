using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class SpaceshipControls : MonoBehaviour
{
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

    public float _tapMultiplier = 1, _perfectTapMultiplier = 1f, _goodTapMultiplier = 0.6f, _poorTapMultiplier = 0.2f;

    [SerializeField] private GameObject _laserLeft, _laserRight;

    //spaceship
    Rigidbody _rbSpaceShip;
    Vector3 _localVelocity;
    public static Vector3 _Vector3Spaceship;
    public static float _magnitude;

    //Input Values for controller / keyboard
    private float _thrust1D, _upDown1D, _strafe1D, _roll1D;
    private Vector2 _pitchYaw;

    //Rocket1
    private float _fire1;
    private float _readyToFire1 = 1f;
    private float _readyToFireLaser = 1f;
    private float _turnOffInOneHalfBeat;

    private bool _allowMovement = false;

    void Awake()

    {
        //Cursor.lockState = CursorLockMode.Locked;

        _rbSpaceShip = GetComponent<Rigidbody>();
        //_rbSpaceShip.AddRelativeForce(Vector3.forward  * 1000);   
    }

    void Start()
    {
        _turnOffInOneHalfBeat = Controller.instance._secondsPerBeat / 2f;
        //Debug.Log("turnOffInOneHalfBeat: " + turnOffInOneHalfBeat);
    }

    // FixedUpdate so that its framerate independant
    void FixedUpdate()
    {
        Movement();
        //Fire1();
        FireLaser();
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
        if (_fire1 < 0.1f && _readyToFireLaser == 0)
            _readyToFireLaser = 1;
        //fire
        if (_fire1 > 0.1f && _readyToFireLaser == 1)
        {
            _laserLeft.SetActive(true);
            _laserRight.SetActive(true);
            _readyToFireLaser = 0;
            StartCoroutine(TurnOffLasers());
        }
    }

    void Movement()
    {
        //for distance calculations
        _Vector3Spaceship = gameObject.transform.position;
        //Debug.Log(_Vector3Spaceship);

        _localVelocity = transform.InverseTransformDirection(_rbSpaceShip.velocity);
        //Debug.Log("Local Velocity: " + _localVelocity.ToString());     

        if (_allowMovement)
        {
            //roll (QE) Roll over axis * buttonpress (±1) * how fast * deltaTime)
            _rbSpaceShip.AddRelativeTorque(Vector3.back * _roll1D * _rollTorque * Time.deltaTime);
        }

        //pitch (UP/Down Mouse delta) (clamp to prevent exceed 1)
        _rbSpaceShip.AddRelativeTorque(Vector3.right * Mathf.Clamp(-_pitchYaw.y, -1f, 1f) * _pitchTorque * Time.deltaTime);
        // Yaw (LR) (clamp to prevent exceed 1)
        _rbSpaceShip.AddRelativeTorque(Vector3.up * Mathf.Clamp(_pitchYaw.x, -1f, 1f) * _yawTorque * Time.deltaTime);

        // THRUST - if pressing a thrust key or move controller slightly above minimum amount
        //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 100, Color.red);
        if (_allowMovement && _thrust1D > 0.1f || _thrust1D < -0.1f)
        {
            float _currentThrust = _thrust;
            _rbSpaceShip.AddRelativeForce(Vector3.forward * _thrust1D * _currentThrust * Time.deltaTime);

            //Clamp Velocity Magnitude
            if (_rbSpaceShip.velocity.magnitude > _maxSpaceshipMagnitude)
                _rbSpaceShip.velocity = Vector3.ClampMagnitude(_rbSpaceShip.velocity, _maxSpaceshipMagnitude);

            //_glide = _thrust;
        }
        else //nothing pressed
        {
            //add negative velocity relative force to all three vertex over glide time
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
        if (_allowMovement && _upDown1D > 0.1f || _upDown1D < -0.1f)
        {

            _rbSpaceShip.AddRelativeForce(Vector3.up * _upDown1D * _upThrust * Time.fixedDeltaTime);
            _verticalGlide = _upDown1D * _upThrust;
        }
        else //nothing pressed
        {
            _rbSpaceShip.AddRelativeForce(Vector3.up * _verticalGlide * Time.fixedDeltaTime);
            //reduce glide to reduce force per frame
            _verticalGlide *= _upDownGllideReduction;
        }


        // STRAFE - if pressing a strafe key or move controller slightly above minimum amount
        if (_allowMovement && _strafe1D > 0.1f || _strafe1D < -0.1f)
        {
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
        //Debug.Log("Fire1");
    }
    #endregion 

    private void setTapMultiuplier()
    {
        _allowMovement = true;
        if (TapLeft._isPerfectHit || TapRight._isPerfectHit)
        {
            _tapMultiplier = _perfectTapMultiplier; //perfect
            //Debug.Log("SC - Perfect Multiplier");
        }
        else if (TapLeft._isGoodHit || TapRight._isGoodHit)
        {
            _tapMultiplier = _goodTapMultiplier; //good
            //Debug.Log("SC - Good Multiplier");
        }
        else if (TapLeft._isPoorHit || TapRight._isPoorHit)
        {
            _tapMultiplier = _poorTapMultiplier; //poor
            //Debug.Log("SC - Poor Multiplier");
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
        yield return new WaitForSeconds(.8f);
        _laserLeft.SetActive(false);
        _laserRight.SetActive(false);
        _readyToFireLaser = 0;
    }

    IEnumerator TurnOffMovement()
    {
        yield return new WaitForSeconds(_turnOffInOneHalfBeat);
        _thrust1D = 0;
        _strafe1D = 0;
        _upDown1D = 0;
        _roll1D = 0;
        _allowMovement = false;
    }
}
