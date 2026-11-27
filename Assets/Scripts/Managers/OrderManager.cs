using UnityEngine;
using System.Collections.Generic;

public class OrderManager : MonoBehaviour
{
    [SerializeField] internal MasterManager masterManager;
    private OrderDatabaseSO orderDatabaseSO;
    public List<DiningTable> diningTables;

    void Awake()
    {
        orderDatabaseSO = masterManager.orderDatabaseSO;
        ItemDatabaseSO itemDatabaseSO = masterManager.itemDatabaseSO;
        for(int i = 0; i < orderDatabaseSO.orders.Count; i++)
        {
            orderDatabaseSO.orders[i].ingridients.Sort();
            for(int j = 0; j < orderDatabaseSO.orders[i].ingridients.Count; j++)
            {
                IngridientID ingridientID = orderDatabaseSO.orders[i].ingridients[j];
                IngridientData ingridientData = itemDatabaseSO.GetIngridient(ingridientID);
                if (!ingridientData.canBeStoredIn.Contains(ContainerID.DeliveryPlate))
                {
                    Debug.LogError("Order Can Not be Plated");
                }
            }
        }
    }

    public void AddTableToManager(DiningTable newTable)
    {
        if (diningTables.Count == 0) diningTables.Add(newTable);
        if (!diningTables.Contains(newTable))
        {
            diningTables.Add(newTable);
        }
    }

    void Start()
    {
        NewOrders();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            NewOrders();
        }
    }
    void NewOrders()
    {
        if (orderDatabaseSO.orders.Count > 0)
        {
            for (int i = 0; i < diningTables.Count; i++)
            {
                
                diningTables[i].NewOrder();
            }
        }
    }
}
