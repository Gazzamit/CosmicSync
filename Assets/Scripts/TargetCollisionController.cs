using UnityEngine;
using System.Collections.Generic;

public class TargetCollisionController : MonoBehaviour
{
    public GameObject targetParent; //holds the targets
    private List<Vector3> _targetPositions;
    public static List<Vector3> _targetPositionsStaticVar;
    public static int _nextTargetIndex;

    // Populate the targetPositions list with the positions of the child objects of the targetParent
    void Start()
    {
        _targetPositions = new List<Vector3>();

        foreach (Transform child in targetParent.transform)
        {
            _targetPositions.Add(child.position);
        }

        _targetPositionsStaticVar = _targetPositions;

        _nextTargetIndex = 0;

    }

    // Check if the collided object is the expected target and update the nextTargetIndex accordingly
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Target"))
        {
            Transform collidedTarget = collision.transform;

            // Check if the collided target is a child of the targetParent
            if (collidedTarget.parent == targetParent.transform)
            {
                //Object set inactive if hit. 
                Transform expectedTarget = targetParent.transform.GetChild(_nextTargetIndex);

                // Check if the collided target is the expected target
                if (collidedTarget == expectedTarget)
                {

                    // First, Check if all targets have been hit ( ==1 as last target)
                    if ( targetParent.transform.childCount == 1)
                    {
                        Debug.Log("All targets have been hit!");
                    }
                    else
                    {
                        _nextTargetIndex++;
                    }

                    //make hit object inactive
                    collidedTarget.gameObject.SetActive(false);

                    //Debug.Log("Correct Target. Remaining Targets: " + targetParent.transform.childCount);
                }
                else
                {
                    //Debug.Log("Wrong target");
                    //Debug.Log("Expected Target:  " +  expectedTarget);
                    //Debug.Log("Collided Target:  " +  collidedTarget);

                }
            }
        }
    }
}
