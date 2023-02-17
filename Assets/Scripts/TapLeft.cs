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

        /*Will need to build beat list input system for this, however, this works as a test for tap on first beat of bar*/

        // Check if the object tapped at top (first beat of the bar)
        // must not have been tapped already and loopPosition > zero (greateer than beat 1)
        if (!isTappedThisRotation && context.performed && loopPosition > 0)
        {
            //check timing accuracy of tap
            if (loopPosition > (1 - perfectThreshold) || loopPosition < perfectThreshold) //perfect
            {
                Debug.Log("Object tapped Perfect: " + loopPosition);
                SetColorAndReset(1);
            }
            else if (loopPosition > (1 - goodThreshold) || loopPosition < goodThreshold) //good
            {
                Debug.Log("Object tapped Good: " + loopPosition);
                SetColorAndReset(2);
            }
            else if (loopPosition > (1 - poorThreshold) || loopPosition < poorThreshold) //poor
            {
                Debug.Log("Object tapped Poor: " + loopPosition);
                SetColorAndReset(3);
            }
            else //miss
            {
                //do miss action
            }
        }
    }

    private void SetColorAndReset(int colorIndex)
    {
        //set colour of the circle if tap is perfect(1), good(2), poor(3)
        renderer.material.color = colors[colorIndex];
        //diallow other taps this rotation until reset
        isTappedThisRotation = true;
        //reet colour after fraction of a second
        StartCoroutine(ResetColour());
        //check if loopPosition is outside tappable area and then reset it
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
