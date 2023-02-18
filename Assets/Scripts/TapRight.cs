using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TapRight : MonoBehaviour
{
    private Renderer renderer;

    private float perfectThreshold, goodThreshold, poorThreshold;
    [Header("REQUIRED")]
    [SerializeField] private Color[] colors;
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
        //Debug.Log("LoopPosition Right: " + loopPosition);

        /*Will need to build beat list input system for this, however, this works as a test for tap on first beat of bar*/

        // Check if the object tapped at top (first beat of the bar)
        // must not have been tapped already
        // top of loop is 0.5
        if (!isTappedThisRotation && context.performed)
        {
            //check timing accuracy of tap
            if (loopPosition > (0.5f - perfectThreshold) && loopPosition < (0.5f + perfectThreshold)) //perfect
            {
                //Debug.Log("Object tapped R Perfect: " + loopPosition);
                SetColorAndReset(1);
            }
            else if (loopPosition > (0.5f - goodThreshold) && loopPosition < (0.5f + goodThreshold)) //good
            {
                //Debug.Log("Object tapped R Good: " + loopPosition);
                SetColorAndReset(2);
            }
            else if (loopPosition > (0.5f - poorThreshold) && loopPosition < (0.5 + poorThreshold)) //poor
            {
                //Debug.Log("Object tapped R Poor: " + loopPosition);
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
        StartCoroutine(Reset_isTapped());
    }
    IEnumerator ResetColour()
    {
        yield return new WaitForSeconds(0.3f);
        renderer.material.color = colors[0];
    }

    IEnumerator Reset_isTapped()
    {
        float loopPosition = Controller.instance.loopPlayheadNormalised;

        while (loopPosition >= 0.3f && loopPosition <= 0.7f)
        {
            yield return null;
            loopPosition = Controller.instance.loopPlayheadNormalised;
        }

        isTappedThisRotation = false;
        //Debug.Log("Allowing R Taps again");
    }
}
