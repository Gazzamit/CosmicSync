using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{

    [Header("REQUIRED")]
    //Song beats per minute
    public float tempo;

    //beats per each loop (4/4 - top numnber of time signature)
    public float beatsInLoop = 4;
    
    //Start of son Offset
    public float startOffset;

    //Song
    public AudioSource audioSource;

    //tap accuracy -perfect, good, poor etc
    public float perfectTapThereshold, goodTapThreshold, poorTapThreshold;

    [Header("FOR REFERENCE")]
    //dspTime at start of song
    public float dspTimeAtStart;
    
    //beat length in seconds; playhead position in seconds
    public float secondsPerBeat, playheadInSeconds;

    //playhead in beats; playhead in the loop
    public float playheadInBeats, loopPlayheadInBeats;

    //loop counter
    public int loopCount = 0;

    //The current relative position of the song within the loop measured between 0 and 1.
    //Used for normalised 0 to 1 movement / lerp in other scripts
    public float loopPlayheadNormalised;

    //Conductor static instance so can be referenced in other scripts
    public static Controller instance;

    void Awake()
    {
        //Create static instance of Controller;
        instance = this;
    }


    void Start()
    {
        //Calculate seconds for each beat
        secondsPerBeat = 60f / tempo;

        //Start the music
        audioSource.Play();

        //Log the time when the music starts
        dspTimeAtStart = (float)AudioSettings.dspTime;
    }

    void Update()
    {
        //playhead seconds from first beat (after start offset)
        playheadInSeconds = (float)(AudioSettings.dspTime - dspTimeAtStart - startOffset);

        //playhead beats from first beat (after start offset)
        playheadInBeats = playheadInSeconds / secondsPerBeat;

        //count numbner of loops
        if (playheadInBeats >= (loopCount + 1) * beatsInLoop)
            loopCount++;

        //playhead beats within current loop
        loopPlayheadInBeats = playheadInBeats - loopCount * beatsInLoop;

        //Normalised playhead beats within current loop (for other scripts)
        loopPlayheadNormalised = loopPlayheadInBeats / beatsInLoop;
    }
}
