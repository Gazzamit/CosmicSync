using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class OrbitScriptRight : MonoBehaviour
{
    [Header("REQUIRED")]
    public float radius = 3.0f;
    [Range(0, 1)]
    public float StartOffsetUnit; //0 to 1
    public GameObject TapIndicatorPrefab;

    void Start()
    {
        float beatsInLoop = Controller.instance.beatsInLoop;
        List<float> rightBeats = TapRight.rightBeatsStaticVar;

        for (int i = 0; i < rightBeats.Count; i++)
        {
            // Instantiate the indicator
            GameObject indicator = Instantiate(TapIndicatorPrefab, transform.parent);

            float rightBeatPosition = (rightBeats[i] - 1) / beatsInLoop;
            // Get the position of the quarter indicator
            float angle = (rightBeatPosition * 360 + StartOffsetUnit * 360) * Mathf.Deg2Rad;
            float x = radius * Mathf.Cos(angle);
            float y = radius * Mathf.Sin(angle);
            Vector3 indicatorPosition = new Vector3(x, y, -0.1f);

            // Set the position / rotation of the position to tap indicator
            indicator.transform.localPosition = indicatorPosition;
            indicator.transform.localRotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg + 90); // rotate around z-axis
        }
    }

    void Update()
    {
        //0 to 1 of position within the current loop
        float loopPosition = Controller.instance.loopPlayheadNormalised;

        //set angle to Radians +90 degrees to start from top
        float angle = (loopPosition * 360 + StartOffsetUnit * 360) * Mathf.Deg2Rad;

        //loopPosition is already adjusted for offset by adding bars until its positive in Contoller
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, 360, loopPosition));

        float x = radius * Mathf.Cos(angle);
        float y = radius * Mathf.Sin(angle);
        transform.localPosition = new Vector3(x, y, -0.1f);

    }

}
