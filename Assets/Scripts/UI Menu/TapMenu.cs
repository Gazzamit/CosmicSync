using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.VectorGraphics;
using System;
using UnityEngine.UI;
using DG.Tweening;

public class TapMenu : MonoBehaviour
{
    public List<float> _menuBeats; //beats entered in inspector
    //public float _startOffsetUnit; //0 to 1

    private SVGImage _svgImageRing; //, _svgImageCharge;
    //private Image _laserSlider;

    private float _perfectThreshold, _goodThreshold, _poorThreshold;
    [Header("REQUIRED")]
    [SerializeField] private Color[] _colours; //normal, good, poor, miss

    // [SerializeField] private float _perfectLaserBoost = 0.06f, _goodLaserBoost = 0.04f, _poorLaserBoost = 0.02f;

    public static List<float> _menuBeatsStaticVar;
    private bool[] _beatsProcessed; //track beats processed to avoid double tap on beat

    //public Slider _menuSlider; //track power of laser charge
    //public static float _menuSliderValue = 0f; //shared to tap scripts

    private float _loopPlayheadInSeconds;
    private float _barInSeconds;

    private bool _resetLoop = false;

    public static bool _isPerfectHit = false;
    public static bool _isGoodHit = false;
    public static bool _isPoorHit = false;
    public static bool _isMissHit = false;
    public ParticleSystem _anyHitMenu;

    void Awake()
    {
        //Shared menuBeats to other script (to draw indicators based on beats)
        _menuBeatsStaticVar = _menuBeats;
    }

    void Start()
    {
        _svgImageRing = GetComponent<SVGImage>();
        //_svgImageCharge = transform.parent.GetChild(2).GetChild(0).gameObject.GetComponent<SVGImage>();
        //_laserSlider = transform.parent.GetChild(2).GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<Image>();
        
        _perfectThreshold = BeatController.instance._perfectTapThereshold;
        _goodThreshold = BeatController.instance._goodTapThreshold;
        _poorThreshold = BeatController.instance._poorTapThreshold;

        // Initialize beatsProcessed array with the same size as menuBeats array
        _beatsProcessed = new bool[_menuBeats.Count];

        //length in seconds of one bar of 'x' beats
        _barInSeconds = BeatController.instance._beatsInLoop * BeatController.instance._secondsPerBeat;
    }

    void Update()
    {
        //_menuSlider.value = _menuSliderValue; //update slider (from pefect, good, poor hits)


        //DEV SETTING  - stop on beat one AV sync check
        // if (BeatController.instance._stopOnBeatSyncCheck == true)
        // {
        //     if (BeatController.instance._loopPlayheadInSeconds > BeatController.instance._secondsPerBeat * 2)
        //     {
        //         //Debug.Log("Sec Per beat: " + BeatController.instance.secondsPerBeat + " Playhead: " + BeatController.instance.loopPlayheadInSeconds);
        //         Time.timeScale = 0.0f; // Stop time
        //         AudioListener.pause = true;
        //     }
        // }

        //reset beatsProcessed array ready for next loop
        _loopPlayheadInSeconds = BeatController.instance._loopPlayheadInSeconds;
        if (_resetLoop && _loopPlayheadInSeconds >= (_barInSeconds - _poorThreshold - 0.05f))
        {
            //Debug.Log("L Array clear at: " + loopPlayheadInSeconds + " / " + barInSeconds);
            Array.Clear(_beatsProcessed, 0, _beatsProcessed.Length);
            _resetLoop = false;
        }
        if (!_resetLoop && _loopPlayheadInSeconds > _poorThreshold && _loopPlayheadInSeconds < _poorThreshold + 0.05f) _resetLoop = true;

        // if (SpaceshipControls._laserFiringMenu == true)
        // {
        //     SpaceshipControls._laserFiringMenu = false;
        //     StartCoroutine(LaserFiring());
        //     StartCoroutine(LerpLaserSliders());
        // }
    }


    // IEnumerator LaserFiring()
    // {
    //     //set Laser Charge SVG to pink
    //     _svgImageCharge.color = _colours[4];
    //     // transition back to blue
    //     _svgImageCharge.DOColor(_colours[0], 0.2f).SetEase(Ease.InExpo);

    //     yield return null;
    // }

    // IEnumerator LerpLaserSliders()
    // {
    //     //while reducing value change laser slider colour
    //     //_laserSlider.color = _colours[4];

    //     // //lerp the laser values down over 0.2s laser firing time
    //     // float _menuSliderValueStart = _menuSliderValue;
        
    //     // float _targetValue = _menuSliderValue - 0.25f;

    //     // float t = 0f;
    //     // float _duration = 0.2f;

    //     // while (t < _duration)
    //     // {
    //     //     t += Time.deltaTime;
    //     //     float _newSliderValue = Mathf.Lerp(_menuSliderValueStart, _targetValue, t / _duration);
    //     //     _menuSliderValue = _newSliderValue;
    //     //     yield return null; //run other code
    //     // }

    //     //After laser fired reset colour of slider
    //     //_laserSlider.color = _colours[5];

    //     //Debug.Log("Target Val : Actual Value : " + _targetValue + " : " + _menuSliderValue);
    // }

    public void AnyKeyPressed(InputAction.CallbackContext _context)
    {
        //Debug.Log("Callback Triggered L: " + context.phase);

        if (_context.performed)
        {
            //_menuSliderValue = _menuSlider.value; //update laser slider value variable

            //Debug.Log("Tap Time L: " + BeatController.instance.loopPlayheadInSeconds);
            //Debug.Log("Menu Slider: " + _menuSlider.value);
            // Loop through menuBeats list
            for (int i = 0; i < _menuBeats.Count; i++)
            {
                //beat being processed
                float _beatNumber = _menuBeats[i];
                float _timeOfBeat = Mathf.Abs((_beatNumber - 1) * BeatController.instance._secondsPerBeat);
                float _tapTime = BeatController.instance._loopPlayheadInSeconds;
                float _timeDiff = Mathf.Abs(_timeOfBeat - _tapTime);
                //if approaching complete loop set timeDiff to fraction of sec before beat 1.
                if ((_barInSeconds - _timeDiff) < _poorThreshold)
                {
                    _timeDiff = _barInSeconds - _timeDiff;
                    //Debug.Log("R Near end of loop. timeDiff: " + timeDiff);
                }
                if (!_beatsProcessed[i])
                {
                    if (_timeDiff <= _perfectThreshold)
                    {
                        //Debug.Log("Perfect L: timeDiff: " + timeDiff);
                        SetColorAndReset(1);
                        _beatsProcessed[i] = true; //Stop Multiple click on same beat
                        _isPerfectHit = true; // for spaceship Controls
                        // _menuSliderValue += _perfectLaserBoost;
                    }
                    else
                    if (_timeDiff <= _goodThreshold)
                    {
                        //Debug.Log("Good L: timeDiff: " + timeDiff);
                        SetColorAndReset(2);
                        _beatsProcessed[i] = true; //Stop Multiple click on same beat
                        _isGoodHit = true; // for spaceship Controls
                        // _menuSliderValue += _goodLaserBoost;
                    }
                    else
                    if (_timeDiff <= _poorThreshold)
                    {
                        //Debug.Log("Poor L: timeDiff: " + timeDiff);
                        SetColorAndReset(3);
                        _beatsProcessed[i] = true; //Stop Multiple click on same beat
                        _isPoorHit = true; // for spaceship Controls
                        // _menuSliderValue += _poorLaserBoost;
                    }
                    else
                    {
                        //Debug.Log("L Not on beat: " + beatNumber);
                    }
                }
            }
        }
    }

    private void SetColorAndReset(int _colourIndex)
    {
        //set colour of the circle if tap is perfect(1), good(2), poor(3)
        _svgImageRing.color = _colours[_colourIndex];
        // _svgImageCharge.color = _colours[_colourIndex];

        StartCoroutine(AnyHitEffectMenu(_colourIndex)); // start particle effect
        //reet colour after fraction of a second
        StartCoroutine(ResetColour());
    }

    IEnumerator ResetColour()
    {

        yield return new WaitForSeconds(0.2f);
        _svgImageRing.color = _colours[0];
        // _svgImageCharge.color = _colours[0];

    }

    IEnumerator AnyHitEffectMenu(int _colourIndex)
    {
        //start particle effect on perfect/good/poor hits
        ParticleSystem.ColorOverLifetimeModule colorModule = _anyHitMenu.colorOverLifetime;
        ParticleSystem.MinMaxGradient color = colorModule.color;
        color = new ParticleSystem.MinMaxGradient(_colours[_colourIndex]);
        colorModule.color = color;
        //Debug.Log("Colour L : " + _colourIndex);
        _anyHitMenu.Stop();//Stop in case it is still playing
        _anyHitMenu.Play();
        yield return null;
    }

    public void _P_Pressed(InputAction.CallbackContext _context)
    {
        //submit acion key P pressed in UI Menu mode
        Debug.Log("P Pressed - works only in UI Menu mode");
    }

}