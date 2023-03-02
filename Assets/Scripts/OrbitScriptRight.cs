using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class OrbitScriptRight : MonoBehaviour
{
    [Header("REQUIRED")]
    public float _radius = 3.0f;
    [Range(0, 1)]
    public float _startOffsetUnit; //0 to 1
    public GameObject _TapIndicatorPrefab;

    void Start()
    {
        float _beatsInLoop = Controller.instance._beatsInLoop;
        List<float> _rightBeats = TapRight._rightBeatsStaticVar;

        for (int i = 0; i < _rightBeats.Count; i++)
        {
            // Instantiate the indicator
            GameObject _indicator = Instantiate(_TapIndicatorPrefab, transform.parent);

            float _rightBeatPosition = (_rightBeats[i] - 1) / _beatsInLoop;
            // Get the position of the quarter indicator
            float _angle = (_rightBeatPosition * 360 + _startOffsetUnit * 360) * Mathf.Deg2Rad;
            float x = _radius * Mathf.Cos(_angle);
            float y = _radius * Mathf.Sin(_angle);
            Vector3 _indicatorPosition = new Vector3(x, y, -0.1f);

            // Set the position / rotation of the position to tap indicator
            _indicator.transform.localPosition = _indicatorPosition;
            _indicator.transform.localRotation = Quaternion.Euler(0, 0, _angle * Mathf.Rad2Deg + 90); // rotate around z-axis
        }
    }

    void Update()
    {
        //0 to 1 of position within the current loop
        float _loopPosition = Controller.instance._loopPlayheadNormalised;

        //set angle to Radians +90 degrees to start from top
        float _angle = (_loopPosition * 360 + _startOffsetUnit * 360) * Mathf.Deg2Rad;

        //loopPosition is already adjusted for offset by adding bars until its positive in Contoller
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, 360, _loopPosition));

        float x = _radius * Mathf.Cos(_angle);
        float y = _radius * Mathf.Sin(_angle);
        transform.localPosition = new Vector3(x, y, -0.1f);

    }

}
