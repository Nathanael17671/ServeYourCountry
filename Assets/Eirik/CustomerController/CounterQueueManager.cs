using System.Collections.Generic;
using Unity.Hierarchy;
using UnityEngine;

public class CounterQueueManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<Transform> queuePoints = new List<Transform>();
    [SerializeField] private ServicePointManager servicePointManager;

    [Header("Current Queue")]
    [SerializeField] private List<CustomerController> waitingCustomers = new List<CustomerController>();

    public int QueueCount => waitingCustomers.Count;
    public bool HasSpace => waitingCustomers.Count < queuePoints.Count;

    public bool JoinQueue(CustomerController customer)
    {
        if (!HasSpace || customer == null || waitingCustomers.Contains(customer))
        {
            return false;
        }
            
        waitingCustomers.Add(customer);

        return true;
    }

    public bool LeaveQueue(CustomerController customer)
    {
        return waitingCustomers.Remove(customer);
    }

    public Transform GetQueuePoint(CustomerController customer)
    {
        int index = waitingCustomers.IndexOf(customer);

        if (index < 0 || index >= queuePoints.Count)
        {
            return null;
        }

        return queuePoints[index];
    }

    public bool IsFirstInQueue(CustomerController customer)
    {
        return waitingCustomers.IndexOf(customer) == 0;
    }
}
