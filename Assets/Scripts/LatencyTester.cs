using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class LatencyTester : MonoBehaviour
{
    private double _lastInputTime;
    private StreamWriter _writer;

    private void OnEnable()
    {
        var _inputAction = new InputAction(binding: "<keyboard>/p");
        _inputAction.performed += OnInputAction;
        _inputAction.Enable();

        // Open file for writing
        _writer = new StreamWriter("LatencyLog.txt", false);
    }

    private void OnDisable()
    {
        var _inputAction = new InputAction(binding: "<keyboard>/p");
        _inputAction.performed -= OnInputAction;
        _inputAction.Disable();

        // Close file
        _writer.Close();
    }

    private void OnInputAction(InputAction.CallbackContext context)
    {
        double currentTime = Time.realtimeSinceStartup;
        double latency = (currentTime - context.startTime) * 1000f;
        Debug.Log("Latency: " + latency + " ms");

        // Write latency to file
        _writer.WriteLine("Latency: " + latency + " ms");
    }
}