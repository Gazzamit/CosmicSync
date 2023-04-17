using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class OnCollidePortal : MonoBehaviour
{
    private GameObject _targetParent; //holds the targets
    // private List<Vector3> _targetPositions;
    // public static List<Vector3> _targetPositionsStaticVar;
    // public static int _nextTargetIndex;
    public static bool _addPortalTurbulanceNow = false;

    //Script is on Spaceship
    // Populate the targetPositions list with the positions of the child objects of the targetParent
    void Start()
    {
        _targetParent = GameObject.FindGameObjectWithTag("TargetHolder");
    }

    // Script is on Spaceship
    //  On Portal collide, expected target? then incremenet nextTargetIndex
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
                Transform _expectedTarget = _targetParent.transform.GetChild(NextTargetIndex._nextTargetIndex);
                //Debug.Log("TCC - Target Transversed: " + _nextTargetIndex);

                // Check if the collided target is the expected target
                if (_collidedTarget == _expectedTarget)
                {
                    // Debug.Log("Adding portal Turbulance");
                    _addPortalTurbulanceNow = true;
                    ScoreManager._instance.AddPoints("portal");

                    // First, Check if all targets have been hit
                    if (ScoreManager._finalTargetDestroyed == true)
                    {
                        // Debug.Log("All targets have been hit!");
                    }
                    else
                    {
                        Debug.Log("OCP - Portal Bool Set Destroyed: " + NextTargetIndex._nextTargetIndex);
                        //Log destroyed in bool and increment target index
                        NextTargetIndex._targetsDestoryedStaticVar[NextTargetIndex._nextTargetIndex] = true;
                        NextTargetIndex._nextTargetIndex++;
                        // Debug.Log("_nextTargetIndex: " + _nextTargetIndex);
                        ScoreManager._instance.TargetDestroyed();
                    }

                    //make hit object inactive
                    _collidedTarget.gameObject.SetActive(false);

                }
            }
        }
    }
}
