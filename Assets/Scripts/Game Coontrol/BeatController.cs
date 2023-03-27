using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatController : MonoBehaviour
{

    [Header("REQUIRED")]
    //Song beats per minute
    public float _tempo;

    //beats per each loop (4/4 - top numnber of time signature)
    public float _beatsInLoop = 4;

    //Start of son Offset
    public float _startOfSongOffset;

    //Song
    public AudioSource _audioSource;

    //tap accuracy -perfect, good, poor etc
    public float _perfectTapThereshold, _goodTapThreshold, _poorTapThreshold;

    [Header("FOR REFERENCE")]
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

    //Conductor static instance so can be referenced in other scripts
    public static BeatController instance;

    void Awake()
    {
        //Create static instance of BeatController;
        instance = this;
    }


    void Start()
    {
        //Calculate seconds for each beat
        _secondsPerBeat = 60f / _tempo;

        //Start the music
        _audioSource.Play();

        //Log the time when the music starts
        _dspTimeAtStart = (float)AudioSettings.dspTime;
    }

    void FixedUpdate()
    {
        //playhead seconds from first beat (after start offset)
        _playheadInSeconds = (float)(AudioSettings.dspTime - _dspTimeAtStart - _startOfSongOffset);

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
    }
}