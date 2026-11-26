using UnityEngine;
using UnityEngine.AI;

public class CustomerController : MonoBehaviour
{
    public enum CustomerType
    {
        Regular,
        Resistance,
        Nazi
    }

    public enum ServiceType
    {
        Counter,
        Table
    }

    public enum CustomerState
    {
        WaitingForServicePoint,
        Entering,
        MovingToQueue,
        WaitingInQueue,
        Deciding,
        WaitingToOrder,
        WaitingForFood,
        Eating,
        Leaving
    }

    [Header("Customer Settings")]
    [SerializeField] private CustomerType customerType;
    [SerializeField] private ServiceType serviceType;

    [Header("References")]
    //[SerializeField] private CounterQueueManager counterQueueManager;
    [SerializeField] private ServicePointManager servicePointManager;
    [SerializeField] private ServicePoint assignedServicePoint;
    [SerializeField] private Transform exitPoint;

    [Header("Behaviour Timers")]
    [SerializeField] private float decisionTime = 3f;
    [SerializeField] private float eatingTime = 10f;
    [SerializeField] private float retryInterval = 1f;

    [Header("Patience Settings")]
    [SerializeField] private float counterPatience = 60f;
    [SerializeField] private float tablePatience = 100f;
    [SerializeField] private float maxPatience;
    [SerializeField] private float currentPatience;

    [Header("Prototype Settings")]
    [SerializeField] private bool automaticService = true;
    [SerializeField] private float testOrderTime = 3f;
    [SerializeField] private float testDeliveryTime = 5f;

    [Header("Current State")]
    [SerializeField] private CustomerState currentState;

    private NavMeshAgent agent;
    private float stateTimer;
    private Transform assignedQueuePoint;
    public float CurrentPatience => currentPatience;
    public float MaxPatience => maxPatience;
    public float PatienceNormalized => maxPatience > 0f ? currentPatience / maxPatience : 0f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        if (customerType == CustomerType.Resistance)
        {
            serviceType = ServiceType.Counter;
        }

        if (customerType == CustomerType.Nazi)
        {
            agent.speed = 2f;
        }

        if (servicePointManager == null ||
            exitPoint == null ||
            !agent.isOnNavMesh)
        {
            Debug.LogError("Customer setup is incomplete.", this);
            enabled = false;
            return;
        }
        
        InitializePatience();

        ChangeState(CustomerState.WaitingForServicePoint);
        FindServicePoint();        
    }

    private void Update()
    {
        stateTimer += Time.deltaTime;

        UpdatePatience();

        switch (currentState)
        {
            case CustomerState.WaitingForServicePoint:

                if (stateTimer >= retryInterval)
                {
                    stateTimer = 0f;
                    FindServicePoint();
                }
                break;

            case CustomerState.Entering:

                if (HasReachedDestination())
                {
                    assignedServicePoint.Occupy(this);

                    transform.rotation = assignedServicePoint.transform.rotation;
  
                    ChangeState(CustomerState.Deciding);
                }
                break;

            case CustomerState.Deciding:

                if (stateTimer >= decisionTime)
                {
                    ChangeState(CustomerState.WaitingToOrder);
                }
                break;

            case CustomerState.WaitingToOrder:

                if (automaticService && stateTimer >= testOrderTime)
                {
                    TakeOrder();
                }
                break;

            case CustomerState.WaitingForFood:

                if (automaticService && stateTimer >= testDeliveryTime)
                {
                    ServeCustomer();
                }
                break;

            case CustomerState.Eating:

                if (stateTimer >= eatingTime)
                {
                    ChangeState(CustomerState.Leaving);
                }
                break;

            case CustomerState.Leaving:

                if (HasReachedDestination())
                {
                    Destroy(gameObject);
                }
                break;
        }
    }

    private void FindServicePoint()
    {
        assignedServicePoint = servicePointManager.RequestServicePoint(this, serviceType);

        if (assignedServicePoint != null)
        {
            ChangeState(CustomerState.Entering);
        }
    }

    private void ChangeState(CustomerState newState)
    {
        currentState = newState;
        stateTimer = 0f;

        switch (newState)
        {
            case CustomerState.WaitingForServicePoint:

                agent.isStopped = true;
                break;

            case CustomerState.Entering:

                MoveTo(assignedServicePoint.transform.position);
                break;

            case CustomerState.Deciding:

                agent.isStopped = true;
                break;

            case CustomerState.Eating:

                eatingTime = Random.Range(5f, 15f);
                break;

            case CustomerState.Leaving:

                ReleaseServicePoint();
                MoveTo(exitPoint.position);
                break;
        }

        Debug.Log($"{gameObject.name}: {currentState}");
    }

    private void MoveTo(Vector3 destination)
    {
        NavMeshPath path = new NavMeshPath();

        if (!agent.CalculatePath(destination, path) || path.status != NavMeshPathStatus.PathComplete)
        {
            Debug.LogError("Customer destination unreachable.", this);

            ReleaseServicePoint();
            Destroy(gameObject);

            return;
        }

        agent.isStopped = false;
        agent.SetPath(path);
    }

    private void TryJoinQueue()
    {
        
    }

    private bool HasReachedDestination()
    {
        if (agent.pathPending) return false;   
        if (agent.pathStatus != NavMeshPathStatus.PathComplete) return false;  
        if (float.IsInfinity(agent.remainingDistance)) return false;
            
        return agent.remainingDistance <= agent.stoppingDistance && agent.velocity.sqrMagnitude < 0.01f;
    }

    public void TakeOrder()
    {
        if (currentState != CustomerState.WaitingToOrder) return;

        ChangeState(CustomerState.WaitingForFood);
    }

    public void ServeCustomer()
    {
        if (currentState != CustomerState.WaitingForFood) return;

        if (serviceType == ServiceType.Table)
        {
            ChangeState(CustomerState.Eating);
        }
        else
        {
            ChangeState(CustomerState.Leaving);
        }
    }

    private void InitializePatience()
    {
        if (serviceType == ServiceType.Counter)
        {
            maxPatience = counterPatience;
        }
        else
        {
            maxPatience = tablePatience;
        }

        currentPatience = maxPatience;
    }

    public bool ShouldDecreasePatience()
    {
        if (currentState == CustomerState.WaitingToOrder || currentState == CustomerState.WaitingForFood) 
        {
            return true;
        }
        
        return false;
    }

    private void UpdatePatience()
    {
        if (ShouldDecreasePatience())
        {
            currentPatience = Mathf.Max(0f, currentPatience - Time.deltaTime);

            if (currentPatience <= 0f)
            {
                Debug.Log($"{gameObject.name} ran out of patience!");
                ChangeState(CustomerState.Leaving); // add loss later
            }
        }
    }

    private void ReleaseServicePoint()
    {
        if (assignedServicePoint == null) return;
        
        assignedServicePoint.Release(this);
        assignedServicePoint = null;
    }

    private void OnDestroy()
    {
        ReleaseServicePoint();
    }
}
