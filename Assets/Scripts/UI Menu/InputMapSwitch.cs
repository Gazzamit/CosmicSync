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

    public static bool _switchInputMaps = false, _switchHUD = false, _isGame = true;

    void Awake()
    {
        _playerInput = GameObject.Find("Spaceship").GetComponent<PlayerInput>();
        _mainMenu = GameObject.Find("MainMenu");
        _spaceshipHolder = GameObject.Find("SpaceShipHolder");
        _targetHolder = GameObject.Find("TargetHolder");
        _mainInGameUI = GameObject.Find("MainInGameUI");
        _targetingSVG = GameObject.Find("TargetingSVG");
    }

    void Update()
    {
        //this is activated from spaceships controls
        if (_switchInputMaps == true)
        {
            if (_playerInput.currentActionMap.name == "UIControls") ActivateSpaceshipControls();
            else if (_playerInput.currentActionMap.name == "SpaceshipControls") ActivateUIControls();
            _switchInputMaps = false;
            //Debug.Log("Switching Input Maps");
        }
    }
    
    void ActivateUIControls()
    {
        // Switch to the UI action map when the game object is enabled
        _playerInput.SwitchCurrentActionMap("UIControls");
        Debug.Log("UI Action Map: " + _playerInput.currentActionMap.name);

        //set bools for HUD
        _switchHUD = true;
        _isGame = false;

        StartCoroutine(ActivateMenuObjects());
    }

    IEnumerator ActivateMenuObjects()
    {   
        yield return new WaitForSeconds(0.3f);
        //activate MenuObjects, disable gameObjects
        _mainMenu.transform.GetChild(0).gameObject.SetActive(true);
        //_spaceshipHolder.SetActive(false);
        _targetHolder.SetActive(false);
        _mainInGameUI.transform.GetChild(0).gameObject.SetActive(false);
        _targetingSVG.SetActive(false);
    }

    void ActivateSpaceshipControls()
    {
        // Switch back to the player action map when the game object is disabled
        _playerInput.SwitchCurrentActionMap("SpaceshipControls");  
        Debug.Log("Spaceship Action Map: " + _playerInput.currentActionMap.name);

        //set bools for HUD
        _switchHUD = true;
        _isGame = true;

        StartCoroutine(ActivateGameObjects());

    }

    IEnumerator ActivateGameObjects()
    {
        yield return new WaitForSeconds(0.3f);
        //activate MenuObjects, disable gameObjects
        _mainMenu.transform.GetChild(0).gameObject.SetActive(false);
        //_spaceshipHolder.SetActive(true);
        _targetHolder.SetActive(true);
        _mainInGameUI.transform.GetChild(0).gameObject.SetActive(true);
        _targetingSVG.SetActive(true);
    }
}
