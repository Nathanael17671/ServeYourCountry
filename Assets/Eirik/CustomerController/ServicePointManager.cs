using System.Collections.Generic;
using UnityEngine;

public class ServicePointManager : MonoBehaviour
{
    [Header("Service Points")]
    [SerializeField] private List<ServicePoint> servicePoints = new List<ServicePoint>();

    public ServicePoint RequestServicePoint(CustomerController customer, CustomerController.ServiceType serviceType)
    {
        List<ServicePoint> availablePoints = new List<ServicePoint>();
        
        foreach (ServicePoint point in servicePoints)
        {
            if (point.IsAvailable && point.ServiceType == serviceType)
            {
                availablePoints.Add(point);
            }
        }

        if (availablePoints.Count == 0) return null;

        int randomIndex = Random.Range(0, availablePoints.Count);

        ServicePoint selectedPoint = availablePoints[randomIndex];

        if (selectedPoint.TryReserve(customer))
        {
            return selectedPoint;
        }
        
        return null;
    }

    public bool HasAvailablePoint(CustomerController.ServiceType serviceType)
    {
        foreach (ServicePoint point in servicePoints)
        {
            if (point != null && point.IsAvailable && point.ServiceType == serviceType)
            {
                return true;
            }
        }

        return false;
    }
}
