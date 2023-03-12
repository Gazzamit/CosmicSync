using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VectorGraphics;

public class GameHUDManager : MonoBehaviour
{
    public Transform _leftRing, _rightRing, _target;

    public Ease _easeType;
    private Vector3 _leftRingStartPos, _rightRingStartPos, _targetStartPos;

    private void Start()
    {
        //Get original Vector3 Locations;
        _leftRingStartPos = _leftRing.localPosition;
        _rightRingStartPos = _rightRing.localPosition;
        _targetStartPos = _target.localPosition;

        //move left ring off screen to left
        _leftRing.localPosition = new Vector3(-2000, 0, 0);
        //move right ring off to the right
        _rightRing.localPosition = new Vector3(2000, 0, 0);
        //move target below screen
        _target.localPosition = new Vector3(0, -1500, 0);

        MoveHUDStart();
    }

    public void MoveHUDStart()
    {
        _leftRing.DOLocalMove(_leftRingStartPos, .3f).SetEase(_easeType);
        _rightRing.DOLocalMove(_rightRingStartPos, .3f).SetEase(_easeType);
        _target.DOLocalMove(_targetStartPos, .3f).SetEase(_easeType);
    }
}