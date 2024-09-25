using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeanBagThrow : MonoBehaviour
{
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private Rigidbody beanBagRigidbody;
    [SerializeField] float maxForce;
    // Drag values after throw to prevent sliding
    public float dragOnGround = 5f;
    public float angularDragOnGround = 5f;

    // Tweakable parameters for the throw
    public float throwForceMultiplier = 0.1f; // Adjust the overall force applied
    public float upwardForce = 5f; // Additional vertical force

    void Start()
    {
        beanBagRigidbody = GetComponent<Rigidbody>();
        ResetDrag(); // Set initial drag to 0 or low to allow the throw
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                endTouchPosition = touch.position;
                ThrowBeanBag();
            }
        }
    }

    void ThrowBeanBag()
    {
        // Calculate swipe direction and force
        Vector2 swipeDirection = endTouchPosition - startTouchPosition;
        float swipeMagnitude = swipeDirection.magnitude * throwForceMultiplier;

        // Clamp the swipe magnitude to a maximum value (for example, 20)
        // maxForce = 20f; // You can adjust this value
        swipeMagnitude = Mathf.Clamp(swipeMagnitude, 0f, maxForce);

        // Throw direction and upward force for arc-like throw
        Vector3 throwDirection = new Vector3(swipeDirection.x, 0, swipeDirection.y).normalized;
        Vector3 finalThrowDirection = throwDirection * swipeMagnitude + Vector3.up * upwardForce;

        // Apply force to the bean bag
        beanBagRigidbody.AddForce(finalThrowDirection, ForceMode.Impulse);

        // Add drag when the object is thrown to slow down post-landing sliding
        StartCoroutine(ApplyGroundDragAfterThrow());
    }

    IEnumerator ApplyGroundDragAfterThrow()
    {
        // Wait for the throw to complete before applying drag
        yield return new WaitForSeconds(0.5f); // Allow the throw to fully apply before increasing drag

        // Increase drag and angular drag to prevent sliding
        beanBagRigidbody.drag = dragOnGround;
        beanBagRigidbody.angularDrag = angularDragOnGround;
    }

    void ResetDrag()
    {
        // Reset drag when starting or resetting the throw
        beanBagRigidbody.drag = 0f;
        beanBagRigidbody.angularDrag = 0.05f;
    }
}
