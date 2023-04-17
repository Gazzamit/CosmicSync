using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VectorGraphics;

public class HUDAnimations : MonoBehaviour
{
    public List<GameObject> _SVGFlickerTweenObjects;

    //set active/inactive
    private GameObject _mainMenu, _spaceshipHolder, _targetHolder, _mainInGameUI, _targetingSVG, _settings, _dialoguePanelObject;

    //transform location set in inspector
    public Transform _leftRing, _rightRing, _targetSquare, _menuRingBlue, _settingsRingBlue, _dialoguePanel, _blackCanvas;

    public Ease _easeType;
    //transfrom start location
    private Vector3 _leftRingStartPos, _rightRingStartPos, _targetSquareStartPos, _menuRingStartPos, _settingsRingStartPos, _dialoguePanelStartPos;

    public static bool _switchingHUD = false, _flickerTween = false, _showDialogue = false, _hideDialogue = false, _fadeToBlack = false, _fadeFromBlack = false, _flickerHUD = false;

    [SerializeField] private int _newXMenuShift = 2000, _newYMenuShift = 1500;
    private void Awake()
    {
        //for set active / inactive
        _mainMenu = GameObject.Find("MainMenu");
        _spaceshipHolder = GameObject.Find("SpaceShipHolder");
        _targetHolder = GameObject.FindGameObjectWithTag("TargetHolder");
        _mainInGameUI = GameObject.Find("MainInGameUI");
        _targetingSVG = GameObject.Find("TargetingSVG");
        _settings = GameObject.Find("Settings");
        _dialoguePanelObject = GameObject.FindGameObjectWithTag("Dialogue");

        //for Transforms
        _blackCanvas = GameObject.Find("BlackoutPanel").transform;

    }
    private void Start()
    {
        //Get original Vector3 Locations;
        _leftRingStartPos = _leftRing.localPosition;
        _rightRingStartPos = _rightRing.localPosition;
        _targetSquareStartPos = _targetSquare.localPosition;
        _menuRingStartPos = _menuRingBlue.localPosition;
        _settingsRingStartPos = _settingsRingBlue.localPosition;
        _dialoguePanelStartPos = _dialoguePanel.localPosition;


        //Start state
        //move left ring off screen to left
        _leftRing.localPosition = new Vector3(-_newXMenuShift, 0, 0);
        //move right ring off to the right
        _rightRing.localPosition = new Vector3(_newXMenuShift, 0, 0);
        //move target below screen
        _targetSquare.localPosition = new Vector3(0, -_newYMenuShift, 0);
        //move menu Ring off top of screen
        _menuRingBlue.localPosition = new Vector3(0, _newYMenuShift, 0);
        //move settings Ring off the bottom of the screen
        _settingsRingBlue.localPosition = new Vector3(0, -_newYMenuShift, 0);
        //move dialogue panel off the bottom of the screen
        _dialoguePanel.localPosition = new Vector3(0, -_newYMenuShift, 0);


        // Normal run Game Start - activate Mainmenu
        if (GameManagerDDOL._normalStart == true && GameManagerDDOL._doWelcome == false)
        {
            Debug.Log("HUDA - Normal Game Start");
            StartCoroutine(ActivateMenuObjects());
            MoveHUDGameLauncher();
            GameManagerDDOL._normalStart = false;
            _switchingHUD = false;
        }
        //welcome dialogue - in game before ship lasered
        if (GameManagerDDOL._doWelcome == true)
        {
            Debug.Log("HUDA - doWelcome Game start");
            StartCoroutine(ActivateGameObjects()); //activate targets for start
            MoveHUDMenuToGame(); //show in game HUD
        }
    }

    void FixedUpdate()
    {
        if (_flickerHUD == true)
        {
            _flickerHUD = false;
            StartCoroutine(FlickerHUD());
        }

        if (GameManagerDDOL._currentMode == GameManagerDDOL.GameMode.MainMenu && _switchingHUD == true)
        {
            //If main Menu called from Game
            if (GameManagerDDOL._previousMode == GameManagerDDOL.GameMode.Game)
            {
                _switchingHUD = false;
                StartCoroutine(ActivateMenuObjects());
                MoveHUDGameToMenu();
                FlickerTween();
            }
            //If Menu called from settings
            else if (GameManagerDDOL._previousMode == GameManagerDDOL.GameMode.SettingsMenu)
            {
                _switchingHUD = false;
                StartCoroutine(ActivateMenuObjects());
                MoveHUDSettingsToMenu();
                FlickerTween();
            }
        }
        //If Game called (and not first run)
        else if (GameManagerDDOL._currentMode == GameManagerDDOL.GameMode.Game && _switchingHUD == true)
        {
            _switchingHUD = false;
            StartCoroutine(ActivateGameObjects());
            MoveHUDMenuToGame();
            FlickerTween();
            ScoreManager._instance.TriggerCountdownCoroutine();
        }
        //If settings called (will always be from Main menu)
        else if (GameManagerDDOL._currentMode == GameManagerDDOL.GameMode.SettingsMenu && _switchingHUD == true)
        {
            _switchingHUD = false;
            StartCoroutine(ActivateSettingsObjects());
            MoveHUDMenuToSettings();
            FlickerTween();
        }

        if (_showDialogue == true)
        {
            _showDialogue = false;
            ShowDialogue();
        }
        else if (_hideDialogue == true)
        {
            _hideDialogue = false;
            HideDialogue();
        }

        if (_fadeFromBlack == true)
        {
            Debug.Log("HUDA - FadeFromBlack");
            _fadeFromBlack = false;
            FadeFromBlack();
        }
        else if (_fadeToBlack == true)
        {
            Debug.Log("HUDA - FadeToBlack");
            _fadeToBlack = false;
            FadeToBlack();
        }
    }

    IEnumerator FlickerHUD()
    {
        float _timer = 7.8f;
        Debug.Log("HUDA - FlickerHUD for: " + _timer);
        float _startTime = Time.time;
        while (Time.time - _startTime < _timer)
        {
            _mainInGameUI.SetActive(true);
            yield return new WaitForSeconds(Random.Range(0.01f, 0.1f));
            _mainInGameUI.SetActive(false);
            yield return new WaitForSeconds(Random.Range(0.05f, 0.15f));
        }
    }

    //called directly from dialogue scripts
    public void FlickerTween()
    {
        foreach (GameObject obj in _SVGFlickerTweenObjects)
        {
            obj.GetComponent<SVGFlickerTween>().StartFlashSequence();
        }
    }
    public void ShowDialogue()
    {
        _dialoguePanel.DOLocalMove(_dialoguePanelStartPos, 0.3f).SetEase(_easeType);
        CanvasGroup _canvasGroup = _dialoguePanel.GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;

        // Create the fade sequence
        Sequence _sequence = DOTween.Sequence()
            .Append(_canvasGroup.DOFade(1f, 1f))
            .SetLoops(1, LoopType.Incremental);

        // Start the sequence
        _sequence.Play();
    }

    public void HideDialogue()
    {
        _dialoguePanel.DOLocalMove(_dialoguePanelStartPos + new Vector3(0, -2000, 0), 0.3f).SetEase(_easeType);
        CanvasGroup _canvasGroup = _dialoguePanel.GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 1;

        // Create the fade sequence
        Sequence _sequence = DOTween.Sequence()
            .Append(_canvasGroup.DOFade(0f, 1f))
            .SetLoops(1, LoopType.Incremental);

        // Start the sequence
        _sequence.Play();
    }

    public void MoveDialogueUp()
    {
        if (_dialoguePanel == null)
        {
            Debug.Log("_dialoguePanel not found");
        }
        _dialoguePanel.DOLocalMove(_dialoguePanelStartPos + new Vector3(0, 1560, 0), 0.3f).SetEase(_easeType);
    }

    public void MoveDialigueDown()
    {
        _dialoguePanel.DOLocalMove(_dialoguePanelStartPos, 0.3f).SetEase(_easeType);
    }

    public void FadeFromBlack()
    {
        CanvasGroup _canvasGroup = _blackCanvas.GetComponent<CanvasGroup>();

        // Create the fade sequence
        Sequence _sequence = DOTween.Sequence()
            .Append(_canvasGroup.DOFade(0f, 1f))
            .SetLoops(1, LoopType.Incremental);

        // Start the sequence
        _sequence.Play();
    }

    public void FadeToBlack()
    {
        CanvasGroup _canvasGroup = _blackCanvas.GetComponent<CanvasGroup>();
        //set canvas to aplha 1 if doWelcome
        if (GameManagerDDOL._doWelcome == true)
        {
            Debug.Log("HUDA - Setting Black canvas Alpha=1");
            _canvasGroup.alpha = 1;
        }
        else
        {
            // Create the fade sequence
            Debug.Log("HUDA - Fading to black");
            Sequence _sequence = DOTween.Sequence()
                .Append(_canvasGroup.DOFade(1f, 1f))
                .SetLoops(1, LoopType.Incremental);

            // Start the sequence
            _sequence.Play();
        }
    }
    public void MoveHUDGameLauncher()
    {
        Debug.Log("HA - MoveHUDGameLauncher");
        _menuRingBlue.DOLocalMove(_menuRingStartPos, 0.3f).SetEase(_easeType);
        GameManagerDDOL._currentMode = GameManagerDDOL.GameMode.MainMenu;
    }

    public void MoveHUDGameToMenu()
    {
        Debug.Log("HA - MoveHUDGameToMenu");
        _leftRing.DOLocalMove(_leftRingStartPos + new Vector3(-_newXMenuShift, 0, 0), .3f).SetEase(_easeType);
        _rightRing.DOLocalMove(_rightRingStartPos + new Vector3(_newXMenuShift, 0, 0), .3f).SetEase(_easeType);
        _targetSquare.DOLocalMove(_targetSquareStartPos + new Vector3(0, -_newYMenuShift, 0), .3f).SetEase(_easeType);
        _menuRingBlue.DOLocalMove(_menuRingStartPos, .3f).SetEase(_easeType);
    }

    public void MoveHUDMenuToGame()
    {
        Debug.Log("HA - MoveHUDMenuToGame");
        _leftRing.DOLocalMove(_leftRingStartPos, .3f).SetEase(_easeType);
        _rightRing.DOLocalMove(_rightRingStartPos, .3f).SetEase(_easeType);
        _targetSquare.DOLocalMove(_targetSquareStartPos, .3f).SetEase(_easeType);
        _menuRingBlue.DOLocalMove(_menuRingStartPos + new Vector3(0, _newYMenuShift, 0), .3f).SetEase(_easeType);
    }

    public void MoveHUDMenuToSettings()
    {
        Debug.Log("HA - MoveHUDMenuToSettings");
        _menuRingBlue.DOLocalMove(_menuRingStartPos + new Vector3(0, _newYMenuShift, 0), .3f).SetEase(_easeType);
        _settingsRingBlue.DOLocalMove(_settingsRingStartPos, .3f).SetEase(_easeType);
    }

    public void MoveHUDSettingsToMenu()
    {
        Debug.Log("MoveHUDSettingsToMenu");
        _menuRingBlue.DOLocalMove(_menuRingStartPos, .3f).SetEase(_easeType);
        _settingsRingBlue.DOLocalMove(_settingsRingStartPos + new Vector3(0, -_newYMenuShift, 0), .3f).SetEase(_easeType);
    }


    IEnumerator ActivateGameObjects()
    {
        //activate Game HUD Objects then move them
        _targetHolder.SetActive(true);
        _mainInGameUI.SetActive(true);
        _targetingSVG.SetActive(true);

        //move objects, then disable them
        yield return new WaitForSeconds(0.3f);
        Debug.Log("HUDA - Activated Game Objects");
    }

    IEnumerator ActivateMenuObjects()
    {
        //activate Menu Objects then move them
        _mainMenu.SetActive(true);

        //move objects, then disable them
        yield return new WaitForSeconds(0.3f);
        //_settings.SetActive(false);
        _targetHolder.SetActive(false);
        //_mainInGameUI.SetActive(false);
    }

    IEnumerator ActivateSettingsObjects()
    {
        //activate Menu Objects then move them
        //_settings.SetActive(true);

        //move objects, then disable them
        yield return new WaitForSeconds(0.3f);
        //_mainMenu.SetActive(false);
    }

}