using UnityEngine;

public class TableValidityChecker : MonoBehaviour
{
    [SerializeField] internal MasterManager masterManager;
    
    public bool CanPickup(MainTableScript currentTableScript)
    {
        TableType mainTableTypeCheck = currentTableScript.tableType;
        //Table type check
        switch (mainTableTypeCheck)
            {
                case TableType.Trash:
                    Debug.Log("Cannot pick up item from a " + mainTableTypeCheck + "!");
                    return false;
                case TableType.Workbench:
                    WorkbenchScript workbenchScript = currentTableScript.GetComponent<WorkbenchScript>();
                    if (!workbenchScript.canPickupPutDown)
                    {
                        return false;
                    }
                break;
            }
        //Is item on table?
        if (!currentTableScript.HasItemOnTable())
        {
            Debug.Log("No item to pick up");
            return false;
        }
        return true;
    }

    public int CanPutDown(MainTableScript currentTableScript, GameObject handItem)
    {
        
        // 0 = Do Nothing, 
        // 1 = Put on Table,
        // 2 = Put in Hand (Not in use),
        // 3 = Move to Container, put on Table, 
        // 4 = Move to Container, put in Hand
        // 5 = Combine Items, and put on Table
        // 6 = Combine Items, and put in Hands
        TableType currentTableType = currentTableScript.tableType;
        ItemManager handItemScript = handItem.GetComponent<ItemManager>();

        if (currentTableType == TableType.Workbench)
        {
            if (!currentTableScript.workbenchScript.canPickupPutDown) return 0;
        }
        
        //Table type check
        if (currentTableScript.HasItemOnTable()) //There is something in both the hands and on the table
        {   
            //Check Container Transfer:
            int containerResult = CanPlateThisItem(handItemScript, currentTableScript);
            if (containerResult == 1)
            {
                return 3;
            } 
            else if (containerResult == 2)
            {
                return 4;
            }
            
            //Check Combination Recipe
            IngridientID combinationRecipeResult = masterManager.recipeChecker.CombinationRecipeCheck(handItemScript.ingridientScript, currentTableScript.tableItemScript.ingridientScript);
            if (combinationRecipeResult != IngridientID.None)
            {
                if (CheckPlacementData(ItemType.Ingridient, combinationRecipeResult, ContainerID.None, currentTableScript))
                {
                    return 5;
                }
                else
                {
                    return 6;
                }
            }
            
            switch (currentTableType) // Check Other Table Functions
            {
                case TableType.Spawner:
                    SpawnerTable spawnerTable = currentTableScript.GetComponent<SpawnerTable>();
                    if (handItemScript.itemType == spawnerTable.spawnItemType && (
                        spawnerTable.spawnItemType == ItemType.Ingridient ? 
                            handItemScript.ingridientScript.ingridientID == spawnerTable.spawnIngridientID : 
                            handItemScript.containerScript.containerID == spawnerTable.spawnContainerID
                            ))
                    {
                        if (handItemScript.itemType == ItemType.Container) //Dont return Container with items in it to spawner
                        {
                            ContainerScript container = handItemScript.containerScript;
                            if (!container.IsEmpty())
                            {
                                return 0;
                            }
                        }
                        Destroy(currentTableScript.tableItemGameobject);
                        currentTableScript.tableItemGameobject = null;
                        return 1;
                    }
                    Debug.Log("Cannot put down this item on the Spawner!");
                return 0;
            }

            Debug.Log("Table is full");
            return 0;
        } 
        else //There is nothing on the table, and something in the hands
        {
            if (handItemScript.itemType == ItemType.Ingridient ? 
                !CheckPlacementData(handItemScript.itemType, handItemScript.ingridientScript.ingridientID, ContainerID.None, currentTableScript) : 
                !CheckPlacementData(handItemScript.itemType, IngridientID.None, handItemScript.containerScript.containerID, currentTableScript)
                )
            {
                return 0;
            }
            
            switch (currentTableType)
            {
                case TableType.Spawner:
                    return 0;
                case TableType.Trash:
                    if (handItemScript.itemType == ItemType.Container)
                    {
                        handItemScript.containerScript.ClearItems();
                        return 0;
                    }
                    return 1;
            }
            return 1;
        }
    }

    private int CanPlateThisItem(ItemManager handItemScript, MainTableScript currentTableScript)
    {
        //Is there a plate in hand
        int isThereAContainer = 0;
        // if = 0, No Containers
        // if = 1, Hand Container
        // if = 2, Table Container
        // if = 3, Two Containers
        ItemManager containerItem = null;
        ItemManager toBePlatedItem = null;
        if (handItemScript.itemType == ItemType.Container) //Is there a plate in hands
        {
            isThereAContainer += 1;
        }
        if (currentTableScript.tableItemScript.itemType == ItemType.Container) //Is there a plate on table
        {
            isThereAContainer += 2;
        }
        
        switch (isThereAContainer)
        {
            case 0:
            break;
            case 1:
                containerItem = handItemScript;
                toBePlatedItem = currentTableScript.tableItemScript;
            break;
            case 2:
                containerItem = currentTableScript.tableItemScript;
                toBePlatedItem = handItemScript;
            break;
            case 3:
            break;
        }
        
        if (isThereAContainer is 1 or 2) //Test if we can move item from table into hand plate
        {
            if (containerItem.containerScript.CanAddItem(toBePlatedItem))
            {
                if (CheckPlacementData(ItemType.Container, IngridientID.None, containerItem.containerScript.containerID, currentTableScript))
                {
                    return 1;
                }
                else
                {
                    return 2;
                }
            }
        }
        return 0;
    }

    public bool CheckPlacementData(ItemType itemType, IngridientID ingridientID, ContainerID containerID, MainTableScript checkMainTable)
    {
        PlacementData placementData = null;
        switch (itemType)
        {
            case ItemType.Container:
                placementData = masterManager.itemDatabaseSO.GetContainer(containerID).placementData;
            break;
            case ItemType.Ingridient:
                placementData = masterManager.itemDatabaseSO.GetIngridient(ingridientID).placementData;
            break;
        }
        switch (checkMainTable.tableType)
        {
            case TableType.Normal:
                return placementData.normalTable;
            case TableType.Spawner:
                return false;
            case TableType.Trash:
                return placementData.binTable;
            case TableType.Delivery:
                return placementData.deliveryTable;
            case TableType.Workbench:
                return checkMainTable.workbenchScript.CheckPlacementDataWorkbench(placementData);
        }
        Debug.LogError("Something went wrong when checking placement validity");
        return false;
    }
}
