using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WelcomeDialogue : MonoBehaviour
{
    private Dialogue _welcome1, _welcome2, _welcome3, _welcome4, _welcome5, _welcome6, _welcome7;

    private GameObject _mainMenu, _dialoguePanel, _blackCanvasPanel, _UIObject, _velocityObj, _mainInGameUI;


    private Material _skyboxMat;

    [SerializeField] private GameObject _smoke;
    void Awake()
    {
        _skyboxMat = RenderSettings.skybox;
    }

    void Start()
    {
        _mainMenu = GameObject.FindGameObjectWithTag("MainMenu");
        _velocityObj = GameObject.FindGameObjectWithTag("Velocity");
        _mainInGameUI = GameObject.Find("MainInGameUI");
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

    void Update()
    {

    }

    IEnumerator StartWelcomeDialogues()
    {
        Debug.Log("SD - Do Welcome");

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
        for (int i = 0; i < 40; i++)
        {
            OnCollidePortal._addPortalTurbulanceNow = true;
            if (i == 12) HUDAnimations._flickerHUD = true;
            yield return new WaitForSeconds(0.25f);
        }

        _skyboxMat.SetColor("_Tint", DialogueManager._instance._skyboxGameColour);
        //add fx to spaceship
        _smoke.GetComponent<ParticleSystem>().Play();


        //Welcome 1: RAI - hi Jamie....
        yield return new WaitForSeconds(2.0f);
        HUDAnimations._showDialogue = true;//show dialogue box
        yield return StartCoroutine(ProcessDialogue(_welcome1));
        HUDAnimations._hideDialogue = true;//hide dialogue box
        yield return new WaitForSeconds(1.0f);//wait for a second

        //welcome 2: r-a-i rebooted...
        HUDAnimations._showDialogue = true;
        _smoke.GetComponent<ParticleSystem>().Stop();//stop smoke fx
        yield return StartCoroutine(ProcessDialogue(_welcome2));
        HUDAnimations._hideDialogue = true; // hide dialogue box
        yield return new WaitForSeconds(1.0f);

        //welcome 3: JENZI...
        HUDAnimations._showDialogue = true;
        yield return StartCoroutine(ProcessDialogue(_welcome3));
        HUDAnimations._hideDialogue = true; // hide dialogue box
        yield return new WaitForSeconds(1.0f);

        //welcome 4: RAI - creates log file... launching hud...
        HUDAnimations._showDialogue = true;
        yield return StartCoroutine(ProcessDialogue(_welcome4));
        HUDAnimations._hideDialogue = true; // hide dialogue box
        yield return new WaitForSeconds(1.0f);
        //activate main menu (set it to game first so it will trigger change)
        GameManagerDDOL._currentMode = GameManagerDDOL.GameMode.Game;
        GameManagerDDOL._currentMode = GameManagerDDOL.GameMode.MainMenu;
        HUDAnimations._switchingHUD = true; //switch to menu HUDD
        yield return new WaitForSeconds(0.5f); //wait for menu to be in place
        _mainInGameUI.SetActive(true); // was inactive affter HUDFlicker

        //welcome5 : RAI - press escape
        HUDAnimations._showDialogue = true;
        yield return StartCoroutine(ProcessDialogue(_welcome5));
        DialogueManager._instance._pauseAdvance = true; //stop I working
        yield return new WaitUntil(() => Keyboard.current.escapeKey.wasPressedThisFrame); //press escape
        DialogueManager._instance._pauseAdvance = false; //start I working
        GameManagerDDOL._currentMode = GameManagerDDOL.GameMode.SettingsMenu;
        HUDAnimations._switchingHUD = true; //switch to setting HUDD

        // A/V sync
        yield return StartCoroutine(ProcessDialogue(_welcome6));
        _UIObject.GetComponent<HUDAnimations>().MoveDialogueUp();//move up dialogue
        //Pitch/Yaw tweak
        yield return StartCoroutine(ProcessDialogue(_welcome7));


        _velocityObj.SetActive(true);
        yield return null;

    }
    IEnumerator ProcessDialogue(Dialogue _dialogue)
    {
        DialogueManager._instance.StartDialogue(_dialogue);

        //wait here until no dialogue
        yield return new WaitUntil(() => !DialogueManager._instance._isDialogue);

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


}
