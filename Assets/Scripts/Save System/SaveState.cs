using UnityEngine;

[System.Serializable]
public class SaveState 
{
    public int levelUnlocked = 0;
    public int completedLevel;

    public int[] starsPerLevel = new int[4];
    
    public int[] highScoresSaved = new int[5];
    public string[] highScoreNameSaved = new string [5];

}
