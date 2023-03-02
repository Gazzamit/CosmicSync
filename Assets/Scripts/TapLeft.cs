using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.VectorGraphics;
using System;

public class TapLeft : MonoBehaviour
{
    private SVGImage _svgImage;

    private float _perfectThreshold, _goodThreshold, _poorThreshold;
    [Header("REQUIRED")]
    [SerializeField] private Color[] _colors;

    public List<float> _leftBeats; //beats entered in inspector

    public static List<float> _leftBeatsStaticVar;
    private bool[] _beatsProcessed; //track beats processed to avoid double tap on beat

    public float _startOffsetUnit; //0 to 1

    private float _loopPlayheadInSeconds;
    private float _barInSeconds;

    private bool _resetLoop = false;

    public static bool _isPerfectHit = false;
    public static bool _isGoodHit = false;
    public static bool _isPoorHit = false;
    public static bool _isMissHit = false;
    

    void Awake()
    {
        //Shared leftBeats to other script (to draw indicators based on beats
        _leftBeatsStaticVar = _leftBeats;
    }

    void Start()
    {
        _svgImage = GetComponent<SVGImage>();

        _perfectThreshold = Controller.instance._perfectTapThereshold;
        _goodThreshold = Controller.instance._goodTapThreshold;
        _poorThreshold = Controller.instance._poorTapThreshold;

        // Initialize beatsProcessed array with the same size as leftBeats array
        _beatsProcessed = new bool[_leftBeats.Count];

        //length in seconds of one bar of 'x' beats
        _barInSeconds = Controller.instance._beatsInLoop * Controller.instance._secondsPerBeat;
    }

    void Update()
    {

        //stop on beat one AV sync check
        if (Controller.instance._stopOnBeatSyncCheck == true)
        {
            if (Controller.instance._loopPlayheadInSeconds > Controller.instance._secondsPerBeat * 2)
            {
                //Debug.Log("Sec Per beat: " + Controller.instance.secondsPerBeat + " Playhead: " + Controller.instance.loopPlayheadInSeconds);
                Time.timeScale = 0.0f; // Stop time
                AudioListener.pause = true;
            }
        }

        //reset beatsProcessed array ready for next loop
        _loopPlayheadInSeconds = Controller.instance._loopPlayheadInSeconds;
        if (_resetLoop && _loopPlayheadInSeconds >= (_barInSeconds - _poorThreshold - 0.05f))
        {
            //Debug.Log("L Array clear at: " + loopPlayheadInSeconds + " / " + barInSeconds);
            Array.Clear(_beatsProcessed, 0, _beatsProcessed.Length);
            _resetLoop = false;
        }
        if (!_resetLoop && _loopPlayheadInSeconds > _poorThreshold && _loopPlayheadInSeconds < _poorThreshold + 0.05f) _resetLoop = true;
    }

    public void AnyKeyPressed(InputAction.CallbackContext _context)
    {
        //Debug.Log("Callback Triggered L: " + context.phase);

        if (_context.performed)
        {
            //Debug.Log("Tap Time L: " + Controller.instance.loopPlayheadInSeconds);

            // Loop through leftBeats list
            for (int i = 0; i < _leftBeats.Count; i++)
            {
                //beat being processed
                float _beatNumber = _leftBeats[i];
                float _timeOfBeat = Mathf.Abs((_beatNumber - 1) * Controller.instance._secondsPerBeat);
                float _tapTime = Controller.instance._loopPlayheadInSeconds;
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
                    }
                    else
                    if (_timeDiff <= _goodThreshold)
                    {
                        //Debug.Log("Good L: timeDiff: " + timeDiff);
                        SetColorAndReset(2);
                        _beatsProcessed[i] = true; //Stop Multiple click on same beat
                        _isGoodHit = true; // for spaceship Controls
                    }
                    else
                    if (_timeDiff <= _poorThreshold)
                    {
                        //Debug.Log("Poor L: timeDiff: " + timeDiff);
                        SetColorAndReset(3);
                        _beatsProcessed[i] = true; //Stop Multiple click on same beat
                        _isPoorHit = true; // for spaceship Controls
                    }
                    else
                    {
                        //Debug.Log("L Not on beat: " + beatNumber);
                    }
                }
            }
        }
    }

    private void SetColorAndReset(int _colorIndex)
    {
        //set colour of the circle if tap is perfect(1), good(2), poor(3)
        _svgImage.color = _colors[_colorIndex];
        //reet colour after fraction of a second
        StartCoroutine(ResetColour());
    }
    IEnumerator ResetColour()
    {
        yield return new WaitForSeconds(0.2f);
        _svgImage.color = _colors[0];
    }

}
