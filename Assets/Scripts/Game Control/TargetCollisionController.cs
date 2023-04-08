using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class TargetCollisionController : MonoBehaviour
{
    private GameObject _targetParent; //holds the targets
    private List<Vector3> _targetPositions;
    public static List<Vector3> _targetPositionsStaticVar;
    public static int _nextTargetIndex;
    public static bool _addPortalTurbulanceNow = false;

    //Script is on Spaceship
    // Populate the targetPositions list with the positions of the child objects of the targetParent
    void Start()
    {
        //Physics.IgnoreLayerCollision(_spaceshipLayer, _targetLeyer, true);

        _targetParent = GameObject.FindGameObjectWithTag("TargetHolder");
        _targetPositions = new List<Vector3>();

        foreach (Transform child in _targetParent.transform)
        {
            _targetPositions.Add(child.position);
        }

        _targetPositionsStaticVar = _targetPositions;

        _nextTargetIndex = 0;

    }

    void Update()
    {
        // Get position of last element - spaceship / final target
        Vector3 _finalObjectPosition = _targetPositions[_targetPositions.Count - 1];

        // Check if position has changed
        if (_finalObjectPosition != _targetParent.transform.GetChild(_targetPositions.Count - 1).position)
        {
            // Update the position in list (last element)
            _targetPositions[_targetPositions.Count - 1] = _targetParent.transform.GetChild(_targetPositions.Count - 1).position;
            //Debug.Log("Updated last target position: ");
        }

    }

    // Check if the collided object is the expected target and update the nextTargetIndex accordingly
    void OnTriggerEnter(Collider _collision)
    {
        //check that not triggered by laser, but by spaceship/portal collision
        if (_collision.gameObject.CompareTag("Target") && PortalTrigger._spaceshipInPortal)
        {
            //reset bool ready for next portal
            PortalTrigger._spaceshipInPortal = false;

            //Debug.Log("Collider. Tag: Target");
            Transform _collidedTarget = _collision.transform;

            // Check if the collided target is a child of the targetParent
            if (_collidedTarget.parent == _targetParent.transform)
            {
                //Object set inactive if hit. 
                Transform _expectedTarget = _targetParent.transform.GetChild(_nextTargetIndex);

                // Check if the collided target is the expected target
                if (_collidedTarget == _expectedTarget)
                {
                    //int _currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
                    //if (_currentSceneIndex > 0)
                    //{
                    // Debug.Log("Adding portal Turbulance");
                    _addPortalTurbulanceNow = true;
                    //}
                    // First, Check if all targets have been hit
                    if (BreakApart._finalTargetDestroyed == true)
                    {
                        // Debug.Log("All targets have been hit!");
                    }
                    else
                    {
                        _nextTargetIndex++;
                        // Debug.Log("_nextTargetIndex: " + _nextTargetIndex);
                    }

                    //make hit object inactive
                    _collidedTarget.gameObject.SetActive(false);

                    //Debug.Log("Correct Target. Remaining Targets: " + _targetParent.transform.childCount);
                }
                else
                {
                    //Debug.Log("Wrong target");
                    //Debug.Log("Expected Target:  " +  _expectedTarget);
                    //Debug.Log("Collided Target:  " +  _collidedTarget);

                }
            }
        }
    }
}
