using UnityEngine;
using System.Collections;
using Unity.VectorGraphics;
using System;

public class TargetRadar : MonoBehaviour
{
    public Transform _shipTransform; //for calculating if target is behind ship
    public Vector3 _targetPosition; // The target to track
    public float _radius = 300.0f; // The radius of the radar
    private float _flashInterval = 0.5f;
    public Color _flashColourSlow, _flashColourFast; // When target is off screen
    private Color _flashColourSelect;

    // Private variables
    private Camera _cam;
    private RectTransform _rectTransformUIIndicator; // UI indicator
    private RectTransform _parentRectTransform; // UI parent object

    private Vector2 _indicatorPos; //UI indicator position

    private bool _isFlashing = false;
    private Coroutine _flashCoroutine = null;

    private Color _originalColour; // Store original color of the indicator

    private int _debugIndexCurrent, _debugIndexPrevious = -1;

    private void Awake()
    {
        // Get references to the camera and RectTransforms
        _cam = Camera.main;
        _rectTransformUIIndicator = GetComponent<RectTransform>();
        _parentRectTransform = transform.parent.GetComponent<RectTransform>();

        // Store the color of the indicator and original colour
        _originalColour = _rectTransformUIIndicator.GetComponent<SVGImage>().color;
    }


    private void Update()
    {
        if (ScoreManager._finalTargetDestroyed == false)
        {
            //Get the next target from collision coltroller script list
            _targetPosition = NextTargetIndex._targetPositionsStaticVar[NextTargetIndex._nextTargetIndex];

            _debugIndexCurrent = NextTargetIndex._nextTargetIndex;
            if (_debugIndexCurrent != _debugIndexPrevious)
            {
                Debug.Log("TR - Target index: " + _debugIndexCurrent + " . Position: " + _targetPosition);
                _debugIndexPrevious = _debugIndexCurrent;
            }
            
            // Convert target position as viewport space
            Vector3 _targetPos = _cam.WorldToViewportPoint(_targetPosition);
            Vector2 _indicatorPos = Vector2.zero;

            // Check if the target is inside target square, snap to zero
            if (_targetPos.z > 0.3f && _targetPos.x > 0.4f && _targetPos.x < 0.6f && _targetPos.y > 0.35f && _targetPos.y < 0.65f)
            {
                // Target is inside target square, zero UI indicator and stop flashing
                _rectTransformUIIndicator.anchoredPosition = new Vector2(0, 0); // Move the UI indicator to center
                StopFlashing();
            }
            else
            {
                // Target is outside target square, flash UI indicator
                Vector3 _screenPos = _cam.WorldToScreenPoint(_targetPosition);
                Vector2 _viewportPos = _cam.ScreenToViewportPoint(_screenPos);

                // Direction from  center to the target
                Vector2 _direction = (_viewportPos - new Vector2(0.5f, 0.5f)).normalized;

                // Angle between the direction and the x-axis
                float _angle = Mathf.Atan2(_direction.y, _direction.x);

                // Calculate the position of the UI indicator relative to the center of the radar
                float x = Mathf.Cos(_angle) * _radius;
                float y = Mathf.Sin(_angle) * _radius;

                // indicatorPos = new Vector2(x, y);

                // Clamp UI to target square via parent rectTransform
                float _aspectRatio = Screen.width / (float)Screen.height;
                float _maxY = _parentRectTransform.rect.height * 0.5f - _rectTransformUIIndicator.rect.height * 0.5f;
                float _maxX = _parentRectTransform.rect.width * 0.5f - _rectTransformUIIndicator.rect.width * 0.5f;


                //Debug.Log("Max(x,y): ()" + _maxX.ToString("F1") + "," + _maxY.ToString("F1") + ")");

                // Limit indicator to the size of the square (take sign and apply to max)
                if (Mathf.Abs(x) > _maxX)
                {
                    x = Mathf.Sign(x) * _maxX;
                }
                if (Mathf.Abs(y) > _maxY)
                {
                    y = Mathf.Sign(y) * _maxY;
                }

                //check if target is BEHIND the ship. Direction is ship to target
                Vector3 _directionShipToTarget = (_targetPosition - _shipTransform.position).normalized;
                //dot produc of directio to target and ship forward
                float _dot = Vector3.Dot(_directionShipToTarget, _shipTransform.forward);
                //negative dot product means target is behind the ship
                //Debug.Log("Dot(x,y): " + _dot.ToString("F1") + "(" + x.ToString("F1") + "," + y.ToString("F1") + ")");

                if (_dot < 0)
                {
                    //invert x, y if target on other side of camera o indicator correctly positioned
                    x *= -1f;
                    y *= -1f;
                    //set fast flash Rate
                    _flashInterval = 0.1f;
                    _flashColourSelect = _flashColourFast;
                }
                else
                {
                    //set slow flash rate
                    _flashInterval = 0.3f;
                    _flashColourSelect = _flashColourSlow;
                }

                // Start flashing if not already flashing
                if (!_isFlashing)
                {
                    // Start the flashing coroutine and store a reference to it
                    _flashCoroutine = StartCoroutine(FlashCoroutine());
                }

                _indicatorPos = new Vector2(x, y);

            }

            // Set the position of the UI indicator
            _rectTransformUIIndicator.anchoredPosition = _indicatorPos;
        }
        else
        {
            //all targets destroyed - zero position
            //_rectTransformUIIndicator.anchoredPosition = new Vector2(0f,0f);
            //Set inactive
            _rectTransformUIIndicator.gameObject.SetActive(false);
        }

    }

    // Flash UI Indicator - target outside square target
    private IEnumerator FlashCoroutine()
    {
        _isFlashing = true;
        while (true)
        {
            _rectTransformUIIndicator.GetComponent<SVGImage>().color = _flashColourSelect;
            yield return new WaitForSeconds(_flashInterval);
            _rectTransformUIIndicator.GetComponent<SVGImage>().color = _originalColour;
            yield return new WaitForSeconds(_flashInterval);
        }
    }


    // Stop flashing UI indicaor
    private void StopFlashing()
    {
        if (_isFlashing)
        {
            StopCoroutine(_flashCoroutine);
            _isFlashing = false;
            // Reset the color UI
            _rectTransformUIIndicator.GetComponent<SVGImage>().color = _originalColour;
        }
    }

}
