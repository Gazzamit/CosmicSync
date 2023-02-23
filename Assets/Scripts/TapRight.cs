using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.VectorGraphics;

public class TapRight : MonoBehaviour
{
    private SVGImage svgImage;

    private float perfectThreshold, goodThreshold, poorThreshold;
    [Header("REQUIRED")]
    [SerializeField] private Color[] colors;
    private bool isTappedThisRotation = false;

    void Start()
    {
        svgImage = GetComponent<SVGImage>();

        perfectThreshold = Controller.instance.perfectTapThereshold;
        goodThreshold = Controller.instance.goodTapThreshold;
        poorThreshold = Controller.instance.poorTapThreshold;
    }

    public void Fire(InputAction.CallbackContext context)
    {
        //Debug.Log("Callback Triggered: " + context.phase);

        //get normalised value of playhead position within loop
        float loopPosition = Controller.instance.loopPlayheadNormalised;
        Debug.Log("LoopPosition Left: " + loopPosition);

        /*Will need to build beat list input system for this, however, this works as a test for tap on first beat of bar*/

        // Check if the object tapped at top (first beat of the bar)
        // must not have been tapped already
        // top of loop of 0
        if (!isTappedThisRotation && context.performed)
        {
            //check timing accuracy of tap
            if (loopPosition > (1 - perfectThreshold) || loopPosition < perfectThreshold) //perfect
            {
                //Debug.Log("Object tapped L Perfect: " + loopPosition);
                SetColorAndReset(1);
            }
            else if (loopPosition > (1 - goodThreshold) || loopPosition < goodThreshold) //good
            {
                //Debug.Log("Object tapped L Good: " + loopPosition);
                SetColorAndReset(2);
            }
            else if (loopPosition > (1 - poorThreshold) || loopPosition < poorThreshold) //poor
            {
                //Debug.Log("Object tapped L Poor: " + loopPosition);
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
        svgImage.color = colors[colorIndex];
        //diallow other taps this rotation until reset
        isTappedThisRotation = true;
        //reet colour after fraction of a second
        StartCoroutine(ResetColour());
        //check if loopPosition is outside tappable area and then reset it
        StartCoroutine(Reset_isTapped());
    }
    IEnumerator ResetColour()
    {
        yield return new WaitForSeconds(0.3f);
        svgImage.color = colors[0];
    }

    IEnumerator Reset_isTapped()
    {
        float loopPosition = Controller.instance.loopPlayheadNormalised;

        while (loopPosition <= 0.2f || loopPosition >= 0.8f)
        {
            yield return null;
            loopPosition = Controller.instance.loopPlayheadNormalised;
        }

        isTappedThisRotation = false;
        //Debug.Log("Allowing L sTaps again");
    }
}
