using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TapLeft : MonoBehaviour
{
    private Renderer renderer;

    private float perfectThreshold, goodThreshold, poorThreshold;
    [Header("REQUIRED")]
    public Color[] colors;
    private bool isTappedThisRotation = false;

    void Start()
    {
        renderer = GetComponent<Renderer>();

        perfectThreshold = Controller.instance.perfectTapThereshold;
        goodThreshold = Controller.instance.goodTapThreshold;
        poorThreshold = Controller.instance.poorTapThreshold;
    }

    public void Fire(InputAction.CallbackContext context)
    {
        //Debug.Log("Callback Triggered: " + context.phase);

        //get normalised value of playhead position within loop
        float loopPosition = Controller.instance.loopPlayheadNormalised;
        //Debug.Log("LoopPosition: " + loopPosition);
        ;
        /*Will need to build beat list input system for this, however, this works as a test for tap on first beat of bar*/

        // Check if the object tapped at top (first beat of the bar) 
        if (!isTappedThisRotation)
        {
            if (context.performed)
            {
                //check timing accuracy of tap
                if (loopPosition > (1 - perfectThreshold) || loopPosition < perfectThreshold && loopPosition > 0) //perfect
                {
                    Debug.Log("Object tapped Perfect: " + loopPosition);
                    SetColorAndReset(1);
                }
                else if (loopPosition > (1 - goodThreshold) || loopPosition < goodThreshold && loopPosition > 0) //good
                {
                    Debug.Log("Object tapped Good: " + loopPosition);
                    SetColorAndReset(2);
                }
                else if (loopPosition > (1 - poorThreshold) || loopPosition < poorThreshold && loopPosition > 0) //poor
                {
                    Debug.Log("Object tapped Poor: " + loopPosition);
                    SetColorAndReset(3);
                }
                else //miss
                {
                    //do miss action

                    //renderer.material.color = colors[0];
                    //StartCoroutine(ResetColour());
                }
            }
        }

    }

    private void SetColorAndReset(int colorIndex)
    {
        renderer.material.color = colors[colorIndex];
        isTappedThisRotation = true;
        StartCoroutine(ResetColour());
        StartCoroutine(ResetHasBeenTapped());
    }
    IEnumerator ResetColour()
    {
        yield return new WaitForSeconds(0.3f);
        renderer.material.color = colors[0];
    }

    IEnumerator ResetHasBeenTapped()
    {
        float loopPosition = Controller.instance.loopPlayheadNormalised;

        while (loopPosition <= 0.2f || loopPosition >= 0.8f)
        {
            yield return null;
            loopPosition = Controller.instance.loopPlayheadNormalised;
        }

        isTappedThisRotation = false;
        Debug.Log("Allowing Taps again");
    }
}
