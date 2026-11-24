using UnityEngine;

public class ServicePoint : MonoBehaviour
{
    public enum PointState
    {
        Available,
        Reserved,
        Occupied,
        NeedsReset
    }

    [Header("Service Point Settings")]
    [SerializeField] private CustomerController.ServiceType serviceType;

    [Header("Reset Settings")]
    [SerializeField] private bool requiresReset;

    [Header("Current Status")]
    [SerializeField] private PointState currentState = PointState.Available;

    [SerializeField] private CustomerController currentCustomer;

    public CustomerController.ServiceType ServiceType => serviceType;
    public PointState CurrentState => currentState;
    public bool IsAvailable => currentState == PointState.Available;

    public bool TryReserve(CustomerController customer)
    {
        if (!IsAvailable || customer == null) return false;

        currentCustomer = customer;
        currentState = PointState.Reserved;

        return true;
    }
    
    public void Occupy(CustomerController customer)
    {
        if (currentCustomer != customer || currentState != PointState.Reserved) return;

        currentState = PointState.Occupied;
    }

    public void Release(CustomerController customer)
    {
        if (currentCustomer != customer) return;

        bool wasOccupied = currentState == PointState.Occupied;

        currentCustomer = null;

        if (requiresReset && wasOccupied)
        {
            currentState = PointState.NeedsReset;
        }
        else
        {
            currentState = PointState.Available;
        }
    }

    public void OnDrawGizmos()
    {
        switch (currentState)
        {
            case PointState.Available:
                Gizmos.color = Color.green;
                break;

            case PointState.Reserved:
                Gizmos.color = Color.yellow;
                break;

            case PointState.Occupied:
                Gizmos.color = Color.red;
                break;

            case PointState.NeedsReset:
                Gizmos.color = Color.gray;
                break;
        }

        Gizmos.DrawWireSphere(transform.position, 0.3f);

        Gizmos.DrawRay(transform.position, transform.forward * 0.7f);
    }
}
