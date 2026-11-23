using UnityEngine;

public class TableValidityChecker : MonoBehaviour
{
    [SerializeField] internal MasterManager masterManager;
    private ItemDatabaseSO itemDatabaseSO;

    void Awake()
    {
        itemDatabaseSO = masterManager.itemDatabaseSO;
    }
    
    public bool CanPickup(MainTableScript currentTableScript)
    {
        TableType mainTableTypeCheck = currentTableScript.mainTableType;
        //Table type check
        switch (mainTableTypeCheck)
            {
                case TableType.Trash:
                    Debug.Log("Cannot pick up item from a " + mainTableTypeCheck + "!");
                    return false;
                case TableType.ToggleActivation:
                    ToggleActivationTable toggleActivationTable = currentTableScript.GetComponent<ToggleActivationTable>();
                    if (toggleActivationTable.tableIsActive)
                        return false;
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
        // 3 = Move to Plate, put on Table, 
        // 4 = Move to Plate, put in Hand
        // 5 = Combine Items, and put on Table
        // 6 = Combine Items, and put in Hands
        TableType mainTableTypeCheck = currentTableScript.mainTableType;
        Items handItemScript = handItem.GetComponent<Items>();

        //Is there a plate in hand
        bool isThereAPlate = false;
        Plate plate = null;
        if (handItemScript.itemData.IsAPlate) //We need it outside for Bin
        {
            plate = handItemScript.plateScript;
            isThereAPlate = true;
        }

        //Table type check
        if (currentTableScript.HasItemOnTable()) //There is something in both the hands and on the table
        {   
            if (isThereAPlate) //Test if we can move item from table into hand plate
            {
                if (plate.CanAddItem(currentTableScript.heldTableItemScript))
                {
                    if (CanIPlaceItemOnTable(ItemID.Plate, mainTableTypeCheck))
                    {
                        return 3;
                    }
                    else
                    {
                        return 4;
                    }
                }
                else
                {
                    return 0;
                }
            } 
            else if (currentTableScript.heldTableItemScript.itemData.IsAPlate) //Is there a plate on table
            {
                plate = currentTableScript.heldTableItemScript.plateScript;
                if (plate.CanAddItem(handItemScript))
                {
                    if (CanIPlaceItemOnTable(ItemID.Plate, mainTableTypeCheck))
                    {
                        return 3;
                    }
                    else
                    {
                        return 4;
                    }
                }
                else
                {
                    return 0;
                }
            }

            ItemID combinationRecipeCheck = CombinationRecipeCheck(handItemScript, currentTableScript.heldTableItemScript);
            if (combinationRecipeCheck != ItemID.None)
            {
                if (CanIPlaceItemOnTable(combinationRecipeCheck, mainTableTypeCheck))
                {
                    return 5;
                }
                else
                {
                    return 6;
                }
            }

            switch (mainTableTypeCheck)
            {
                case TableType.Normal:
                    break;
                case TableType.Spawner:
                    SpawnerTable spawnerTable = currentTableScript.GetComponent<SpawnerTable>();
                    if (handItemScript.itemID == spawnerTable.spawnItem)
                    {
                        Destroy(currentTableScript.heldTableItem);
                        currentTableScript.heldTableItem = null;
                        return 1;
                    }
                    Debug.Log("Cannot put down "+ handItemScript.itemID +" item on a "+ spawnerTable.spawnItem +" Spawner!");
                    return 0;
                case TableType.Trash:
                    Debug.LogError("Bin should NOT have an item in it");
                    return 0;
                case TableType.PlayerInteraction:
                    break;
                case TableType.ToggleActivation:
                    ToggleActivationTable toggleActivationTable = currentTableScript.GetComponent<ToggleActivationTable>();
                    if (toggleActivationTable.tableIsActive)
                        return 0;
                    break;
                case TableType.AlwaysActive:
                    break;
                case TableType.Delivery:
                    break;
            }

            Debug.Log("Table is full");
            return 0;
        } 
        else //There is nothing on the table, and something in the hands
        {
            if (!CanIPlaceItemOnTable(handItemScript.itemID, mainTableTypeCheck))
            {
                return 0;
            }
            switch (mainTableTypeCheck)
            {
                case TableType.Normal:
                    return 1;
                case TableType.Spawner:
                    Debug.LogError("Spawner should have an item in it");
                    return 0;
                case TableType.Trash:
                    if (isThereAPlate)
                    {
                        plate.ClearItems();
                        return 0;
                    }
                    return 1;
                case TableType.PlayerInteraction:
                    return 1;
                case TableType.ToggleActivation:
                    ToggleActivationTable toggleActivationTable = currentTableScript.GetComponent<ToggleActivationTable>();
                    if (toggleActivationTable.tableIsActive)
                    {
                        Debug.Log("This Table is Closed/Active");
                        return 0;
                    }
                    return 1;
                case TableType.AlwaysActive:
                    return 1;
                case TableType.Delivery:
                    return 1;
            }
        }

        Debug.LogError("Should be inaccessable");
        return 0;
    }

    public bool CanIPlaceItemOnTable(ItemID checkItemID, TableType checkTableType)
    {
        ItemData checkItem = itemDatabaseSO.GetItem(checkItemID);
        switch (checkTableType)
        {
            case TableType.Normal:
                if (!checkItem.placementData.normalTable)
                {
                    Debug.Log(checkItemID + " can not be placed on " + checkTableType);
                    return false;
                }
                return true;
            case TableType.Spawner:
                return false;
            case TableType.Trash:
                if (!checkItem.placementData.binTable)
                {
                    Debug.Log(checkItemID + " can not be placed on " + checkTableType);
                    return false;
                }
                return true;
            case TableType.PlayerInteraction:
                if (!checkItem.placementData.playerInteractionTable)
                {
                    Debug.Log(checkItemID + " can not be placed on " + checkTableType);
                    return false;
                }
                return true;
            case TableType.ToggleActivation:
                if (!checkItem.placementData.toggleActivationTable)
                {
                    Debug.Log(checkItemID + " not be placed on " + checkTableType);
                    return false;
                }
                return true;
            case TableType.AlwaysActive:
                if (!checkItem.placementData.alwaysActiveTable)
                {
                    Debug.Log(checkItemID + " can not be placed on " + checkTableType);
                    return false;
                }
                return true;
            case TableType.Delivery:
                if (!checkItem.placementData.deliveryTable)
                {
                    Debug.Log(checkItemID + " can not be placed on " + checkTableType);
                    return false;
                }
                return true;
        }
        Debug.LogError("Something went wrong when checking placement validity");
        return false;
    }

    public ItemID CombinationRecipeCheck(Items firstItemScript, Items secondItemScript)
    {
        for (int i = 0; i < secondItemScript.itemData.combineData.Count; i++)
        {
            if (secondItemScript.itemData.combineData[i].item == firstItemScript.itemID)
            {
                //Debug.Log(secondItemScript.itemID + " 1+ " + firstItemScript.itemID + " = " + secondItemScript.itemData.combineData[i].result);
                return secondItemScript.itemData.combineData[i].result;
            }
        }
        for (int i = 0; i < firstItemScript.itemData.combineData.Count; i++)
        {
            if (firstItemScript.itemData.combineData[i].item == secondItemScript.itemID)
            {
                //Debug.Log(firstItemScript.itemID + " 2+ " + secondItemScript.itemID + " = " + firstItemScript.itemData.combineData[i].result);
                return firstItemScript.itemData.combineData[i].result;
            }
        }
        return ItemID.None;
    }
}
