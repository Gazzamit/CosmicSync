using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class PitchYawSlider : MonoBehaviour
{
    private Slider _slider;

    private Vector2 _navInput;

    private void Start()
    {
        // Get  lider componen
        _slider = GetComponent<Slider>();
        _slider.value = GameManagerDDOL._pitchYawSliderValue;
    }

    private void Update()
    {

    }

    public void onNav(InputAction.CallbackContext _context)
    {
        _navInput = _context.ReadValue<Vector2>();
        //Debug.Log("PYS - Nav (A, D) Pressed");

        //if in Settings get slider value
        if (GameManagerDDOL._currentMode == GameManagerDDOL.GameMode.SettingsMenu)
        {
            if (_context.performed)
            {
                float _newValue = GameManagerDDOL._pitchYawSliderValue; ;
                // Get the new value
                if (_navInput.x == 1 && _newValue <= _slider.maxValue)
                {
                    //PC needs very small values. Mac seems to need bigger.
                    if (_newValue <= 2f)
                    {
                        _newValue += 0.1f;
                    }
                    else
                    {
                        _newValue += 1f;
                    }
                    // Debug.Log("Slider value: " + _newValue);
                }
                if (_navInput.x == -1 && _newValue >= _slider.minValue)
                {
                    if (_newValue <= 2f)
                    {
                        _newValue -= 0.1f;
                    }
                    else
                    {
                        _newValue -= 1f;
                    }
                    // Debug.Log("Slider value: " + _newValue);
                }
                _slider.value = _newValue;
                GameManagerDDOL._pitchYawSliderValue = _newValue;
                //Debug.Log("PtichYaw Value: " + _newValue);

            }
        }
    }
}


