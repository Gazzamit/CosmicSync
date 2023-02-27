using UnityEngine;

public class TargetIndicator : MonoBehaviour
{
    public Transform target;
    public float radius = 100.0f;

    private Camera cam;
    private RectTransform rectTransform;

    private void Awake()
    {
        cam = Camera.main;
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        Vector3 targetPos = cam.WorldToViewportPoint(target.position);
        Vector2 indicatorPos = Vector2.zero;

        if (targetPos.z > 0.3f && targetPos.x > 0.3f && targetPos.x < .7f && targetPos.y > 0.3f && targetPos.y < .7f)
        {
            // Target is visible on screen, hide the UI indicator
            rectTransform.anchoredPosition = new Vector2(-1000, -1000); // Move the UI indicator off screen
        }
        else
        {
            // Target is not visible on screen, show the UI indicator
            Vector3 screenPos = cam.WorldToScreenPoint(target.position);
            Vector2 viewportPos = cam.ScreenToViewportPoint(screenPos);

            Vector2 direction = (viewportPos - new Vector2(0.5f, 0.5f)).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x);

            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;

            indicatorPos = new Vector2(x, y);

            // Clamp the position of the UI indicator to the edge of the screen
            float aspectRatio = Screen.width / (float)Screen.height;
            float maxX = Screen.width * 0.5f - rectTransform.rect.width * 0.5f;
            float maxY = Screen.height * 0.5f - rectTransform.rect.height * 0.5f;

            if (Mathf.Abs(x) > maxX)
            {
                x = Mathf.Sign(x) * maxX;
            }

            if (Mathf.Abs(y) > maxY)
            {
                y = Mathf.Sign(y) * maxY;
            }

            indicatorPos = new Vector2(x, y);
        }

        // Set the position of the UI indicator
        rectTransform.anchoredPosition = indicatorPos;
    }
}
