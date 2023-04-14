using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    //singleton
    public static DialogueManager _instance;

    public TextMeshProUGUI _nameText;
    public TextMeshProUGUI _dialogueText;

    //Handle Starting Dialogue
    public bool _isDialogue = false;

    //Queue from sentances
    private Queue<string> _sentences;

    private Coroutine _typewritterCoroutine;

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

        //get sentances from Character (Rai)
        foreach (string _sentance in _dialogue._sentences)
        {
            _sentences.Enqueue(_sentance);
        }

        //display the next sentence in the list
        DisplayNextSentence();

    }

    public void TriggerNextSentence(InputAction.CallbackContext _context)
    {
        if (_context.performed)
        {
            //Debug.Log("DM - context: " + _context.phase);
            DisplayNextSentence();
        }
    }

    public void DisplayNextSentence()
    {
        //if no sentences, end dialogue
        if (_sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        //Have sentances, so Dequeue
        string _sentence = _sentences.Dequeue();

        // Stop the running coroutine, if there is one

        if (_typewritterCoroutine != null)
        {
            StopCoroutine(_typewritterCoroutine);
        }
        _typewritterCoroutine = StartCoroutine(Typewritter(_sentence));
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
