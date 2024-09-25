using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeanBagThrow : MonoBehaviour
{
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

    [Header("Throw Settings")]
    [SerializeField] private GameObject beanBagPrefab; // Prefab for the bean bags
    [SerializeField] private float maxForce = 20f;
    [SerializeField] private float upwardForce = 5f;
    [SerializeField] private float throwForceMultiplier = 0.1f;

    [Header("Drag Settings")]
    public float dragOnGround = 5f;
    public float angularDragOnGround = 5f;
    public float airResistance = 0.2f;

    [Header("Bean Bag Management")]
    public int maxBags = 4; // Max number of bags the player can throw
    private int currentBagIndex = 0; // To track how many bags have been thrown
    private List<GameObject> beanBags = new List<GameObject>(); // List to store the instantiated bean bags

    [Header("Advanced Throw")]
    public AnimationCurve throwForceCurve;
    public LayerMask groundLayer;

    private bool isThrown = false;
    private Rigidbody currentBeanBagRigidbody;

    void Start()
    {
        SpawnNextBeanBag(); // Spawn the first bean bag at the start
    }

    void Update()
    {
        if (Input.touchCount > 0 && currentBagIndex < maxBags)
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

        if (isThrown && IsOnGround(currentBeanBagRigidbody))
        {
            ApplyGroundDrag(currentBeanBagRigidbody);
        }
    }

    void ThrowBeanBag()
    {
        if (currentBagIndex >= maxBags) return; // Limit to max bags

        // Calculate swipe direction and force
        Vector2 swipeDirection = endTouchPosition - startTouchPosition;
        float swipeMagnitude = swipeDirection.magnitude * throwForceMultiplier;

        // Clamp swipe magnitude to prevent excessive force
        swipeMagnitude = Mathf.Clamp(swipeMagnitude, 0f, maxForce);

        // Apply the force curve for more control over throw behavior
        float adjustedForce = throwForceCurve.Evaluate(swipeMagnitude / maxForce) * maxForce;

        // Adjust throw direction and add upward force
        Vector3 throwDirection = new Vector3(swipeDirection.x, 0, swipeDirection.y).normalized;
        Vector3 finalThrowDirection = throwDirection * adjustedForce + Vector3.up * upwardForce;

        // Apply impulse force to the current bean bag
        currentBeanBagRigidbody.AddForce(finalThrowDirection, ForceMode.Impulse);
        isThrown = true;

        // Start air resistance coroutine
        StartCoroutine(ApplyAirResistance(currentBeanBagRigidbody));

        // Move to the next bean bag
        currentBagIndex++;
        if (currentBagIndex < maxBags)
        {
            SpawnNextBeanBag(); // Spawn the next bag
        }
    }

    public void SpawnNextBeanBag()
    {
        // Instantiate a new bean bag
        GameObject newBeanBag = Instantiate(beanBagPrefab, transform.position, Quaternion.identity);
        beanBags.Add(newBeanBag);
        currentBeanBagRigidbody = newBeanBag.GetComponent<Rigidbody>();
        ResetDrag(currentBeanBagRigidbody);
    }

    IEnumerator ApplyAirResistance(Rigidbody beanBagRigidbody)
    {
        // Gradually apply air resistance while the object is airborne
        while (!IsOnGround(beanBagRigidbody))
        {
            beanBagRigidbody.drag = airResistance;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f); // Allow time before applying ground drag
        ApplyGroundDrag(beanBagRigidbody);
    }

    void ApplyGroundDrag(Rigidbody beanBagRigidbody)
    {
        // Apply increased drag once the bean bag lands
        beanBagRigidbody.drag = dragOnGround;
        beanBagRigidbody.angularDrag = angularDragOnGround;
        isThrown = false;
    }

    bool IsOnGround(Rigidbody beanBagRigidbody)
    {
        // Raycast to check if the bean bag is on the ground
        return Physics.Raycast(beanBagRigidbody.transform.position, Vector3.down, 1f, groundLayer);
    }

    void ResetDrag(Rigidbody beanBagRigidbody)
    {
        // Reset drag for initial throw conditions
        beanBagRigidbody.drag = 0f;
        beanBagRigidbody.angularDrag = 0.05f;
    }
}
