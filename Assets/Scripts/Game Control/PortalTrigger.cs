using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    //for target collision control
    //Script is on portals only
    public static bool _spaceshipInPortal = false;//, _triggerWhiteFlash = false;


    void OnTriggerEnter(Collider _collision)
    {
        if (_collision.gameObject.CompareTag("Spaceship"))
        {
            _spaceshipInPortal = true;
        }
    }
}