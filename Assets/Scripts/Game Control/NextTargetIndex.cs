using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextTargetIndex : MonoBehaviour
{
    private GameObject _targetParent; //holds the targets
    public static List<Vector3> _targetPositionsStaticVar;

    public static bool[] _targetsDestoryedStaticVar;

    public bool[] _tempTargetsDestroyed;
    public static int _nextTargetIndex;
    private int _counter;

    // Track next Target position
    // Populate the targetPositions list with the positions of the child objects of the targetParent
    void Start()
    {
        // Get a list of all targets (child object of targetholder)
        _targetParent = GameObject.FindGameObjectWithTag("TargetHolder");
        _targetPositionsStaticVar = new List<Vector3>();
        _targetsDestoryedStaticVar = new bool[_targetParent.transform.childCount];

        foreach (Transform child in _targetParent.transform)
        {
            _targetPositionsStaticVar.Add(child.position);
            _targetsDestoryedStaticVar[_counter] = false;
            Debug.Log("NT - Child position: " + child.position);
            _counter++;
        }
        _nextTargetIndex = 0;
    }

    void Update()
    {
        _tempTargetsDestroyed = _targetsDestoryedStaticVar;
        //Update positions of all children if they moved
        for (int i = 0; i < _targetParent.transform.childCount; i++)
        {
            // Get pos of child(i)
            Vector3 _currentPosition = _targetParent.transform.GetChild(i).position;

            // Check if position has changed
            if (_currentPosition != _targetPositionsStaticVar[i])
            {
                // Update the position in list
                _targetPositionsStaticVar[i] = _currentPosition;
                //Debug.Log("NT - Updated target position " + i + ": " + _currentPosition);
            }
        }

        //not end of game, and child object is destoryed, increment child
        if (
            ScoreManager._finalTargetDestroyed == false
            && _targetsDestoryedStaticVar[_nextTargetIndex] == true
        )
        {
            Debug.Log("NTI - Object already destroyed: " + _nextTargetIndex);
            _nextTargetIndex++;
        }
    }
}
