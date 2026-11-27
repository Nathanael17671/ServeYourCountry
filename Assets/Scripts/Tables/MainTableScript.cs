using UnityEngine;

public enum TableType
{
    Normal,
    Spawner,
    Trash,
    Delivery,
    Workbench
}

public class MainTableScript : MonoBehaviour
{
    //[Header("Table References")]
    internal MasterManager masterManager;
    internal TableValidityChecker tableValidityChecker;
    internal RecipeChecker recipeChecker;
    
    [Header("Table Settings")]
    [SerializeField] internal GameObject itemHoldPoint;
    [SerializeField] private GameObject indicator;
    [SerializeField] internal TableType tableType;
    [SerializeField] internal WorkbenchType workbenchType;
    [Header("Start Item")]
    [SerializeField] private ItemType startItemType;
    [SerializeField] private IngridientID startIngridient;
    [SerializeField] private ContainerID startContainer;
    
    //[Header("Table Inventory")]
    internal GameObject tableItemGameobject;
    internal ItemManager tableItemScript;

    internal WorkbenchScript workbenchScript;

    void Awake()
    {
        masterManager = FindAnyObjectByType<MasterManager>();
        tableValidityChecker = masterManager.tableValidityChecker;
        recipeChecker = masterManager.recipeChecker;
    }

    void Start()
    {
        SetIndicatorActive(false);

        if (tableType == TableType.Workbench)
        {    
            workbenchScript = GetComponent<WorkbenchScript>();          
        }
        if (startItemType != ItemType.None) SpawnStartItem();
    }

    void SpawnStartItem()
    {
        GameObject spawnPrefab = null;
        switch (startItemType)
        {
            case ItemType.Container:
                spawnPrefab = masterManager.itemDatabaseSO.GetContainer(startContainer).Prefab;
            break;
            case ItemType.Ingridient:
                spawnPrefab = masterManager.itemDatabaseSO.GetIngridient(startIngridient).Prefab;
            break;
        }
        if (spawnPrefab == null)
        {
            Debug.LogError("Item in database is missing prefab OR startItemType:" + startItemType + " / startIngridientID: " + startIngridient + " / startContainerID: " + startContainer + " is incorrect");
        }
        //Remove current Item
        Destroy(tableItemGameobject);
        tableItemGameobject = null;
        tableItemScript = null;

        //Create new Item
        tableItemGameobject = Instantiate(spawnPrefab, itemHoldPoint.transform.position, Quaternion.identity);
        tableItemGameobject.transform.parent = itemHoldPoint.transform;
        tableItemScript = tableItemGameobject.GetComponent<ItemManager>();
        tableItemScript.currentTableScript = this;
    }

    public void SetIndicatorActive(bool isActive)
    {
        indicator.SetActive(isActive);
    }

    public bool HasItemOnTable()
    {
        if (tableItemGameobject == null || tableItemScript == null)
        {
            return false;
        }
        return true;
    }

    public GameObject Pickup()
    {
        if (tableItemScript.itemType == ItemType.Container)
        {
            GameObject ingridientGameobject = tableItemScript.containerScript.TakeOutFirstAvailableItem();
            if (ingridientGameobject != null)
            {
                ItemManager ingridientScript = ingridientGameobject.GetComponent<ItemManager>();
                ingridientScript.UpdateRecipeData(null);
                ingridientScript.ResetTimers();
                ingridientScript.currentTableScript = null;
                return ingridientGameobject;
            }
        }

        //Take item from table
        tableItemGameobject.transform.parent = null;
        GameObject pickedUpItem = tableItemGameobject;
        tableItemGameobject = null;

        tableItemScript.UpdateRecipeData(null);
        tableItemScript.ResetTimers();
        tableItemScript.currentTableScript = null;
        tableItemScript = null;
        
        //Spawn new item
        if (tableType == TableType.Spawner)
        {
            GetComponent<SpawnerTable>().SpawnNewItem();
        }
        return pickedUpItem;
    }

    public void PutDownItem(GameObject item, int action)
    {
        // 0 = Put on Table,
        // 1 = Move to Container, put on Table, 
        // 2 = Combine Items, and put on Table

        ItemManager itemScript = item.GetComponent<ItemManager>();
        
        
        switch (action)
        {
            case 0:
                tableItemGameobject = item;
            break;
            case 1: //Is it a Plate
                if (tableItemScript.itemType == ItemType.Container)
                {
                    ContainerScript containerScript = tableItemScript.containerScript;
                    containerScript.AddItem(item);
                } 
                else if (itemScript.itemType == ItemType.Container)
                {
                    ContainerScript containerScript = itemScript.containerScript;
                    containerScript.AddItem(tableItemGameobject);
                    tableItemGameobject = item;
                } 
            break;
            case 2: //Combine Ingridients
                IngridientID combinationResult = masterManager.recipeChecker.CombinationRecipeCheck(itemScript.ingridientScript, tableItemScript.ingridientScript);
                Destroy(tableItemGameobject);
                tableItemGameobject = null;
                tableItemScript = null;
                Destroy(item);
                GameObject newItemPrefab = masterManager.itemDatabaseSO.GetIngridient(combinationResult).Prefab;
                tableItemGameobject = Instantiate(newItemPrefab, itemHoldPoint.transform.position, Quaternion.identity);
            break;
        }
        //Place item
        tableItemGameobject.transform.parent = itemHoldPoint.transform;
        tableItemGameobject.transform.localPosition = Vector3.zero;
        tableItemGameobject.transform.localRotation = Quaternion.identity;
        //Get item script
        tableItemScript = tableItemGameobject.GetComponent<ItemManager>();
        tableItemScript.currentTableScript = this;
        if (tableType == TableType.Workbench)
        {
            workbenchScript.CheckHeldItemRecipe();
        }
            
        //Destroy item if bin
        if (tableType == TableType.Trash)
        {
            Destroy(tableItemGameobject);
            tableItemGameobject = null;
            tableItemScript = null;
            return;
        }

        if (tableType == TableType.Delivery && tableItemScript.itemType == ItemType.Container)
        {
            if (!tableItemScript.containerScript.containerData.isForDelivery)
                return;
            DiningTable diningTableScript = GetComponent<DiningTable>();
            if (diningTableScript.MatchingOrder(tableItemScript.containerScript.ingridientIDList))
            {
                Debug.Log("Matching");
                diningTableScript.OrderFulfilled(tableItemGameobject);
                tableItemGameobject = null;
                tableItemScript = null;
                return;
            }
            else
            {
                //Debug.Log("Not Matching");
            }
        }
    }

    public void InteractPress()
    {
        if (tableType != TableType.Workbench)
            return;
        workbenchScript.ToggleActivation();
    }

    public void InteractHold()
    {
        if (tableType != TableType.Workbench)
            return;
        if (!HasItemOnTable())
            return;
        workbenchScript.ProssesWorkbench();
    }

}
