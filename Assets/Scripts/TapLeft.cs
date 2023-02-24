using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.VectorGraphics;

public class TapLeft : MonoBehaviour
{
    private SVGImage svgImage;

    private float perfectThreshold, goodThreshold, poorThreshold;
    [Header("REQUIRED")]
    [SerializeField] private Color[] colors;
    private bool isTappedThisRotation = false;

    public List<float> leftBeats;

    private bool isKeyPressed = false;

    void Start()
    {
        leftBeats.AddRange(new float[] { 1, 2, 3, 4});
        
        svgImage = GetComponent<SVGImage>();

        perfectThreshold = Controller.instance.perfectTapThereshold;
        goodThreshold = Controller.instance.goodTapThreshold;
        poorThreshold = Controller.instance.poorTapThreshold;
    }

    void Update()
    {
        
        //stop on beat one AV sync check
        if(Controller.instance.stopOnBeatSyncCheck == true)
        {
            if (Controller.instance.loopPlayheadInSeconds > Controller.instance.secondsPerBeat * 2)
            {
                Debug.Log("Sec Per beat: " + Controller.instance.secondsPerBeat + " Playhead: " + Controller.instance.loopPlayheadInSeconds);
                Time.timeScale = 0.0f; // Stop time
                AudioListener.pause = true;
            }
        }
    }
    public void AnyKeyPressed(InputAction.CallbackContext context)
    {
        //Debug.Log("Callback Triggered L: " + context.phase);


        if (context.performed)
        {
            //Debug.Log("Tap Time: " + Controller.instance.loopPlayheadInSeconds);
            foreach (float beatTime in leftBeats) 
            {
                float timeDiff = Mathf.Abs((beatTime - 1) * Controller.instance.secondsPerBeat - Controller.instance.loopPlayheadInSeconds);
                //float tolerance = Controller.instance.secondsPerBeat  * 0.05f;
                if (timeDiff <= perfectThreshold) 
                    {
                        Debug.Log("Perfect: timeDiff: " + timeDiff);
                    }
                else 
                if (timeDiff <= goodThreshold) 
                    {
                        Debug.Log("Good: timeDiff: " + timeDiff);
                    }
                else 
                if (timeDiff <= poorThreshold) 
                    {
                        Debug.Log("Poor: timeDiff: " + timeDiff);
                    }
                else 
                    {
                        //Debug.Log("beatTime: " + beatTime);
                    }

            }
        }
    }

    private void SetColorAndReset(int colorIndex)
    {
        //set colour of the circle if tap is perfect(1), good(2), poor(3)
        svgImage.color = colors[colorIndex];
        //diallow other taps this rotation until reset
        isTappedThisRotation = true;
        //reet colour after fraction of a second
        StartCoroutine(ResetColour());
    }
    IEnumerator ResetColour()
    {
        yield return new WaitForSeconds(0.3f);
        svgImage.color = colors[0];
    }

}
