using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VectorGraphics;

public class HUDAnimations : MonoBehaviour
{
    public Transform _leftRing, _rightRing, _target, _menuRing;

    public Ease _easeType;
    private Vector3 _leftRingStartPos, _rightRingStartPos, _targetStartPos, _menuRingStartPos;

    private bool _firstRun = true; //run Game start transition once
    private bool _switchingMenu = true;

    private void Start()
    {
        //Get original Vector3 Locations;
        _leftRingStartPos = _leftRing.localPosition;
        _rightRingStartPos = _rightRing.localPosition;
        _targetStartPos = _target.localPosition;
        _menuRingStartPos = _menuRing.localPosition;

        //Start state
        //move left ring off screen to left
        _leftRing.localPosition = new Vector3(-2000, 0, 0);
        //move right ring off to the right
        _rightRing.localPosition = new Vector3(2000, 0, 0);
        //move target below screen
        _target.localPosition = new Vector3(0, -1500, 0);
        //move menu Ring off top of screen
        _menuRing.localPosition = new Vector3 (0, 1500, 0);

        MoveHUDGameStart();
    }

    void Update()
    {
        //_isGame is false when menu called
        if (InputMapSwitch._isGame == false && InputMapSwitch._switchingHUD == true)
        {
            MoveHUDGameToMenu();
            InputMapSwitch._switchingHUD = false;
        }
        //isGame is true if Game called
        else if (InputMapSwitch._isGame == true && InputMapSwitch._switchingHUD == true && _firstRun == false)
        {
            MoveHUDMenuToGame();
            InputMapSwitch._switchingHUD = false;
        }
    }

    public void MoveHUDGameStart()
    {
        _leftRing.DOLocalMove(_leftRingStartPos, .3f).SetEase(_easeType);
        _rightRing.DOLocalMove(_rightRingStartPos, .3f).SetEase(_easeType);
        _target.DOLocalMove(_targetStartPos, .3f).SetEase(_easeType);
        _firstRun = false;
    }

    public void MoveHUDGameToMenu()
    {
        _leftRing.DOLocalMove(_leftRing.localPosition + new Vector3(-2000, 0, 0), .3f).SetEase(_easeType);
        _rightRing.DOLocalMove(_rightRing.localPosition + new Vector3(2000, 0, 0), .3f).SetEase(_easeType);
        _target.DOLocalMove(_target.localPosition + new Vector3(0, -1500, 0), .3f).SetEase(_easeType);
        _menuRing.DOLocalMove(new Vector3(0, 0, 0), .3f).SetEase(_easeType);
    }

    public void MoveHUDMenuToGame()
    {
        _leftRing.DOLocalMove(_leftRingStartPos, .3f).SetEase(_easeType);
        _rightRing.DOLocalMove(_rightRingStartPos, .3f).SetEase(_easeType);
        _target.DOLocalMove(_targetStartPos, .3f).SetEase(_easeType);
        _menuRing.DOLocalMove(_menuRing.localPosition + new Vector3 (0, 1500, 0), .3f).SetEase(_easeType);
    }

}