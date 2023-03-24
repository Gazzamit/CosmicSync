using UnityEngine;
using UnityEngine.InputSystem;

public class MoveTowardsClick : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private float _movementSpeed = 5f;
    private Vector3 _targetPosition;

    public GameObject _uiTarget;

    public void MoveTowardsTheClick(InputAction.CallbackContext context)
    {
        //Debug.Log("Callback Move: " + context.phase);
        if (context.started)
        {
            // Get the position of the mouse click in world space
            Vector3 mousePosition = Mouse.current.position.ReadValue();
            //Debug.Log("MousePosition: " + mousePosition);

            Ray ray = _mainCamera.ScreenPointToRay(mousePosition);

            //Debug.DrawLine(ray.origin, ray.direction * 100f, Color.red, 5f);
            
            
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 30f))
            {
                //Debug.Log("Hit object name: " + hit.collider.name);
                if (hit.collider.name == "UI_Target")
                {
                    //Debug.Log("Hit the UI Collider");
                    _targetPosition = hit.point;
                }
            }
        }
    }

    private void Update()
    {
        // Move the object towards the target position
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _movementSpeed * Time.deltaTime);
    }
}
