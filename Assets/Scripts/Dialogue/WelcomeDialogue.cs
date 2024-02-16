using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class WelcomeDialogue : MonoBehaviour
{
    private Dialogue _welcome1, _welcome2, _welcome3, _welcome4, _welcome5, _welcome6, _welcome7;

    private GameObject _mainMenu, _dialoguePanel, _blackCanvasPanel, _UIObject, _velocityObj;

    public GameObject[] _enableDisableMenuObj;

    [SerializeField] GameObject _skipObj;

    private TextMeshProUGUI _skipText;

    private Material _skyboxMat;

    [SerializeField] private GameObject _smoke;

    [SerializeField] int i = 0;
    public PlayerInput _playerInput; // Reference to the PlayerInput

    public static bool _triggerRestartMenuLoop = false;
    [SerializeField] private AudioManager _audioManager;

    public static bool _resetAudioWhenRaiReboots = false;
    void Awake()
    {
        _skyboxMat = RenderSettings.skybox;
    }

    void Start()
    {
        _skipText = _skipObj.GetComponent<TextMeshProUGUI>();

        _mainMenu = GameObject.FindGameObjectWithTag("MainMenu");
        _velocityObj = GameObject.FindGameObjectWithTag("Velocity");

        _welcome1 = GameObject.FindGameObjectWithTag("Welcome1").GetComponent<DialogueTrigger>()._dialogue;
        _welcome2 = GameObject.FindGameObjectWithTag("Welcome2").GetComponent<DialogueTrigger>()._dialogue;
        _welcome3 = GameObject.FindGameObjectWithTag("Welcome3").GetComponent<DialogueTrigger>()._dialogue;
        _welcome4 = GameObject.FindGameObjectWithTag("Welcome4").GetComponent<DialogueTrigger>()._dialogue;
        _welcome5 = GameObject.FindGameObjectWithTag("Welcome5").GetComponent<DialogueTrigger>()._dialogue;
        _welcome6 = GameObject.FindGameObjectWithTag("Welcome6").GetComponent<DialogueTrigger>()._dialogue;
        _welcome7 = GameObject.FindGameObjectWithTag("Welcome7").GetComponent<DialogueTrigger>()._dialogue;

        _UIObject = GameObject.FindGameObjectWithTag("UI");
        _smoke = GameObject.FindGameObjectWithTag("WelcomeSmoke");

        StartCoroutine(WriteDialogueToDisk());

        if (GameManagerDDOL._doWelcome == true)
        {
            StartCoroutine(StartWelcomeDialogues());
        }

        //set skybox for normal gameplay
        if (GameManagerDDOL._doWelcome == false)
        {
            _skyboxMat.SetColor("_Tint", DialogueManager._instance._skyboxGameColour);
        }
    }

    IEnumerator StartWelcomeDialogues()
    {
        Debug.Log("WD - Welcome 1");

        if (DialogueManager._instance.DEV_BYPASS_INTRO == false)
        {

            // welcome 1
            HUDAnimations._hideDialogue = true;
            //set to black at start
            _skyboxMat.SetColor("_Tint", DialogueManager._instance._skyboxInWelcomeColor);
            HUDAnimations._fadeToBlack = true;
            //wait till black
            yield return new WaitForSeconds(1f);
            //fadeup from black
            //add forwad thrust and fire lasers
            SpaceshipControls._doWelcomeSpaceShipControls = true;
            yield return new WaitForSeconds(0.3f);
            HUDAnimations._fadeFromBlack = true;
            yield return new WaitForSeconds(8.8f);
            //hide velocity in warp tunnel
            _velocityObj.SetActive(false);

            //add temporary rotation to spaceship at start
            for (int k = 0; k < 40; k++)
            {
                OnCollidePortal._addPortalTurbulanceNow = true;
                if (k == 12) HUDAnimations._flickerHUD = true;
                yield return new WaitForSeconds(0.25f);
            }


            _skyboxMat.SetColor("_Tint", DialogueManager._instance._skyboxGameColour);
            //add fx to spaceship
            _smoke.GetComponent<ParticleSystem>().Play();

            //Welcome 1: RAI - hi Jamie....
            yield return new WaitForSeconds(2.0f);
            HUDAnimations._showDialogue = true; //show dialogue box
            Debug.Log("WD - Welcome 1: " + i);
            yield return StartCoroutine(ProcessDialogue(_welcome1));
            Debug.Log("WD - Welcome 1 Completed : " + i);
            HUDAnimations._hideDialogue = true; //hide dialogue box
            yield return new WaitForSeconds(1.0f); //wait for a second

            //welcome 2: r-a-i rebooted...
            HUDAnimations._showDialogue = true;
            _smoke.GetComponent<ParticleSystem>().Stop(); //stop smoke fx

            //reset on Rai Reeboot
            _resetAudioWhenRaiReboots = true;
            Debug.Log("WD - Audio reset: " + _resetAudioWhenRaiReboots);

            Debug.Log("WD - Welcome 2: " + i);
            yield return StartCoroutine(ProcessDialogue(_welcome2));
            Debug.Log("WD - Welcome 2 Completed : " + i);
            HUDAnimations._hideDialogue = true;  // hide dialogue box
            yield return new WaitForSeconds(1.0f);

            //welcome 3: JENZI...
            HUDAnimations._showDialogue = true;
            Debug.Log("WD - Welcome 3: " + i);
            yield return StartCoroutine(ProcessDialogue(_welcome3));
            Debug.Log("WD - Welcome 3 Completed : " + i);
            HUDAnimations._hideDialogue = true;  // hide dialogue box
            yield return new WaitForSeconds(1.0f);

            //welcome 4: RAI - creates log file... launching hud...
            HUDAnimations._showDialogue = true;
            Debug.Log("WD - Welcome 4: " + i);
            yield return StartCoroutine(ProcessDialogue(_welcome4));
            Debug.Log("WD - Welcome 4 Completed : " + i);
            HUDAnimations._hideDialogue = true;  // hide dialogue box
            yield return new WaitForSeconds(1.0f);

        }

        //activate main menu (set it to game first so it will trigger change)
        HUDAnimations._switchingHUD = true;
        GameManagerDDOL._currentMode = GameManagerDDOL.GameMode.Game;
        GameManagerDDOL._currentMode = GameManagerDDOL.GameMode.MainMenu;
        yield return new WaitForSeconds(0.5f); //wait for menu to be in place
        foreach (var _obj in _enableDisableMenuObj)
        {
            _obj.SetActive(true);
        }
        // was inactive affter HUDFlicker

        //force correct map
        SwitchActionMap("UIControls");
        Debug.Log("WD SelectUI controls action Map");
        HUDAnimations._showDialogue = true;

        //Menu loop script and another to reset coroutine if needed
        StartCoroutine(InMenuSettingsLoop());
        StartCoroutine(WatchForESC());

        _velocityObj.SetActive(true);
        yield return null;

    }

    IEnumerator WatchForESC()
    {
        while (true)
        {
            //default settings to detect menu and run appropriate dialogue
            if (_triggerRestartMenuLoop == true)
            {
                Debug.Log("WD -Restarting Menu / Setting Loop in WelcomeDialogue");
                _UIObject.GetComponent<HUDAnimations>().MoveDialogueDown();
                _triggerRestartMenuLoop = false;
                DialogueManager._instance._isDialogue = false;
                StopCoroutine(InMenuSettingsLoop());
                StartCoroutine(InMenuSettingsLoop());
            }
            yield return null;
        }
    }

    IEnumerator InMenuSettingsLoop()
    {
        GameManagerDDOL._doWelcome = false;
        while (true)

        {
            if (GameManagerDDOL._currentMode == GameManagerDDOL.GameMode.SettingsMenu)
            {
                _UIObject.GetComponent<HUDAnimations>().MoveDialogueDown();
                Debug.Log("WD - InMenuSettingsLoop: Settings");
                _skipText.text = "More (Space / Click)";
                yield return StartCoroutine(ProcessDialogue(_welcome6)); //AV Sync O/P
                Debug.Log("WD - Welcome 6 Completed");


                //if still in settings
                if (GameManagerDDOL._currentMode == GameManagerDDOL.GameMode.SettingsMenu)
                {
                    _UIObject.GetComponent<HUDAnimations>().MoveDialogueUp(); //move up dialogue
                    //Pitch/Yaw tweak
                    _skipText.text = "More (Space / Click)";
                    yield return StartCoroutine(ProcessDialogue(_welcome7)); //Pitch Yaw K/L
                    Debug.Log("WD - Welcome 7 Completed");

                }
            }
            if (GameManagerDDOL._currentMode == GameManagerDDOL.GameMode.MainMenu)
            {
                //welcome5 : RAI - press escape for settings
                _skipText.text = "";
                Debug.Log("WD - InMenuSettingsLoop: MainMenu");
                _UIObject.GetComponent<HUDAnimations>().MoveDialogueDown();
                yield return StartCoroutine(ProcessDialogue(_welcome5));
            }
        }

    }
    IEnumerator ProcessDialogue(Dialogue _dialogue)
    {
        DialogueManager._instance.StartDialogue(_dialogue);

        //wait here until _isDialogue == false
        yield return new WaitUntil(() => !DialogueManager._instance._isDialogue);
        // while (DialogueManager._instance._isDialogue)
        // {
        //     yield return null;
        // }
        // yield break;
    }
    public void TriggerWelcomeDialogues()
    {
        StartCoroutine(StartWelcomeDialogues());
    }

    IEnumerator WriteDialogueToDisk()
    {
        // Create a StreamWriter to write the dialogue to a text file
        System.IO.StreamWriter file = new System.IO.StreamWriter(@"/Users/gazzamit/Documents/welcome_dialogue.txt");

        // Write the welcome dialogues to the text file
        file.WriteLine("Welcome Dialogue 1: " + _welcome1._name + ", " + string.Join(", ", _welcome1._sentences));
        file.WriteLine("Welcome Dialogue 2: " + _welcome1._name + ", " + string.Join(", ", _welcome2._sentences));
        file.WriteLine("Welcome Dialogue 3: " + _welcome1._name + ", " + string.Join(", ", _welcome3._sentences));
        file.WriteLine("Welcome Dialogue 4: " + _welcome1._name + ", " + string.Join(", ", _welcome4._sentences));
        file.WriteLine("Welcome Dialogue 5: " + _welcome1._name + ", " + string.Join(", ", _welcome5._sentences));
        file.WriteLine("Welcome Dialogue 6: " + _welcome1._name + ", " + string.Join(", ", _welcome6._sentences));
        file.WriteLine("Welcome Dialogue 7: " + _welcome1._name + ", " + string.Join(", ", _welcome7._sentences));
        //file.WriteLine("Welcome Dialogue 1: " + _welcome1._name + ", " + string.Join(", ", _welcome1._sentences));

        // Close the StreamWriter
        file.Close();

        yield return null;
    }
    public void SwitchActionMap(string actionMapName)
    {
        PlayerInput[] playerInputs = PlayerInput.all.ToArray();

        foreach (PlayerInput playerInput in playerInputs)
        {
            playerInput.SwitchCurrentActionMap(actionMapName);
        }
    }


}
