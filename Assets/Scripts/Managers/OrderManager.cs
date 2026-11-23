using UnityEngine;
using System.Collections.Generic;

public class OrderManager : MonoBehaviour
{
    [SerializeField] internal MasterManager masterManager;
    private ItemDatabaseSO itemDatabaseSO;
    public List<DiningTable> diningTables;

    void Awake()
    {
        itemDatabaseSO = masterManager.itemDatabaseSO;
        for(int i = 0; i < itemDatabaseSO.orders.Count; i++)
        {
            itemDatabaseSO.orders[i].ingridients.Sort();
            for(int j = 0; j < itemDatabaseSO.orders[i].ingridients.Count; j++)
            {
                ItemID ingridientID = itemDatabaseSO.orders[i].ingridients[j];
                ItemData ingridientData = itemDatabaseSO.GetItem(ingridientID);
                if (!ingridientData.canBePlated)
                {
                    Debug.LogError(ingridientID + " in recipe " + itemDatabaseSO.orders[i].Name + ", can not be plated!");
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
        
            if (itemDatabaseSO.orders.Count > 0)
            {
                for (int i = 0; i < diningTables.Count; i++)
                {
                    int randomOrder = Random.Range(0, itemDatabaseSO.orders.Count);
                    diningTables[i].NewOrder(itemDatabaseSO.orders[randomOrder]);
                }
            }
         
    }
}
