using UnityEngine;

public class CustomerAnimationTester : MonoBehaviour
{
    [SerializeField]
    private CustomerAnimationController animationController;

    [Range(0f, 1f)]
    public float testSpeed;

    public bool testSitting;
    public bool testEating;

    private void Awake()
    {
        if (animationController == null)
            animationController =
                GetComponent<CustomerAnimationController>();
    }

    private void Update()
    {
        if (animationController == null)
            return;

        animationController.SetMovement(testSpeed);
        animationController.SetSitting(testSitting);
        animationController.SetEating(testEating);
    }
}
