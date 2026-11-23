using UnityEngine;

public class WorkstationAnimationController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

    private static readonly int IsActive = Animator.StringToHash("IsActive");

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void SetActive(bool active)
    {
        animator.SetBool(IsActive, active);
    }
}
