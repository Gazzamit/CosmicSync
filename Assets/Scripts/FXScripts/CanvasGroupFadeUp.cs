using UnityEngine;

public class CanvasGroupFadeUp : MonoBehaviour
{
    // The time it takes to fade in
    public float fadeInTime = 2f;

    // Reference to the object's CanvasGroup
    private CanvasGroup canvasGroup;
    private float timer = 0f;

    private void Start()
    {
        // Get the CanvasGroup component and set Alplh to 0
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
    }

    void Update()
    {
        if (timer < fadeInTime)
        {
            timer += Time.deltaTime;

            // Set the new alpha value with the calculated ratio
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeInTime);
        }
    }
}
