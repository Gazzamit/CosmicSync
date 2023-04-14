using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VectorGraphics;
using DG.Tweening;

public class SVGFlickerTween : MonoBehaviour
{

    private SVGImage _svgImage;
    //[SerializeField] SVGImage _svgImage;
    void Start()
    {
        _svgImage = gameObject.GetComponent<SVGImage>();
        //First run fx
        StartFlashSequence();
    }

    //function directly called by HUDAnimatinos via List of Flicker Objects
    public void StartFlashSequence()
    {
        //Debug.Log("Flicker Tween: " + gameObject.name);
        // Create the fade sequence
        Sequence fadeSequence = DOTween.Sequence()
            .Append(_svgImage.DOFade(1, 0.04f)) //to White
            .AppendInterval(Random.Range(0.01f, 0.04f))
            .Append(_svgImage.DOFade(0, 0.04f)) //to black
            .AppendInterval(Random.Range(0.01f, 0.04f))
            .Append(_svgImage.DOFade(1, 0.04f)) //to white
            .AppendInterval(Random.Range(0.01f, 0.04f))
            .Append(_svgImage.DOFade(0, 0.04f)) //to black
            .AppendInterval(Random.Range(0.01f, 0.04f))
            .Append(_svgImage.DOFade(1, 0.04f)) //to white
            .Append(_svgImage.DOFade(1, 0.04f)) //to white
            .AppendInterval(Random.Range(0.01f, 0.04f))
            .Append(_svgImage.DOFade(0, 0.04f)) //to black
            .AppendInterval(Random.Range(0.01f, 0.04f))
            .Append(_svgImage.DOFade(1, 0.04f)); //to white


        // Start the sequence
        fadeSequence.Play();
        
        // HUDAnimations._flickerTween = false;
    }
}
