using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraBlur : MonoBehaviour
{
    public float startAperture = 32f; // not blurred aperture
    public float blurAperture = 5f; // blurred aperture
    public float blurDuration = 0.3f; // blur time

    private DepthOfField depthOfField;
    private Volume volume;
    private bool isBlurEnabled = false;

    private void Start()
    {
        // Get the DepthOfField 
        volume = GetComponent<Volume>();

        //did not work?
        //depthOfField = volume.GetComponent<DepthOfField>();
        
        volume.profile.TryGet(out depthOfField);
    }

    private void Update()
    {
        // Check if the bool is set and motion blur is not already enabled
        if (SpaceshipControls._laserFiringLeft && !isBlurEnabled)
        {
            // Start the coroutine to enable and disable motion blur
            StartCoroutine(EnableCameraBlur());
        }
    }

    private IEnumerator EnableCameraBlur()
    {
        isBlurEnabled = true;
        //Debug.Log("Aperture setting 1: " + depthOfField.aperture.value);

        // Wait for the duration of the motion blur effect
        float elapsedTime = 0.0f;
        while (elapsedTime < blurDuration)
        {
            //add random blur
            depthOfField.aperture.value = blurAperture + Random.Range(2f, -2f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Disable motion blur
        depthOfField.aperture.value = startAperture;
        isBlurEnabled = false;
        //Debug.Log("Aperture setting 2: " + depthOfField.aperture.value);
    }
}
