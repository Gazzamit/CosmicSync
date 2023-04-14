using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//simple C# class
[System.Serializable]
public class Dialogue
{
    //Name and sentances queu will be added in the inspector
    
    //Character (e.g. Rhythm AI Rai)
    public string _name;

    //sentances for queue array 
    [TextArea (3, 10)]
    public string[] _sentences;
}
