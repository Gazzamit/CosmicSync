using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PanelTween : MonoBehaviour
{
    public GameObject _panel;

    void Start()
    {

    }

    void LateUpdate()
    {
        //run seq on HUD change to Menu
        //if (HUDAnimations._showPanelHUD == true)
        {
            //ShowPanel();
            //Debug.Log("HUD Flicker Panel tween");
           // HUDAnimations._flickerPanelHUD = false;
        }
    }

    private void ShowPanel()
    {
        CanvasGroup _canvasGroup = _panel.GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;

        // Create the fade sequence
        Sequence _flickerSequence = DOTween.Sequence()
            .Append(_canvasGroup.DOFade(1f, 0.5f))
            .SetLoops(1, LoopType.Incremental);

        // Start the sequence
        // _flickerSequence.Play();
    }
}
