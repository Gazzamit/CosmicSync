using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StartDialogue : MonoBehaviour
{
    private Dialogue _welcome1, _welcome2, _welcome3, _welcome4, _welcome5, _welcome6, _welcome7;

    private GameObject _mainMenu, _dialoguePanel, _blackCanvasPanel, _UIObject, _velocityObj;


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
  
        yield return new WaitForSeconds(2.0f);
        HUDAnimations._showDialogue = true;//show dialogue box
        yield return StartCoroutine(ProcessDialogue(_welcome1));
  

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
