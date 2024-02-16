using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.VectorGraphics;
using System;
using UnityEngine.UI;
using DG.Tweening;
using System.IO;

public class TapLeft : MonoBehaviour
{
    //[SerializeField] private GameObject _audioManagerObj;
    [SerializeField]
    private AudioManager _audioManager;
    public List<float> _leftBeats; //beats entered in inspector

    //public float _startOffsetUnit; //0 to 1

    private SVGImage _svgImageRing;

    private float _perfectThreshold,
        _goodThreshold,
        _poorThreshold;

    [Header("REQUIRED")]
    [SerializeField]
    private Color[] _colours; //normal, good, poor, miss

    [SerializeField]
    private float _perfectLaserBoost = 0.06f,
        _goodLaserBoost = 0.04f,
        _poorLaserBoost = 0.02f;

    public static List<float> _leftBeatsStaticVar;
    private bool[] _beatsProcessed; //track beats processed to avoid double tap on beat

    public Slider _leftSlider; //track power of laser charge
    public static float _leftSliderValue = 0f; //shared to tap scripts

    private float _loopPlayheadInSeconds;
    private float _barInSeconds;

    private bool _resetLoop = false;

    public static bool _isPerfectHit = false;
    public static bool _isGoodHit = false;
    public static bool _isPoorHit = false;
    public static bool _isMissHit = false;

    private bool _hitCounts = false;
    public ParticleSystem _anyHitLeft;

    [SerializeField]
    private SVGImage _svgImageCharge;

    [SerializeField]
    private Image _laserSlider;

    // Struct to hold beat and tap times for the left side
    public struct LeftHitAccuracyData
    {
        public float DataTimeOfBeat;
        public float DataTapTime;
    }

    // Lists to store hit accuracy data
    private List<LeftHitAccuracyData> _listLeftPerfectHits = new List<LeftHitAccuracyData>();
    private List<LeftHitAccuracyData> _listLeftGoodHits = new List<LeftHitAccuracyData>();
    private List<LeftHitAccuracyData> _listLeftPoorHits = new List<LeftHitAccuracyData>();

    private bool _metricsSavedLeft = false; // bool flag to check saved once only from update

    void Awake()
    {
        //Shared leftBeats to other script (to draw indicators based on beats)
        _leftBeatsStaticVar = _leftBeats;
        _leftSliderValue = 0f;
    }

    void Start()
    {
        _svgImageRing = GetComponent<SVGImage>();

        _perfectThreshold = BeatController.instance._perfectTapThereshold;
        _goodThreshold = BeatController.instance._goodTapThreshold;
        _poorThreshold = BeatController.instance._poorTapThreshold;

        // Initialize beatsProcessed array with the same size as leftBeats array
        _beatsProcessed = new bool[_leftBeats.Count];

        //length in seconds of one bar of 'x' beats
        _barInSeconds =
            BeatController.instance._beatsInLoop * BeatController.instance._secondsPerBeat;

        //_audioManager = _audioManagerObj.GetComponent<AudioManager>(); //get reference to the script
    }

    void FixedUpdate()
    {
        //menu disable gameplay wrapper
        //if (GameManagerDDOL._isGame == true)
        if (GameManagerDDOL._currentMode == GameManagerDDOL.GameMode.Game)
        {
            _leftSlider.value = _leftSliderValue; //update slider (from pefect, good, poor hits)

            //stop on beat one AV sync check
            if (BeatController.instance._stopOnBeatSyncCheck == true)
            {
                if (
                    BeatController.instance._loopPlayheadInSeconds
                    > BeatController.instance._secondsPerBeat * 2
                )
                {
                    //Debug.Log("Sec Per beat: " + BeatController.instance.secondsPerBeat + " Playhead: " + BeatController.instance.loopPlayheadInSeconds);
                    Time.timeScale = 0.0f; // Stop time
                    AudioListener.pause = true;
                }
            }

            //reset beatsProcessed array ready for next loop
            _loopPlayheadInSeconds = BeatController.instance._loopPlayheadInSeconds;
            if (_resetLoop && _loopPlayheadInSeconds >= (_barInSeconds - _poorThreshold - 0.05f))
            {
                //Debug.Log("L Array clear at: " + loopPlayheadInSeconds + " / " + barInSeconds);
                Array.Clear(_beatsProcessed, 0, _beatsProcessed.Length);
                _resetLoop = false;
            }
            if (
                !_resetLoop
                && _loopPlayheadInSeconds > _poorThreshold
                && _loopPlayheadInSeconds < _poorThreshold + 0.05f
            )
                _resetLoop = true;

            if (SpaceshipControls._laserFiringLeftReduceValue == true)
            {
                SpaceshipControls._laserFiringLeftReduceValue = false;
                StartCoroutine(LaserFiring());
                StartCoroutine(LerpLaserSliders());
            }
        }

        if (ScoreManager._finalTargetDestroyed && !_metricsSavedLeft)
        {
            SaveMetricsToFiles();
            _metricsSavedLeft = true; // don't call SaveMetricsToFiles again
        }
    }

    IEnumerator LaserFiring()
    {
        //set Laser Charge SVG to pink
        _svgImageCharge.color = _colours[4];
        // transition back to blue
        _svgImageCharge.DOColor(_colours[0], 0.2f).SetEase(Ease.InExpo);

        yield return null;
    }

    IEnumerator LerpLaserSliders()
    {
        //while reducing value change laser slider colour
        _laserSlider.color = _colours[4];

        //lerp the laser values down over 0.2s laser firing time
        float _leftSliderValueStart = _leftSliderValue;

        float _targetValue = _leftSliderValue - 0.25f;

        float t = 0f;
        float _duration = 0.2f;

        while (t < _duration)
        {
            t += Time.deltaTime;
            float _newSliderValue = Mathf.Lerp(_leftSliderValueStart, _targetValue, t / _duration);
            _leftSliderValue = _newSliderValue;
            yield return null; //run other code
        }

        //After laser fired reset colour of slider
        _laserSlider.color = _colours[5];

        //Debug.Log("Target Val : Actual Value : " + _targetValue + " : " + _leftSliderValue);
    }

    public void AnyKeyPressed(InputAction.CallbackContext _context)
    {
        //Debug.Log("Callback Triggered L: " + context.phase);

        if (_context.performed)
        {
            _leftSliderValue = _leftSlider.value; //update laser slider value variable

            //Debug.Log("Tap Time L: " + BeatController.instance.loopPlayheadInSeconds);
            //Debug.Log("Left Slider: " + _leftSlider.value);
            // Loop through leftBeats list
            for (int i = 0; i < _leftBeats.Count; i++)
            {
                //beat being processed
                float _beatNumber = _leftBeats[i];
                float _timeOfBeat = Mathf.Abs(
                    (_beatNumber - 1) * BeatController.instance._secondsPerBeat
                );
                float _tapTime = BeatController.instance._loopPlayheadInSeconds;
                float _timeDiff = Mathf.Abs(_timeOfBeat - _tapTime);
                //if approaching complete loop set timeDiff to fraction of sec before beat 1.
                if ((_barInSeconds - _timeDiff) < _poorThreshold)
                {
                    _timeDiff = _barInSeconds - _timeDiff;
                    // Debug.Log("L Near end of loop. timeDiff: " + _timeDiff);
                }
                if (!_beatsProcessed[i])
                {
                    if (_timeDiff <= _perfectThreshold)
                    {
                        //Debug.Log("Perfect L: timeDiff: " + timeDiff);
                        SetColorAndReset(1);
                        _beatsProcessed[i] = true; //Stop Multiple click on same beat
                        _isPerfectHit = true; // for spaceship Controls
                        _leftSliderValue += _perfectLaserBoost;
                        ScoreManager._instance.AddPoints("perfect");
                        _hitCounts = true; //so not a miss hit
                        _audioManager.PlayThruster();
                        _listLeftPerfectHits.Add(
                            new LeftHitAccuracyData
                            {
                                DataTimeOfBeat = _timeOfBeat,
                                DataTapTime = _tapTime
                            }
                        );
                    }
                    else if (_timeDiff <= _goodThreshold)
                    {
                        //Debug.Log("Good L: timeDiff: " + timeDiff);
                        SetColorAndReset(2);
                        _beatsProcessed[i] = true; //Stop Multiple click on same beat
                        _isGoodHit = true; // for spaceship Controls
                        _leftSliderValue += _goodLaserBoost;
                        ScoreManager._instance.AddPoints("good");
                        _hitCounts = true; //so not a miss hit
                        _audioManager.PlayThruster();
                        _listLeftGoodHits.Add(
                            new LeftHitAccuracyData
                            {
                                DataTimeOfBeat = _timeOfBeat,
                                DataTapTime = _tapTime
                            }
                        );
                    }
                    else if (_timeDiff <= _poorThreshold)
                    {
                        //Debug.Log("Poor L: timeDiff: " + timeDiff);
                        SetColorAndReset(3);
                        _beatsProcessed[i] = true; //Stop Multiple click on same beat
                        _isPoorHit = true; // for spaceship Controls
                        _leftSliderValue += _poorLaserBoost;
                        ScoreManager._instance.AddPoints("poor");
                        _hitCounts = true; //so not a miss hit
                        _audioManager.PlayThruster();
                        _listLeftPoorHits.Add(
                            new LeftHitAccuracyData
                            {
                                DataTimeOfBeat = _timeOfBeat,
                                DataTapTime = _tapTime
                            }
                        );
                    }
                    else
                    {
                        //Debug.Log("L Not on beat: " + beatNumber);
                    }
                }
            }
            //if the hit was not inside even the poor threshold for any beat
            if (_hitCounts == false)
            {
                ScoreManager._instance.AddPoints("miss"); //minus value
            }
            if (_hitCounts == true)
                _hitCounts = false; //reset for next key press
        }
    }

    private void SetColorAndReset(int _colourIndex)
    {
        //set colour of the circle if tap is perfect(1), good(2), poor(3)
        _svgImageRing.color = _colours[_colourIndex];
        _svgImageCharge.color = _colours[_colourIndex];

        StartCoroutine(AnyHitEffectLeft(_colourIndex)); // start particle effect
        //reet colour after fraction of a second
        StartCoroutine(ResetColour());
    }

    IEnumerator ResetColour()
    {
        yield return new WaitForSeconds(0.2f);
        _svgImageRing.color = _colours[0];
        _svgImageCharge.color = _colours[0];
    }

    IEnumerator AnyHitEffectLeft(int _colourIndex)
    {
        //start particle effect on perfect.good.poor hits
        ParticleSystem.ColorOverLifetimeModule colorModule = _anyHitLeft.colorOverLifetime;
        ParticleSystem.MinMaxGradient color = colorModule.color;
        color = new ParticleSystem.MinMaxGradient(_colours[_colourIndex]);
        colorModule.color = color;
        //Debug.Log("Colour L : " + _colourIndex);
        _anyHitLeft.Stop(); //Sto in case it is still playing
        _anyHitLeft.Play();
        yield return null;
    }

    // Save the metrics to files. and save vars
    public void SaveMetricsToFiles()
    {
        SaveListToCSV(_listLeftPerfectHits, "CSPerfectHits_Left.csv");
        SaveListToCSV(_listLeftGoodHits, "CSGoodHits_Left.csv");
        SaveListToCSV(_listLeftPoorHits, "CSPoorHits_Left.csv");
        SaveVarsToCSV();
    }

    private void SaveListToCSV(List<LeftHitAccuracyData> list, string fileName)
    {
        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string baseFileName = Path.GetFileNameWithoutExtension(fileName);
        string extension = Path.GetExtension(fileName);
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        string directoryPath = Path.Combine(
            documentsPath,
            $"Documents/CSGameData/Data_Left_{timestamp}"
        );
        Debug.Log("Save Folder Left: " + directoryPath);

        // Check if the directory exists, if not, create it
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        string fileNameWithTimestamp = $"{baseFileName}_{timestamp}{extension}";
        string filePath = Path.Combine(directoryPath, fileNameWithTimestamp);

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine("TimeOfBeat,TapTime"); // CSV Header
            foreach (var hit in list)
            {
                writer.WriteLine($"{hit.DataTimeOfBeat},{hit.DataTapTime}");
            }
        }
    }

    private void SaveVarsToCSV()
    {
        // Save secondsPerBeat and beatsInLoop to a text file
        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string baseFileName = "CSDataVars";
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        string varsFileName = $"{baseFileName}_{timestamp}.csv";
        string directoryPath = Path.Combine(
            documentsPath,
            $"Documents/CSGameData/Data_Left_{timestamp}"
        );
        string varsFilePath = Path.Combine(directoryPath, varsFileName);
        using (StreamWriter writer = new StreamWriter(varsFilePath))
        {
            writer.WriteLine($"SecondsPerBeat, {BeatController.instance._secondsPerBeat}");
            writer.WriteLine($"BeatsInLoop, {BeatController.instance._beatsInLoop}");
            writer.WriteLine($"TimeStamp, {timestamp}");
            writer.WriteLine($"LeftOrRight, Left");
        }
    }
}
