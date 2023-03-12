using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VectorGraphics;
using DG.Tweening;

public class SVGFlickerTween : MonoBehaviour
{

    void Start()
    {
        StartFlashSequence();
    }
    private void StartFlashSequence()
    {
        Unity.VectorGraphics.SVGImage svgImage = gameObject.GetComponent<SVGImage>();

        // Create the fade sequence
        Sequence fadeSequence = DOTween.Sequence()
            .Append(svgImage.DOFade(0.5f, 0.08f))
            .AppendInterval(Random.Range(0.08f, 0.15f))
            .Append(svgImage.DOFade(0, 0.12f))
            .SetLoops(2, LoopType.Yoyo);

        // Start the sequence
        fadeSequence.Play();
    }
}
