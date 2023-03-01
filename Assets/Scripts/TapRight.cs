using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.VectorGraphics;
using System;

public class TapRight : MonoBehaviour
{
    private SVGImage svgImage;

    private float perfectThreshold, goodThreshold, poorThreshold;
    [Header("REQUIRED")]
    [SerializeField] private Color[] colors;

    public List<float> rightBeats; //beats entered in inspector

    public static List<float> rightBeatsStaticVar;
    private bool[] beatsProcessed; //track beats processed to avoid double tap on beat

    public float startOffsetUnit; //0 to 1

    private float loopPlayheadInSeconds;
    private float barInSeconds;

    private bool resetLoop = false;

    void Awake()
    {
        //Shared leftBeats to other script (to draw indicators based on beats
        rightBeatsStaticVar = rightBeats;
    }

    void Start()
    {
        svgImage = GetComponent<SVGImage>();

        perfectThreshold = Controller.instance.perfectTapThereshold;
        goodThreshold = Controller.instance.goodTapThreshold;
        poorThreshold = Controller.instance.poorTapThreshold;

        // Initialize beatsProcessed array with the same size as leftBeats array
        beatsProcessed = new bool[rightBeats.Count];

        //length in seconds of one bar of 'x' beats
        barInSeconds = Controller.instance.beatsInLoop * Controller.instance.secondsPerBeat;

    }

    void Update()
    {

        //stop on beat one AV sync check
        if (Controller.instance.stopOnBeatSyncCheck == true)
        {
            if (Controller.instance.loopPlayheadInSeconds > Controller.instance.secondsPerBeat * 2)
            {
                //Debug.Log("Sec Per beat: " + Controller.instance.secondsPerBeat + " Playhead: " + Controller.instance.loopPlayheadInSeconds);
                Time.timeScale = 0.0f; // Stop time
                AudioListener.pause = true;
            }
        }

        //reset beatsProcessed array ready for next loop
        loopPlayheadInSeconds = Controller.instance.loopPlayheadInSeconds;
        if (resetLoop && loopPlayheadInSeconds >= (barInSeconds - poorThreshold - 0.05f))
        {
            //Debug.Log("R Array clear at: " + loopPlayheadInSeconds + " / " + barInSeconds);
            Array.Clear(beatsProcessed, 0, beatsProcessed.Length);
            resetLoop = false;
        }
        if (!resetLoop && loopPlayheadInSeconds > poorThreshold && loopPlayheadInSeconds < poorThreshold + 0.05f) resetLoop = true;
    }
    public void AnyKeyPressed(InputAction.CallbackContext context)
    {
        //Debug.Log("Callback Triggered R: " + context.phase);

        if (context.performed)
        {
            //Debug.Log("Tap Time R: " + Controller.instance.loopPlayheadInSeconds);

            // Loop through leftBeats list
            for (int i = 0; i < rightBeats.Count; i++)
            {
                //beat being processed
                float beatNumber = rightBeats[i];
                float timeOfBeat = Mathf.Abs((beatNumber - 1) * Controller.instance.secondsPerBeat);
                float tapTime = Controller.instance.loopPlayheadInSeconds;
                float timeDiff = Mathf.Abs(timeOfBeat - tapTime);
                //if approaching complete loop set timeDiff to fraction of sec before beat 1.
                if ((barInSeconds - timeDiff) < poorThreshold)
                {
                    timeDiff = barInSeconds - timeDiff;
                    //Debug.Log("R Near end of loop. timeDiff: " + timeDiff);
                }
                if (!beatsProcessed[i])
                {
                    if (timeDiff <= perfectThreshold)
                    {
                        //Debug.Log("Perfect R: timeDiff: " + timeDiff);
                        SetColorAndReset(1);
                        beatsProcessed[i] = true; //Stop Multiple click on same beat
                    }
                    else
                    if (timeDiff <= goodThreshold)
                    {
                        //Debug.Log("Good R: timeDiff: " + timeDiff);
                        SetColorAndReset(2);
                        beatsProcessed[i] = true; //Stop Multiple click on same beat
                    }
                    else
                    if (timeDiff <= poorThreshold)
                    {
                        //Debug.Log("Poor R: timeDiff: " + timeDiff);
                        SetColorAndReset(3);
                        beatsProcessed[i] = true; //Stop Multiple click on same beat
                    }
                    else
                    {
                        //Debug.Log("R Not on beat: " + beatNumber);
                    }
                }
            }
        }
    }

    private void SetColorAndReset(int colorIndex)
    {
        //set colour of the circle if tap is perfect(1), good(2), poor(3)
        svgImage.color = colors[colorIndex];
        //reet colour after fraction of a second
        StartCoroutine(ResetColour());
    }
    IEnumerator ResetColour()
    {
        yield return new WaitForSeconds(0.2f);
        svgImage.color = colors[0];
    }

}
