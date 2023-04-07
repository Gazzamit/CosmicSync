using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class AVSynchronisation : MonoBehaviour
{
    private Slider _slider;


    private float _OP_Input;

    private void Start()
    {
        // Get  slider component
        _slider = GetComponent<Slider>();
        _slider.value = GameManagerDDOL._timingSliderValue;
    }

    private void Update()
    {

    }

    public void onOP_Key(InputAction.CallbackContext _context)
    {
        _OP_Input = _context.ReadValue<float>();
        //Debug.Log("AVS - OP Pressed");

        //if in Settings get slider value
        if( GameManagerDDOL._currentMode == GameManagerDDOL.GameMode.SettingsMenu)
        {
            if (_context.performed)
            {
                float _newValue = GameManagerDDOL._timingSliderValue;
                // P pressed
                if (_OP_Input == 1 && _newValue <= _slider.maxValue)
                {
                    _newValue += 0.01f;
                    // Debug.Log("Slider value: " + _newValue);
                }
                // O pressed
                if (_OP_Input == -1 && _newValue >= _slider.minValue)
                {
                    _newValue -= 0.01f;
                    // Debug.Log("Slider value: " + _newValue);
                }
                _slider.value = _newValue;
                GameManagerDDOL._timingSliderValue = _newValue;
            }
        }
    }
}


