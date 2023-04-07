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

        // If loading a new scene to play level
        if (GameManagerDDOL._currentMode == GameManagerDDOL.GameMode.Game)
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
            else //this should not be called
            if (_playerInput.currentActionMap.name == "UIControls")
            {
                Debug.Log("IMS - Switching to Spaceship Contols...");
                ActivateSpaceshipControls();
            }
            _switchInputMaps = false;
        }

        //end of level, activate menu controls
        if (BreakApart._finalTargetDestroyed == true)
        {
            if (_endOfLevel == false)
            {
                _endOfLevel = true;
                //Debug.Log("Start End Of Level Coroutine");
                StartCoroutine(EndOfLevel());
            }
        }
    }

    IEnumerator EndOfLevel()
    {
        //go back to manu
        yield return new WaitForSeconds(15f);
        ActivateUIMenuControls();
    }

    void ActivateUIMenuControls()
    {
        // Switch to the UI action map 
        _playerInput.SwitchCurrentActionMap("UIControls");
        //Debug.Log("In UI Action Map: " + _playerInput.currentActionMap.name);

        //set bools for HUD
        HUDAnimations._switchingHUD = true; //for HUD animations
        //GameManagerDDOL._isGame = false; //for menu HUD animation
        GameManagerDDOL._currentMode = GameManagerDDOL.GameMode.MainMenu;
        //Debug.Log("MainMenu Mode: " + GameManagerDDOL._currentMode);
    }

    void ActivateSpaceshipControls()
    {
        // Switch back to the player action map 
        _playerInput.SwitchCurrentActionMap("SpaceshipControls");
        //Debug.Log("In Spaceship Action Map: " + _playerInput.currentActionMap.name);

        //set bools for HUD
        HUDAnimations._switchingHUD = true; //for HUD animations
        //GameManagerDDOL._isGame = true; //for menu HUD animation
        GameManagerDDOL._currentMode = GameManagerDDOL.GameMode.Game;

    }

}
