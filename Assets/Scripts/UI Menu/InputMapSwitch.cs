using UnityEngine;
using System;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class InputMapSwitch : MonoBehaviour
{

    public PlayerInput _playerInput; // Reference to the PlayerInput

    private GameObject _mainMenu, _spaceshipHolder, _targetHolder, _mainInGameUI, _targetingSVG;

    public static bool _switchInputMaps = false, _switchingHUD = false, _isGame = true;

    void Awake()
    {
        _playerInput = GameObject.Find("Spaceship").GetComponent<PlayerInput>();
        _mainMenu = GameObject.Find("MainMenu");
        _spaceshipHolder = GameObject.Find("SpaceShipHolder");
        _targetHolder = GameObject.Find("TargetHolder");
        _mainInGameUI = GameObject.Find("MainInGameUI");
        _targetingSVG = GameObject.Find("TargetingSVG");
        
        // Load only spaceship controls at the start of the game
        ActivateSpaceshipControls();
    }

    void Update()
    {
        //this is activated from spaceships controls
        if (_switchInputMaps == true)
        {
            if (_playerInput.currentActionMap.name == "UIControls")
            {
                // Debug.Log("Switching to Spaceship Contols...");
                ActivateSpaceshipControls();
            }
            else if (_playerInput.currentActionMap.name == "SpaceshipControls")
            {
                // Debug.Log("Switching to UI Menu");
                ActivateUIMenuControls();
            }
            _switchInputMaps = false;
        }
    }

    void ActivateUIMenuControls()
    {
        // Switch to the UI action map 
        _playerInput.SwitchCurrentActionMap("UIControls");
        // Debug.Log("UI Action Map: " + _playerInput.currentActionMap.name);

        //set bools for HUD
        _switchingHUD = true; //for HUD animations
        _isGame = false; //for menu HUD animation

        StartCoroutine(ActivateMenuObjects());
    }

    IEnumerator ActivateMenuObjects()
    {
        //activate Menu Objects then move them
        _mainMenu.transform.GetChild(0).gameObject.SetActive(true);

        //move objects, then disable them
        yield return new WaitForSeconds(0.3f);
        //_spaceshipHolder.SetActive(false);
        _targetHolder.SetActive(false);
        //_mainInGameUI.transform.GetChild(0).gameObject.SetActive(false);
        //_targetingSVG.SetActive(false);


    }

    void ActivateSpaceshipControls()
    {
        // Switch back to the player action map 
        _playerInput.SwitchCurrentActionMap("SpaceshipControls");
        // Debug.Log("Spaceship Action Map: " + _playerInput.currentActionMap.name);

        //set bools for HUD
        _switchingHUD = true; //for HUD animations
        _isGame = true; //for menu HUD animation

        StartCoroutine(ActivateGameObjects());

    }

    IEnumerator ActivateGameObjects()
    {
        //activate Game Objects then move them
        //_spaceshipHolder.SetActive(true);
        _targetHolder.SetActive(true);
        _mainInGameUI.transform.GetChild(0).gameObject.SetActive(true);
        _targetingSVG.SetActive(true);

        //move objects, then disable them
        yield return new WaitForSeconds(0.3f);
        //_mainMenu.transform.GetChild(0).gameObject.SetActive(false);

    }
}
