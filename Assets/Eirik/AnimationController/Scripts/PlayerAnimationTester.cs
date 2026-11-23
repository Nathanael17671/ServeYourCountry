using UnityEngine;

public class PlayerAnimationTester : MonoBehaviour
{
    [SerializeField]
    private PlayerAnimationController animationController;

    [Range(0f, 1f)]
    public float testSpeed;
    public bool isCarrying;
    public bool isInteracting;

    private void Awake()
    {
        if (animationController == null) animationController = GetComponent<PlayerAnimationController>();
    }

    private void Update()
    {
        if (animationController == null)
            return;

        animationController.SetMovement(testSpeed);
        animationController.SetCarrying(isCarrying);
        animationController.SetInteracting(isInteracting);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            animationController.PlayPickup();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            isCarrying = !isCarrying;
            animationController.SetCarrying(isCarrying);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            isInteracting = !isInteracting;
            animationController.SetInteracting(isInteracting);
        }
    }

    [ContextMenu("Test Pickup")]
    public void TestPickup()
    {
        if (animationController != null)
        {
            animationController.PlayPickup();
        }
    }
}
