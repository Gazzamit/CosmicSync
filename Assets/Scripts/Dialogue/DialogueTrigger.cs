using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    //Add to game object to create dialogue
    public Dialogue _dialogue;


    public void TriggerDialogue()
    {
        //pass triggered dialogue to Manager
        DialogueManager._instance.StartDialogue(_dialogue);
    }
}
