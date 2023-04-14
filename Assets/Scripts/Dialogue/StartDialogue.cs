using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDialogue : MonoBehaviour
{
    private Dialogue _welcome1, _welcome2, _welcome3, _welcome4;

    private GameObject _mainMenu, _dialoguePanel, _blackCanvasPanel, _UIObject;

    //    //[SerializeField] private GameObject _smoke, _haze;

    void Start()
    {
        _mainMenu = GameObject.FindGameObjectWithTag("MainMenu");
        _welcome1 = GameObject.FindGameObjectWithTag("Welcome1").GetComponent<DialogueTrigger>()._dialogue;
        _welcome2 = GameObject.FindGameObjectWithTag("Welcome2").GetComponent<DialogueTrigger>()._dialogue;
        _welcome3 = GameObject.FindGameObjectWithTag("Welcome3").GetComponent<DialogueTrigger>()._dialogue;
        _welcome4 = GameObject.FindGameObjectWithTag("Welcome4").GetComponent<DialogueTrigger>()._dialogue;


        _UIObject = GameObject.FindGameObjectWithTag("UI");
        //        //_smoke = GameObject.FindGameObjectWithTag("WelcomeSmoke");
        //        //_haze = GameObject.FindGameObjectWithTag("WelcomeHaze");

        StartCoroutine(WriteDialogueToDisk());

        if (GameManagerDDOL._doWelcome == true)
        {
            StartCoroutine(StartWelcomeDialogues());
        }
    }

    IEnumerator StartWelcomeDialogues()
    {
        Debug.Log("SD - Do Welcome");


        // welcome 1
        //set to black at start
        HUDAnimations._fadeToBlack = true;
        //wait till black
        yield return new WaitForSeconds(1f);
        //fadeup from black
        HUDAnimations._fadeFromBlack = true;
        //add temporary rotation to spaceship at start
        for (int i = 0; i < 12; i++)
        {
            OnCollidePortal._addPortalTurbulanceNow = true;
            yield return new WaitForSeconds(0.2f);
        }
        //add fx to spaceship
        //        //_smoke.GetComponent<ParticleSystem>().Play();
        //        //_haze.GetComponent<ParticleSystem>().Play();
        //wait for menu flicker to end then...
        yield return new WaitForSeconds(2.0f);
        //show dialogue box
        HUDAnimations._showDialogue = true;
        //begin dialogue 1
        yield return StartCoroutine(ProcessDialogue(_welcome1));
        //hide dialogue box
        HUDAnimations._hideDialogue = true;
        //wait for a second
        yield return new WaitForSeconds(1.0f);

        //welcome 2: Hello Jamie, my name is Rai
        HUDAnimations._showDialogue = true;
        //stop smoke fx
        //        //_smoke.GetComponent<ParticleSystem>().Stop();
        //        //_haze.GetComponent<ParticleSystem>().Stop();
        //begin dalogue 2
        yield return StartCoroutine(ProcessDialogue(_welcome2));
        // hide dialogue box
        HUDAnimations._hideDialogue = true;
        yield return new WaitForSeconds(1.0f);

        //welcome 3
        HUDAnimations._showDialogue = true;
        //activate main menu (set it to game forst so it will trigger change)
        GameManagerDDOL._currentMode = GameManagerDDOL.GameMode.Game;
        GameManagerDDOL._currentMode = GameManagerDDOL.GameMode.MainMenu;
        HUDAnimations._switchingHUD = true; //adds flicker as well
        yield return StartCoroutine(ProcessDialogue(_welcome3));

        //move up dialogue mid sentence list
        _UIObject.GetComponent<HUDAnimations>().MoveDialogueUp();
        yield return StartCoroutine(ProcessDialogue(_welcome4));

        yield return null;

    }
    IEnumerator ProcessDialogue(Dialogue _dialogue)
    {
        DialogueManager._instance.StartDialogue(_dialogue);

        yield return new WaitUntil(() => !DialogueManager._instance._isDialogue);

    }
    public void TriggerWelcomeDialogues()
    {
        StartCoroutine(StartWelcomeDialogues());
    }

    // trigger abouve with
    // StartDialogue startDialogueScript = GetComponent<StartDialogue>();
    // startDialogueScript.TriggerWelcomeDialogues();

    //not currently used
    // IEnumerator AddSmokeEffect()
    // {
    //     // Instantiate the particle effect prefab 
    //     GameObject _fx1 = Instantiate(_smoke, transform);
    //     _fx1.transform.localPosition = Vector3.zero;

    //     // Enable the particle effect component on the particle object
    //     ParticleSystem _ps1 = _fx1.GetComponent<ParticleSystem>();
    //     if (_ps1 != null)
    //     {
    //         _ps1.Play();
    //     }
    //     yield return new WaitForSeconds(2f);
    //     //_ps1.Stop();
    // }

    // //not currently used
    // IEnumerator AddHazeEffect()
    // {
    //     // Instantiate the particle effect prefab 
    //     GameObject _fx2 = Instantiate(_haze, transform);
    //     _fx2.transform.localPosition = Vector3.zero;

    //     // Enable the particle effect component on the particle object
    //     ParticleSystem _ps2 = _fx2.GetComponent<ParticleSystem>();
    //     if (_ps2 != null)
    //     {
    //         _ps2.Play();
    //     }
    //     yield return new WaitForSeconds(2f);
    //     //_ps1.Stop();
    // }

    IEnumerator WriteDialogueToDisk()
    {
        // Create a StreamWriter to write the dialogue to a text file
        System.IO.StreamWriter file = new System.IO.StreamWriter(@"/Users/gazzamit/Documents/welcome_dialogue.txt");

        // Write the welcome dialogues to the text file
        file.WriteLine("Welcome Dialogue 1: " + string.Join(", ", _welcome1._sentences));
        file.WriteLine("Welcome Dialogue 2: " + string.Join(", ", _welcome2._sentences));
        file.WriteLine("Welcome Dialogue 3: " + string.Join(", ", _welcome3._sentences));

        // Close the StreamWriter
        file.Close();

        yield return null;
    }


}
