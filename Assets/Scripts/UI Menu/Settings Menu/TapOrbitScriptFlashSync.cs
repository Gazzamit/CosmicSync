using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using Unity.VectorGraphics;

public class TapOrbitScriptFlashSync : MonoBehaviour
{
    [Header("REQUIRED")]
    public float _radius = 215.5f;
    public GameObject _tapIndicatorPrefab;
    [SerializeField] private Color[] _colours; //normal, flash

    [SerializeField] private SVGImage _blueCircle;

    [SerializeField] private int _currentBeatPosition, _lastBeatPosition;

    void FixedUpdate()
    {
        // Calculate the beat and loop position from the BeatController
        //offset by -0.01 for slightly earlier 0.1s flash
        _currentBeatPosition = Mathf.FloorToInt(BeatController.instance._playheadInBeats - 0.01f);

        // On increment flash the circle
        if (_currentBeatPosition != _lastBeatPosition)
        {
            // Flash the image
            StartCoroutine(FlashIndicator());

            _lastBeatPosition = _currentBeatPosition;
        }
    }

    IEnumerator FlashIndicator()
    {
        //_yellowIndicator.color = _colours[1];
        _blueCircle.color = _colours[1];

        yield return new WaitForSeconds(0.1f);

        //_yellowIndicator.color = _colours[0];
        _blueCircle.color = _colours[0];
        //_isFlashingIndicator = false;
    }

    //on Cancel, retrun to Menu
    public void onReturnToMenu(InputAction.CallbackContext _context)
    {
        //if in Settings
        if (GameManagerDDOL._currentMode == GameManagerDDOL.GameMode.SettingsMenu)
        {
            if (_context.performed)
            {
                HUDAnimations._switchingHUD = true; //for HUD animations
                GameManagerDDOL._currentMode = GameManagerDDOL.GameMode.MainMenu;
            }
        }
    }



    //on Q pressed, Quit Game
    public void onQ(InputAction.CallbackContext _context)
    {
        //if in Settings
        if (GameManagerDDOL._currentMode == GameManagerDDOL.GameMode.SettingsMenu)
        {
            if (_context.performed)
            {
                Debug.Log("Quitting Game");
                #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                #else
                    Application.Quit();
                #endif
            }
        }

    }
}
