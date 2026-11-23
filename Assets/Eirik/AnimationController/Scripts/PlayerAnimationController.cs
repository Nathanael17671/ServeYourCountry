using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

    [Header("Animation Settings")]
    [SerializeField] private float speedDampTime = 0.1f;

    private static readonly int Speed = Animator.StringToHash("Speed");

    private static readonly int IsCarrying = Animator.StringToHash("IsCarrying");
 
    private static readonly int IsInteracting = Animator.StringToHash("IsInteracting");

    private static readonly int Pickup = Animator.StringToHash("Pickup");     

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }
   
    public void SetMovement(float normalizedSpeed)
    {
        animator.SetFloat(Speed, Mathf.Clamp01(normalizedSpeed), speedDampTime, Time.deltaTime);
    }

    public void SetCarrying(bool carrying)
    {
        animator.SetBool(IsCarrying, carrying);
    }
    
    public void SetInteracting(bool interacting)
    {
        animator.SetBool(IsInteracting, interacting);
    }

    public void PlayPickup()
    {
        animator.SetTrigger(Pickup);
    }
}
