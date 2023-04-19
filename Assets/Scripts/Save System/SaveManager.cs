using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public bool RESET_SAVE = false;

    public static SaveManager _instance; //access from anywhere in the
    public SaveState state; //this will be converted to/from a string

    private void Awake()
    {
        //TEMP for reset state

        if (RESET_SAVE)
        {
            ResetSave();
            RESET_SAVE = false;
            Debug.Log("--------------");
            Debug.Log("SAVE WAS RESET");
            Debug.Log("--------------");
        }
        else
        {
            Debug.Log ("Save not reset");
        }
         if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        Load();

        //test state
        //Debug.Log(IsSkinOwned(0));
        //UnlockSkin(0);
        //Debug.Log(IsSkinOwned(0));

        //to save/load data
        //SaveManager.Instance.Save()
        //SaveManager.Instance.state.yourVariableToSave = x; 
        //SaveManager.Instance.Load()
        //x = SaveManager.Instance.state.yourVariableToLoad

    }

    // Save state to saveState script to player prefs via serialize
    public void Save()
    {
        PlayerPrefs.SetString("save", Helper.Serialize<SaveState>(state)); // convert state into a string
    }

    //load state from saveState script from playerprefs via deserialize
    public void Load()
    {
        //have already a saved state?
        if (PlayerPrefs.HasKey("save"))
        {
            Debug.Log("Loading Player Prefs");
            state = Helper.Deserialize<SaveState>(PlayerPrefs.GetString("save")); // convert string to state
        }
        else //first save
        {
            state = new SaveState();
            Save();
            Debug.Log("First Save");
        }
    }


    //Complete the level
    public void CompleteLevel(int index)
    {
        //if this level is the current active level
        //Debug.Log("SM - Save: state.completedLevel: " + state.completedLevel + " index: " + index);
        if (state.completedLevel == index)
        {
            state.completedLevel++;
            Debug.Log("Increment Level Completed and Save. Completed Level: " + state.completedLevel);
            Save();
        }
    }

    //reset save file
    public void ResetSave()
    {
        PlayerPrefs.DeleteKey("save");
    }
}

