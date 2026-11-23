using UnityEngine;

public class CustomerAnimationController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

    [Header("Settings")]
    [SerializeField] private float speedDampTime = 0.1f;

    private static readonly int Speed = Animator.StringToHash("Speed");

    private static readonly int IsSitting = Animator.StringToHash("IsSitting");

    private static readonly int IsEating = Animator.StringToHash("IsEating");
    
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>(); 
    }

    public void SetMovement(float normalizedSpeed)
    {
        animator.SetFloat(Speed,Mathf.Clamp01(normalizedSpeed),speedDampTime,Time.deltaTime);
    }

    public void SetSitting(bool sitting)
    {
        animator.SetBool(IsSitting, sitting);
    }

    public void SetEating(bool eating)
    {
        animator.SetBool(IsEating, eating);
    }
}
