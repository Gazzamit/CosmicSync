using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class OrbitScriptLeft : MonoBehaviour
{
    [Header("REQUIRED")]
    public float _radius = 215.5f;

    [Range(0, 1)]
    public float _startOffsetUnit; //0 to 1
    public GameObject _tapIndicatorPrefab;

    void Start()
    {
        gameObject.SetActive(true);

        float _beatsInLoop = BeatController.instance._beatsInLoop;
        List<float> _leftBeats = TapLeft._leftBeatsStaticVar;

        for (int i = 0; i < _leftBeats.Count; i++)
        {
            // Instantiate the indicator
            GameObject _indicator = Instantiate(_tapIndicatorPrefab, transform.parent);

            float _leftBeatPosition = (_leftBeats[i] - 1) / _beatsInLoop;
            // Get the position of the indicator
            float _angle = (_leftBeatPosition * 360 + _startOffsetUnit * 360) * Mathf.Deg2Rad;
            float x = _radius * Mathf.Cos(_angle);
            float y = _radius * Mathf.Sin(_angle);
            Vector3 _indicatorPosition = new Vector3(x, y, -0.1f);

            // Direction towards the center of circle
            Vector3 _dirToCenter = -_indicatorPosition.normalized;

            // Calculate angle to centre
            float _angleToCenter = Mathf.Atan2(_dirToCenter.y, _dirToCenter.x) * Mathf.Rad2Deg;

            // Set the position / rotation of the position to tap indicator
            _indicator.transform.localPosition = _indicatorPosition;
            _indicator.transform.localRotation = Quaternion.Euler(0, 0, _angleToCenter + 90); // rotate around z-axis
        }
    }

    void FixedUpdate()
    {
        //0 to 1 of position within the current loop
        float _loopPosition = BeatController.instance._loopPlayheadNormalised;

        //set angle to Radians +90 degrees to start from top
        float _angle = (_loopPosition * 360 + _startOffsetUnit * 360) * Mathf.Deg2Rad;
        float x = _radius * Mathf.Cos(_angle);
        float y = _radius * Mathf.Sin(_angle);
        Vector3 _indicatorPosition = new Vector3(x, y, -0.1f);

        // Direction towards the center of circle
        Vector3 _dirToCenter = -_indicatorPosition.normalized;

        // Calculate angle to centre
        float _angleToCenter = Mathf.Atan2(_dirToCenter.y, _dirToCenter.x) * Mathf.Rad2Deg;

        // Set the position / rotation of the position to tap indicator
        transform.localPosition = _indicatorPosition;
        transform.localRotation = Quaternion.Euler(0, 0, _angleToCenter + 90); // rotate around z-axis
    }
}
