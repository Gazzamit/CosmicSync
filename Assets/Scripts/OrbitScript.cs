using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

private void onEnable()
{
    playerControls.Enable();
}

private void onDisable()
{
    playerControls.Disable();
}


 public class OrbitScript : MonoBehaviour
{
    public float radius = 5.0f;
    public float speed = 1.0f;
    public float tolerance = 0.1f;

    private float startTime;
    private bool passedTopDeadCenter;
    private float topDeadCenterTime;
    private float fullCycleTime;

    void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        float elapsedTime = Time.time - startTime;
        float angle = speed * elapsedTime;
        float x = radius * Mathf.Cos(angle);
        float y = radius * Mathf.Sin(angle);
        transform.position = new Vector3(x, y, 0);

        // Rotate the object to be upright
        transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);

        if (y >= radius && !passedTopDeadCenter)
        {
            passedTopDeadCenter = true;
            topDeadCenterTime = elapsedTime;
        }
        else if (y < radius && passedTopDeadCenter)
        {
            passedTopDeadCenter = false;
        }

        if (passedTopDeadCenter && fullCycleTime == 0)
        {
            fullCycleTime = elapsedTime;
        }

        if (fullCycleTime > 0 && elapsedTime >= fullCycleTime + 2 * Mathf.PI / speed)
        {
            fullCycleTime = 0;
            passedTopDeadCenter = false;
        }
    }

    void OnTap(InputAction.CallbackContext context)
    {
        float elapsedTime = Time.time - startTime;
        float tapTime = elapsedTime;
        float timeDifference = Mathf.Abs(tapTime - topDeadCenterTime);
        Debug.Log("Time Difference: " + timeDifference);

        if (timeDifference <= tolerance)
        {
            Debug.Log("Perfect tap!");
        }
        else if (timeDifference <= 2 * tolerance)
        {
            Debug.Log("Good tap!");
        }
        else if (timeDifference <= 3 * tolerance)
        {
            Debug.Log("Poor tap!");
        }
        else
        {
            Debug.Log("Missed tap!");
        }
    }
}