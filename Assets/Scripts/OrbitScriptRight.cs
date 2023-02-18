using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class OrbitScriptRight : MonoBehaviour
{
    [Header("REQUIRED")]
    public float radius = 3.0f;

    void Update()
    {
        //0 to 1 of position within the current loop
        float loopPosition = Controller.instance.loopPlayheadNormalised;

        //set angle to Radians +90 degrees to start from top
        float angle = (loopPosition * 360 + 270) * Mathf.Deg2Rad;

        //loopPosition is already adjusted for offset by adding bars until its positive in Contoller
        transform.rotation = Quaternion.Euler(0,0,Mathf.Lerp(0, 360, loopPosition));

        float x = radius * Mathf.Cos(angle);
        float y = radius * Mathf.Sin(angle);
        transform.localPosition = new Vector3(x, y,-0.1f);

    }

}
   