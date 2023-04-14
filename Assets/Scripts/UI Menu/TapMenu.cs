using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.VectorGraphics;
using System;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class TapMenu : MonoBehaviour
{
    public List<float> _menuBeats; //beats entered in inspector
    private SVGImage _svgImageRing; //_svgImageRing for green in time flash method below

    public float _perfectThreshold, _poorThreshold;
    [Header("REQUIRED")]
    [SerializeField] private Color[] _colours; //normal, good, poor, miss

    public static List<float> _menuBeatsStaticVar; //share to indicator orbit script
    private bool[] _beatsProcessed; //track beats processed to avoid double tap on beat


    private float _loopPlayheadInSeconds;
    private float _barInSeconds;

    private bool _resetLoop = false;

    private bool _isPerfectHit = false;
    private bool _isGoodHit = false;
    private bool _isPoorHit = false;
    private bool _isMissHit = false;
    public ParticleSystem _WHitMenu;

    private Vector2 _navInput; // used to get Y

    void Awake()
    {
        //Shared menuBeats to other script (to draw indicators based on beats)
        _menuBeatsStaticVar = _menuBeats;
    }

    void Start()
    {
        _svgImageRing = GetComponent<SVGImage>();

        // Initialize beatsProcessed array with the same size as menuBeats array
        _beatsProcessed = new bool[_menuBeats.Count];

        //length in seconds of one bar of 'x' beats
        //this debiugged to zero so moved some Beatcontroller vars to awake
        _barInSeconds = BeatController.instance._beatsInLoop * BeatController.instance._secondsPerBeat;
    }

    void FixedUpdate()
    {
        //reset beatsProcessed array ready for next loop
        _loopPlayheadInSeconds = BeatController.instance._loopPlayheadInSeconds;
        if (_resetLoop && _loopPlayheadInSeconds >= (_barInSeconds - _poorThreshold - 0.05f))
        {
            //Debug.Log("L Array clear at: " + loopPlayheadInSeconds + " / " + barInSeconds);
            Array.Clear(_beatsProcessed, 0, _beatsProcessed.Length);
            _resetLoop = false;
        }
        if (!_resetLoop && _loopPlayheadInSeconds > _poorThreshold && _loopPlayheadInSeconds < _poorThreshold + 0.05f) _resetLoop = true;
    }

    public void WKeyPressed(InputAction.CallbackContext _context)
    {
        //Debug.Log("Callback Triggered W: " + context.phase);

        _navInput = _context.ReadValue<Vector2>();

        //if in Menu 
        //if (GameManagerDDOL._isGame == false)
        if (GameManagerDDOL._currentMode == GameManagerDDOL.GameMode.MainMenu)
        {
            //if W tapped
            if (_context.performed && _navInput.y == 1)
            {
                // Loop through menuBeats list
                for (int i = 0; i < _menuBeats.Count; i++)
                {
                    //beat being processed
                    float _beatNumber = _menuBeats[i];
                    float _timeOfBeat = Mathf.Abs((_beatNumber - 1) * BeatController.instance._secondsPerBeat);
                    float _tapTime = BeatController.instance._loopPlayheadInSeconds;
                    float _timeDiff = Mathf.Abs(_timeOfBeat - _tapTime);

                    // Debug.Log("TM - TimmDiff from beaat(" + i + "): " + _timeDiff);

                    //if approaching complete loop set timeDiff to fraction of sec before beat 1.
                    if ((_barInSeconds - _timeDiff) < _poorThreshold)
                    {
                        _timeDiff = _barInSeconds - _timeDiff;
                        // Debug.Log("TM - Bar in Seconds: " + _barInSeconds);
                        // Debug.Log("TM - Menu Near end of loop. timeDiff: " + _timeDiff);
                    }



                    if (!_beatsProcessed[i])
                    {
                        if (_timeDiff <= _perfectThreshold)
                        {
                            // Debug.Log("Perfect Hit on Beat: " + i);
                            SetColorAndReset(1);
                            _beatsProcessed[i] = true; //Stop Multiple click on same beat
                            _isPerfectHit = true;

                            //load new scence with index tied to beat number
                            StartCoroutine(LoadNewScene(i));
                        }
                        else
                        if (_timeDiff <= _poorThreshold)
                        {
                            //Debug.Log("Poor L: timeDiff: " + timeDiff);
                            SetColorAndReset(3);
                            _beatsProcessed[i] = true; //Stop Multiple click on same beat
                        }
                        else
                        {
                            //Debug.Log("L Not on beat: " + beatNumber);
                        }
                    }
                }
            }
        }
    }

    //setting Menu Call
    public void ESCpressed(InputAction.CallbackContext _context)
    {
        //if in Menu
        if (GameManagerDDOL._currentMode == GameManagerDDOL.GameMode.MainMenu)
        {
            //as settings/menu active, transform used to check which input esc to use
            if (_context.performed && gameObject.transform.position.y == 0)
            {
                Debug.Log("TM - ESC pressed from Menu");
                GameManagerDDOL._currentMode = GameManagerDDOL.GameMode.SettingsMenu;
                HUDAnimations._switchingHUD = true; //for HUD animations
            }
        }
    }
    private void SetColorAndReset(int _colourIndex)
    {
        //set colour of the circle if tap is perfect(1), good(2), poor(3)
        _svgImageRing.color = _colours[_colourIndex];
        // _svgImageCharge.color = _colours[_colourIndex];

        StartCoroutine(WHitEffectMenu(_colourIndex)); // start particle effect
        //reet colour after fraction of a second
        StartCoroutine(ResetColour());
    }

    IEnumerator ResetColour()
    {

        yield return new WaitForSeconds(0.2f);
        _svgImageRing.color = _colours[0];
        // _svgImageCharge.color = _colours[0];
    }

    IEnumerator LoadNewScene(int i)
    {
        //load scene based on beat

        yield return new WaitForSeconds(0.3f);
        if (_isPerfectHit == true)
        {
            _isPerfectHit = false; //load just one new scene
            GameManagerDDOL._currentMode = GameManagerDDOL.GameMode.Game;
            HUDAnimations._switchingHUD = true; //for HUD animations
            //int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            Debug.Log("TM - Load Scene: " + i);
            //Debug.Log("TM - Current Game Mode: " + GameManagerDDOL._currentMode );
            SceneManager.LoadScene(i, LoadSceneMode.Single);
        }
    }

    IEnumerator WHitEffectMenu(int _colourIndex)
    {
        //start particle effect on perfect/good/poor hits
        ParticleSystem.ColorOverLifetimeModule colorModule = _WHitMenu.colorOverLifetime;
        ParticleSystem.MinMaxGradient color = colorModule.color;
        color = new ParticleSystem.MinMaxGradient(_colours[_colourIndex]);
        colorModule.color = color;
        //Debug.Log("Colour L : " + _colourIndex);
        _WHitMenu.Stop();//Stop in case it is still playing
        _WHitMenu.Play();
        yield return null;
    }

}
