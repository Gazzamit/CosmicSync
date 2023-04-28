using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    public bool DEV_BYPASS_INTRO;
    //singleton
    public static DialogueManager _instance;
    public Color _skyboxInWelcomeColor, _skyboxGameColour;

    public TextMeshProUGUI _nameText;
    public TextMeshProUGUI _dialogueText;

    //Handle Starting Dialogue
    public bool _isDialogue = false, _pauseAdvance = false;

    //Queue from sentances
    private Queue<string> _sentences;

    private Coroutine _typewritterCoroutine;

    private int _counter = 0;

    void Awake()
    {
        _instance = this;
        _sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue _dialogue)
    {
        _isDialogue = true;

        //safety check
        if (_dialogue == null)
        {
            Debug.LogError("Dialogue is null.");
            return;
        }

        //add Dotween animation?

        _nameText.text = _dialogue._name;

        //Clear previous sentances
        _sentences.Clear();

        //get sentances from Character (Rai, jenzi etc)
        foreach (string _sentence in _dialogue._sentences)
        {
            _sentences.Enqueue(_sentence);
        }

        //display the next sentence in the list
        DisplayNextSentence();

    }

    public void TriggerNextSentence(InputAction.CallbackContext _context)
    {
        if (_context.performed)
        {
            Debug.Log("DM - PressI context: " + _context.phase);
            DisplayNextSentence();
        }
    }

    public void DisplayNextSentence()
    {
        if (_pauseAdvance == false)
        {
            Debug.Log("Sentences Count: " + _sentences.Count);
            //if no sentences, end dialogue
            if (_sentences.Count == 0)
            {
                EndDialogue();
                return;
            }

            //Have sentances, so Dequeue
            string _sentence = _sentences.Dequeue();

            Debug.Log(" DM - DisplayNextSentence: " + _sentence);

            // Stop the running coroutine, if there is one

            if (_typewritterCoroutine != null)
            {
                StopCoroutine(_typewritterCoroutine);
            }
            _typewritterCoroutine = StartCoroutine(Typewritter(_sentence));
        }
    }

    public void NextDialogueNow()
    {
        EndDialogue();
    }

    IEnumerator Typewritter(string _sentence)
    {
        _dialogueText.text = "";
        foreach (char _letter in _sentence.ToCharArray())
        {
            _dialogueText.text += _letter;
            yield return new WaitForSeconds(0.01f);
        }
    }

    private void EndDialogue()
    {
        //Close Dialogue

        //add Dotween animation?

        Debug.Log("DM - EndDialogue");
        _isDialogue = false;

    }
}
