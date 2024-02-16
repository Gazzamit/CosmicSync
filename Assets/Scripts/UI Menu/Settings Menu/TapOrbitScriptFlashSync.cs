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

    [SerializeField]
    private Color[] _colours; //normal, flash

    [SerializeField]
    private SVGImage _blueCircle;

    [SerializeField]
    private int _currentBeatPosition,
        _lastBeatPosition;

    [SerializeField]
    private float _flashOffset = -0.02f;

    void FixedUpdate()
    {
        // Calculate the beat and loop position from the BeatController
        //offset by -0.01 for slightly earlier 0.1s flash
        _currentBeatPosition = Mathf.FloorToInt(
            BeatController.instance._playheadInBeats + _flashOffset
        );

        // On increment flash the circle
        if (_currentBeatPosition != _lastBeatPosition)
        {
            // Flash the image
            _blueCircle.color = _colours[1];
            StartCoroutine(ResetColour());

            _lastBeatPosition = _currentBeatPosition;
        }
    }

    IEnumerator ResetColour()
    {
        yield return new WaitForSeconds(0.1f);
        _blueCircle.color = _colours[0];
    }

    //on ESC Cancel, retrun to Menu
    public void onReturnToMenu(InputAction.CallbackContext _context)
    {
        //if in Settings
        if (GameManagerDDOL._currentMode == GameManagerDDOL.GameMode.SettingsMenu)
        {
            //as settings/menu active, transform used to check which input esc to use
            if (_context.performed && gameObject.transform.position.y == 0)
            {
                Debug.Log("TOSFS - ESC Pressed from Settings Menu");
                HUDAnimations._switchingHUD = true; //for HUD animations
                GameManagerDDOL._currentMode = GameManagerDDOL.GameMode.MainMenu;

                //reset which dialogue shown to user in welcome
                WelcomeDialogue._triggerRestartMenuLoop = true;
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
                Debug.Log("TOSFS - Quitting Game");
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }
    }
}
