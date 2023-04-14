using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatController : MonoBehaviour
{
    //Static instance so can be referenced in other scripts
    public static BeatController instance;

    [Header("REQUIRED")]
    //Song beats per minute
    public float _tempo;

    //beats per each loop (4/4 - top numnber of time signature)
    public float _beatsInLoop = 4;

    //Start of song Offset
    public float _startOfSongOffset;

    //Song
    public AudioSource _audioSource;

    //tap accuracy -perfect, good, poor etc
    public float _perfectTapThereshold, _goodTapThreshold, _poorTapThreshold;

    [Header("FOR REFERENCE")]

    //AV sync offset (plus / minus range)
    public float _avSyncOffset;
    //dspTime at start of song
    public float _dspTimeAtStart;

    //beat length in seconds; playhead position in seconds
    public float _secondsPerBeat, _playheadInSeconds;

    //playhead in beats; playhead in the loop
    public float _playheadInBeats, _loopPlayheadInBeats, _loopPlayheadInSeconds;

    //loop counter
    public int _loopCount = 0;

    //The current relative position of the song within the loop measured between 0 and 1.
    //Used for normalised 0 to 1 movement / lerp in other scripts
    public float _loopPlayheadNormalised;

    //check for AV sync
    public bool _stopOnBeatSyncCheck = false;

    private bool _init = true;

    void Awake()
    {
        //Create static instance of BeatController;
        instance = this;

        //Calculate seconds for each beat
        _secondsPerBeat = 60f / _tempo;

    }

    void FixedUpdate()
    {
        if (_init)
        {
            _init = false;

            //Start the music
            _audioSource.Play();

            //Log the time when the music starts
            _dspTimeAtStart = (float)AudioSettings.dspTime;
        }
        // get AV sync offset from menu scene
        _avSyncOffset = GameManagerDDOL._timingSliderValue;

        //playhead seconds from first beat (after start offset + AV offset)
        _playheadInSeconds = (float)(AudioSettings.dspTime - _dspTimeAtStart - _startOfSongOffset + _avSyncOffset);

        //playhead beats from first beat (after start offset)
        _playheadInBeats = _playheadInSeconds / _secondsPerBeat;

        //count numbner of loops
        if (_playheadInBeats >= (_loopCount + 1) * _beatsInLoop)
            _loopCount++;

        //playhead beats within current loop
        _loopPlayheadInBeats = _playheadInBeats - _loopCount * _beatsInLoop;

        //offset may make loop value negative, so keep adding bars until cycled through to positive number
        while (_loopPlayheadInBeats < 0)
        {
            _loopPlayheadInBeats += _beatsInLoop;
        }

        //Current playhead position in seconds within the loop (for Tap scripts)
        _loopPlayheadInSeconds = _loopPlayheadInBeats * _secondsPerBeat;

        //Normalised playhead beats within current loop (for other scripts)
        _loopPlayheadNormalised = _loopPlayheadInBeats / _beatsInLoop;

        if (!_audioSource.isPlaying)
        {
            // Debug.Log("Audio has stopped playing!");
            //Start the music
            _audioSource.Play();

            //Log the time when the music starts
            _dspTimeAtStart = (float)AudioSettings.dspTime;
        }
    }
}
