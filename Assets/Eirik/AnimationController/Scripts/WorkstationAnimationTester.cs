using UnityEngine;

public class WorkstationAnimationTester : MonoBehaviour
{
    [SerializeField]
    private WorkstationAnimationController animationController;

    [Header("Testing")]
    public bool testActive;

    private void Awake()
    {
        animationController = GetComponent<WorkstationAnimationController>();     
    }

    private void Update()
    {
        animationController.SetActive(testActive);
    }
}
