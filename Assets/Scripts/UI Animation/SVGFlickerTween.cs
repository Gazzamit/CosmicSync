using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VectorGraphics;
using DG.Tweening;

public class SVGFlickerTween : MonoBehaviour
{

    void Start()
    {
        //First run fx
        StartFlashSequence();
    }

    void LateUpdate()
    {
        //run seq on HUD change to Menu
        if (HUDAnimations._switchingHUD == true) 
        {
            StartFlashSequence();
            // Debug.Log("HUD Switch Flicker Tween");
        }
    }

    private void StartFlashSequence()
    {
        SVGImage svgImage = gameObject.GetComponent<SVGImage>();

        // Create the fade sequence
        Sequence fadeSequence = DOTween.Sequence()
            .Append(svgImage.DOFade(1, 0.04f)) //to black
            .AppendInterval(Random.Range(0.01f, 0.04f))
            .Append(svgImage.DOFade(0, 0.04f)) //to black
            .SetLoops(4, LoopType.Yoyo);

        // Start the sequence
        fadeSequence.Play();
    }
}
