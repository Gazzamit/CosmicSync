using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrottleAnimation : MonoBehaviour
{
    //thrust spaceship control
    public Transform _throttle;

    void Start()
    {
        _throttle = gameObject.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        //magnitude to control thrust control local angle
        float _angle = SpaceshipControls._magnitude;
        Quaternion rotation = Quaternion.Euler(_angle, 0, 0);
        _throttle.transform.localRotation = rotation;
    }
}
