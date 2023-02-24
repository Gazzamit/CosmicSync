using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class LatencyTester : MonoBehaviour
{
    private double lastInputTime;
    private StreamWriter writer;

    private void OnEnable()
    {
        var inputAction = new InputAction(binding: "<keyboard>/p");
        inputAction.performed += OnInputAction;
        inputAction.Enable();

        // Open file for writing
        writer = new StreamWriter("LatencyLog.txt", false);
    }

    private void OnDisable()
    {
        var inputAction = new InputAction(binding: "<keyboard>/p");
        inputAction.performed -= OnInputAction;
        inputAction.Disable();

        // Close file
        writer.Close();
    }

    private void OnInputAction(InputAction.CallbackContext context)
    {
        double currentTime = Time.realtimeSinceStartup;
        double latency = (currentTime - context.startTime) * 1000f;
        Debug.Log("Latency: " + latency + " ms");

        // Write latency to file
        writer.WriteLine("Latency: " + latency + " ms");
    }
}
