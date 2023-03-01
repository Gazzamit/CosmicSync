using UnityEngine;
using System.Collections;
using Unity.VectorGraphics;

public class TargetRadar : MonoBehaviour
{
    public Transform shipTransform; //for calculating if target is behind ship
    public Vector3 targetPosition; // The target to track
    public float radius = 300.0f; // The radius of the radar
    public float flashInterval = 0.5f;
    public Color flashColourSlow, flashColourFast; // When target is off screen
    private Color flashColourSelect;

    // Private variables
    private Camera cam;
    private RectTransform rectTransform; // UI indicator
    private RectTransform parentRectTransform; // UI parent object

    private bool isFlashing = false;
    private Coroutine flashCoroutine = null;

    private Color originalColour; // Store original color of the indicator

    private void Awake()
    {
        // Get references to the camera and RectTransforms
        cam = Camera.main;
        rectTransform = GetComponent<RectTransform>();
        parentRectTransform = transform.parent.GetComponent<RectTransform>();

        // Store the color of the indicator and original colour
        originalColour = rectTransform.GetComponent<SVGImage>().color;
    }

    private void Update()
    {
        //Get the next target from collision coltroller script list
        targetPosition = TargetCollisionController._targetPositionsStaticVar[TargetCollisionController._nextTargetIndex];
        //Debug.Log("targetPosition: " + TargetCollisionController._nextTargetIndex + ": " + targetPosition);

        // Convert target position as viewport space
        Vector3 targetPos = cam.WorldToViewportPoint(targetPosition);
        Vector2 indicatorPos = Vector2.zero;

        // Check if the target is inside target square, snap to zero
        if (targetPos.z > 0.3f && targetPos.x > 0.4f && targetPos.x < 0.6f && targetPos.y > 0.35f && targetPos.y < 0.65f)
        {
            // Target is inside target square, zero UI indicator and stop flashing
            rectTransform.anchoredPosition = new Vector2(0, 0); // Move the UI indicator to center
            StopFlashing();
        }
        else
        {
            // Target is outside target square, flash UI indicator
            Vector3 screenPos = cam.WorldToScreenPoint(targetPosition);
            Vector2 viewportPos = cam.ScreenToViewportPoint(screenPos);

            // Direction from  center to the target
            Vector2 direction = (viewportPos - new Vector2(0.5f, 0.5f)).normalized;

            // Angle between the direction and the x-axis
            float angle = Mathf.Atan2(direction.y, direction.x);

            // Calculate the position of the UI indicator relative to the center of the radar
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;

            // indicatorPos = new Vector2(x, y);

            // Clamp UI to target square via parent rectTransform
            float aspectRatio = Screen.width / (float)Screen.height;
            float maxX = parentRectTransform.rect.width * 0.5f - rectTransform.rect.width * 0.5f;
            float maxY = parentRectTransform.rect.height * 0.5f - rectTransform.rect.height * 0.5f;

            // Place indicator on correct side of square depending upon sign
            if (Mathf.Abs(x) > maxX)
            {
                x = Mathf.Sign(x) * maxX;
            }
            if (Mathf.Abs(y) > maxY)
            {
                y = Mathf.Sign(y) * maxY;
            }

            indicatorPos = new Vector2(x, y);

            //check if target is behind the ship. Direction is ship to target
            Vector3 directionShipToTarget = (targetPosition - shipTransform.position).normalized;
            //dot produc of directio to target and ship forward
            float dot = Vector3.Dot(directionShipToTarget, shipTransform.forward);
            //negative dot product means target ois behind the ship
            if (dot < 0)
            {
                flashInterval = 0.1f;
                flashColourSelect = flashColourFast;
            }
            else
            {
                flashInterval = 0.3f;
                flashColourSelect = flashColourSlow;
            }

            // Start flashing if not already flashing
            if (!isFlashing)
            {
                // Start the flashing coroutine and store a reference to it
                flashCoroutine = StartCoroutine(FlashCoroutine());
            }

        }
        // Set the position of the UI indicator
        rectTransform.anchoredPosition = indicatorPos;
    }

    // Flash UI Indicator - target outside square target
    private IEnumerator FlashCoroutine()
    {
        isFlashing = true;
        while (true)
        {
            rectTransform.GetComponent<SVGImage>().color = flashColourSelect;
            yield return new WaitForSeconds(flashInterval);
            rectTransform.GetComponent<SVGImage>().color = originalColour;
            yield return new WaitForSeconds(flashInterval);
        }
    }


    // Stop flashing UI indicaor
    private void StopFlashing()
    {
        if (isFlashing)
        {
            StopCoroutine(flashCoroutine);
            isFlashing = false;
            // Reset the color UI
            rectTransform.GetComponent<SVGImage>().color = originalColour;
        }
    }

}
