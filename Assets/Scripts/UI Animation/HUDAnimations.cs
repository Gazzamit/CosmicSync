using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VectorGraphics;

public class HUDAnimations : MonoBehaviour
{

    private GameObject _mainMenu, _spaceshipHolder, _targetHolder, _mainInGameUI, _targetingSVG, _settings;

    public Transform _leftRing, _rightRing, _targetSquare, _menuRingBlue, _settingsRingBlue;

    public Ease _easeType;
    private Vector3 _leftRingStartPos, _rightRingStartPos, _targetStartPos, _menuRingStartPos, _settingsRingStartPos;

    public static bool _switchingHUD = false;

    private void Awake()
    {
        //for set active / inactive
        _mainMenu = GameObject.Find("MainMenu");
        _spaceshipHolder = GameObject.Find("SpaceShipHolder");
        _targetHolder = GameObject.FindGameObjectWithTag("TargetHolder");
        _mainInGameUI = GameObject.Find("MainInGameUI");
        _targetingSVG = GameObject.Find("TargetingSVG");
        _settings = GameObject.Find("Settings");
        
        //if inactive in editor
        _mainMenu.SetActive(true);
        _mainInGameUI.SetActive(true);
        _settings.SetActive(true);

    }
    private void Start()
    {
        //Get original Vector3 Locations;
        _leftRingStartPos = _leftRing.localPosition;
        _rightRingStartPos = _rightRing.localPosition;
        _targetStartPos = _targetSquare.localPosition;
        _menuRingStartPos = _menuRingBlue.localPosition;
        _settingsRingStartPos = _settingsRingBlue.localPosition;

        //Start state
        //move left ring off screen to left
        _leftRing.localPosition = new Vector3(-2000, 0, 0);
        //move right ring off to the right
        _rightRing.localPosition = new Vector3(2000, 0, 0);
        //move target below screen
        _targetSquare.localPosition = new Vector3(0, -1500, 0);
        //move menu Ring off top of screen
        _menuRingBlue.localPosition = new Vector3(0, 1500, 0);
        //move settings Ring off the bottom of the screen
        _settingsRingBlue.localPosition = new Vector3(0, -1500, 0);

        //First run Game Start on Mainmenu
        if (GameManagerDDOL._init == true)
        {
            StartCoroutine(ActivateMenuObjects());
            MoveHUDGameLauncher();
            GameManagerDDOL._init = false;
        }
    }

    void Update()
    {
        if (GameManagerDDOL._currentMode == GameManagerDDOL.GameMode.MainMenu && _switchingHUD == true)
        {
            //If main Menu called from Game
            if (GameManagerDDOL._previousMode == GameManagerDDOL.GameMode.Game)
            {
                StartCoroutine(ActivateMenuObjects());
                MoveHUDGameToMenu();
                _switchingHUD = false;
            }
            //If Menu called from settings
            else if (GameManagerDDOL._previousMode == GameManagerDDOL.GameMode.SettingsMenu)
            {
                StartCoroutine(ActivateMenuObjects());
                MoveHUDSettingsToMenu();
                _switchingHUD = false;
            }
        }
        //If Game called (and not first run)
        else if (GameManagerDDOL._currentMode == GameManagerDDOL.GameMode.Game && _switchingHUD == true)
        {
            StartCoroutine(ActivateGameObjects());
            MoveHUDMenuToGame();
            _switchingHUD = false;
        }
        //If settings called (will always be from Main menu)
        else if (GameManagerDDOL._currentMode == GameManagerDDOL.GameMode.SettingsMenu && _switchingHUD == true)
        {
            StartCoroutine(ActivateSettingsObjects());
            MoveHUDMenuToSettings();
            _switchingHUD = false;

        }
    }

    public void MoveHUDGameLauncher()
    {
        Debug.Log("HA - MoveHUDGameLauncher");
        _menuRingBlue.DOLocalMove(new Vector3(0, 0, 0), .3f).SetEase(_easeType);
        GameManagerDDOL._currentMode = GameManagerDDOL.GameMode.MainMenu;
    }

    public void MoveHUDGameToMenu()
    {
        Debug.Log("HA - MoveHUDGameToMenu");
        _leftRing.DOLocalMove(_leftRing.localPosition + new Vector3(-2000, 0, 0), .3f).SetEase(_easeType);
        _rightRing.DOLocalMove(_rightRing.localPosition + new Vector3(2000, 0, 0), .3f).SetEase(_easeType);
        _targetSquare.DOLocalMove(_targetSquare.localPosition + new Vector3(0, -1500, 0), .3f).SetEase(_easeType);
        _menuRingBlue.DOLocalMove(_menuRingStartPos, .3f).SetEase(_easeType);
    }

    public void MoveHUDMenuToGame()
    {
        Debug.Log("HA - MoveHUDMenuToGame");
        _leftRing.DOLocalMove(_leftRingStartPos, .3f).SetEase(_easeType);
        _rightRing.DOLocalMove(_rightRingStartPos, .3f).SetEase(_easeType);
        _targetSquare.DOLocalMove(_targetStartPos, .3f).SetEase(_easeType);
        _menuRingBlue.DOLocalMove(_menuRingBlue.localPosition + new Vector3(0, 1500, 0), .3f).SetEase(_easeType);
    }

    public void MoveHUDMenuToSettings()
    {
        Debug.Log("HA - MoveHUDMenuToSettings");
        _menuRingBlue.DOLocalMove(_menuRingBlue.localPosition + new Vector3(0, 1500, 0), .3f).SetEase(_easeType);
        _settingsRingBlue.DOLocalMove(_settingsRingStartPos, .3f).SetEase(_easeType);
    }

    public void MoveHUDSettingsToMenu()
    {
        Debug.Log("MoveHUDSettingsToMenu");
        _menuRingBlue.DOLocalMove(_menuRingStartPos, .3f).SetEase(_easeType);
        _settingsRingBlue.DOLocalMove(_settingsRingBlue.localPosition + new Vector3(0, -1500, 0), .3f).SetEase(_easeType);
    }


    IEnumerator ActivateGameObjects()
    {
        //activate Game HUD Objects then move them
        _targetHolder.SetActive(true);
        _mainInGameUI.SetActive(true);
        _targetingSVG.SetActive(true);

        //move objects, then disable them
        yield return new WaitForSeconds(0.3f);
    }

    IEnumerator ActivateMenuObjects()
    {
        //activate Menu Objects then move them
        _mainMenu.SetActive(true);

        //move objects, then disable them
        yield return new WaitForSeconds(0.3f);
        _settings.SetActive(false);
        _targetHolder.SetActive(false);
        _mainInGameUI.SetActive(false);
    }

    IEnumerator ActivateSettingsObjects()
    {
        //activate Menu Objects then move them
        _settings.SetActive(true);

        //move objects, then disable them
        yield return new WaitForSeconds(0.3f);
        _mainMenu.SetActive(false);
    }

}