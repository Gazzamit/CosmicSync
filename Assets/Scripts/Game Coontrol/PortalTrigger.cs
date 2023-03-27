using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTrigger : MonoBehaviour
{

    public static bool _spaceshipInPortal = false;


    void OnTriggerEnter(Collider _collision)
    {
        if (_collision.gameObject.CompareTag("Spaceship"))
        {
            _spaceshipInPortal = true;
        }
    }
}