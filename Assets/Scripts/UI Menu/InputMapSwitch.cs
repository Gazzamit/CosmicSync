using UnityEngine;
using System;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class InputMapSwitch : MonoBehaviour
{

    public PlayerInput _playerInput; // Reference to the PlayerInput

    public static bool _switchInputMaps = false;

    private bool _endOfLevel;

    void Awake()
    {
        _endOfLevel = false;

        //if doWelcome active welcome playercontrols (pressI)
        if (GameManagerDDOL._doWelcome == true)
        {
            Debug.Log("IMS - Awake - Activate doWelcome PressI Controls");
            ActivateWelcomeControls();
        }
        // If loading a new scene to play level
        else if (GameManagerDDOL._currentMode == GameManagerDDOL.GameMode.Game)
        {
            Debug.Log("IMS - Awake - Activate Spaceship Controls");
            ActivateSpaceshipControls();
        }
        //if loading game for first time
        else
        {
            Debug.Log("IMS - Awake - Activate Menu Controls");
            ActivateUIMenuControls();
        }
    }

    void Update()
    {
        //Switch Input Maps for Game or Menu
        //From game switch to Menu map (for Menu and settings UI)
        //Switch to UI is called from spaceships controls
        //switch to spaceship controls happens when new scenen in awake
        if (_switchInputMaps == true)
        {
            if (_playerInput.currentActionMap.name == "SpaceshipControls")
            {
                Debug.Log("IMS - Switching to UI Menu...");
                ActivateUIMenuControls();
            }
            else if (_playerInput.currentActionMap.name == "PressI")
            {
                Debug.Log("IMS - Switching to UI Menu...");
                ActivateUIMenuControls();
            }
            //this should not be called as a load scene occura for each level
            else if (_playerInput.currentActionMap.name == "UIControls")
            {
                Debug.Log("IMS - Switching to Spaceship Contols...");
                ActivateSpaceshipControls();
            }
            _switchInputMaps = false;
        }

        //end of level, activate menu controls
        if (ScoreManager._finalTargetDestroyed == true)
        {
            if (GameManagerDDOL._doWelcome == false)
            {
                if (_endOfLevel == false)
                {
                    _endOfLevel = true;
                    //Debug.Log("Start End Of Level Coroutine");
                    StartCoroutine(EndOfLevel()); //activate menu controls
                }
            }
        }
    }

    private void CheckWhichMapsAreActive()
    {
        // get reference 
        InputActionMap _pressIActionMap = _playerInput.actions.FindActionMap("PressI");
        InputActionMap _UIControlsActionMap = _playerInput.actions.FindActionMap("UIControls");
        InputActionMap _spaceshipControlsActionMap = _playerInput.actions.FindActionMap("SpaceshipControls");

        // check if active
        if (_playerInput.currentActionMap == _pressIActionMap)
        {
            Debug.Log("WD - Action Map Check for pressI: " + _playerInput.currentActionMap.name);
        }
        if (_playerInput.currentActionMap == _UIControlsActionMap)
        {
            Debug.Log("WD - Action Map Check for UI: " + _playerInput.currentActionMap.name);
        }
        if (_playerInput.currentActionMap == _spaceshipControlsActionMap)
        {
            Debug.Log("WD - Action Map Check for Spaceship Conotrols: " + _playerInput.currentActionMap.name);
        }
    }

    IEnumerator EndOfLevel()
    {
        //go back to manu
        yield return new WaitForSeconds(10f);
        ActivateUIMenuControls();
    }

    void ActivateUIMenuControls()
    {
        // Switch to the UI action map 
        SwitchActionMap("UIControls");
        Debug.Log("IMS - To UI Action Map: " + _playerInput.currentActionMap.name);
        CheckWhichMapsAreActive();

        //set bools for HUD
        HUDAnimations._switchingHUD = true; //for HUD animations
        Debug.Log("IMS - Game Mode is MainMenu");
        GameManagerDDOL._currentMode = GameManagerDDOL.GameMode.MainMenu;
        //Debug.Log("MainMenu Mode: " + GameManagerDDOL._currentMode);
    }

    void ActivateSpaceshipControls()
    {
        // Switch back to the player action map 
        SwitchActionMap("SpaceshipControls");
        Debug.Log("IMS - To Spaceship Action Map: " + _playerInput.currentActionMap.name);
        CheckWhichMapsAreActive();

        //set bools for HUD
        HUDAnimations._switchingHUD = true; //for HUD animations
        Debug.Log("IMS - Game Mode is Game");
        GameManagerDDOL._currentMode = GameManagerDDOL.GameMode.Game;

    }

    void ActivateWelcomeControls()
    {
        SwitchActionMap("PressI");
        Debug.Log("IMS - To PressI Action Map: " + _playerInput.currentActionMap.name);
        CheckWhichMapsAreActive();
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


