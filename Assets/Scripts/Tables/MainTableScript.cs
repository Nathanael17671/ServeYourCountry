using UnityEngine;

public enum TableType
{
    Normal,
    Spawner,
    Trash,
    PlayerInteraction,
    ToggleActivation,
    AlwaysActive,
    Delivery
}
public class MainTableScript : MonoBehaviour
{
    //[Header("Table References")]
    internal MasterManager masterManager;
    
    
    [Header("Table Settings")]
    [SerializeField] internal GameObject itemHoldPoint;
    [SerializeField] private GameObject indicator;
    [SerializeField] internal TableType mainTableType;
    
    //[Header("Table Inventory")]
    internal GameObject heldTableItem;
    internal Items heldTableItemScript;

    internal TableValidityChecker tableValidityChecker;
    private SpawnerTable spawnerTableScript;
    private TrashTable trashTableScript;
    private ToggleActivationTable toggleActivationTableScript;
    private PlayerInteractionTable playerInteractionTableScript;
    private AlwaysActiveTable alwaysActiveTableScript;
    private DiningTable diningTableScript;

    void Awake()
    {
        masterManager = FindAnyObjectByType<MasterManager>();
    }

    void Start()
    {
        SetIndicatorActive(false);
        tableValidityChecker = masterManager.tableValidityChecker;

        switch (mainTableType)
        {
            case TableType.Normal:
                break;
            case TableType.Spawner:
                spawnerTableScript = GetComponent<SpawnerTable>();
                break;
            case TableType.Trash:
                trashTableScript = GetComponent<TrashTable>();
                break;
            case TableType.PlayerInteraction:
                playerInteractionTableScript = GetComponent<PlayerInteractionTable>();
                break;
            case TableType.ToggleActivation:
                toggleActivationTableScript = GetComponent<ToggleActivationTable>();
                break;
            case TableType.AlwaysActive:
                alwaysActiveTableScript = GetComponent<AlwaysActiveTable>();
                break;
            case TableType.Delivery:
                diningTableScript = GetComponent<DiningTable>();
                break;
        }
    }

    public void SetIndicatorActive(bool isActive)
    {
        indicator.SetActive(isActive);
    }

    public bool HasItemOnTable()
    {
        if (heldTableItem == null || heldTableItemScript == null)
        {
            return false;
        }
        return true;
    }

    public void Pickup()
    {
        //Take item from table
        heldTableItem.transform.parent = null;
        heldTableItem = null;
        heldTableItemScript.ResetTimers();
        if (heldTableItemScript != null) heldTableItemScript.currentTableScript = null;
        heldTableItemScript = null;
        
        //Spawn new item
        if (mainTableType == TableType.Spawner)
        {
            spawnerTableScript.SpawnNewItem();
        }
    }

    public void PutDownItem(GameObject item, int action)
    {
        // 0 = Put on Table,
        // 1 = Move to Plate, put on Table, 
        // 2 = Combine Items, and put on Table

        Items itemScript = item.GetComponent<Items>();
        
        
        switch (action)
        {
            case 0:
                heldTableItem = item;
            break;
            case 1: //Is it a Plate
                if (heldTableItemScript.itemData.IsAPlate)
                {
                    Plate plate = heldTableItemScript.plateScript;
                    plate.AddItem(item);
                } 
                else if (itemScript.itemData.IsAPlate)
                {
                    Plate plate = itemScript.plateScript;
                    plate.AddItem(heldTableItem);
                    heldTableItem = item;
                } 
            break;
            case 2: //Combine Ingridients
                ItemID combinationResult = tableValidityChecker.CombinationRecipeCheck(itemScript, heldTableItemScript);
                Destroy(heldTableItem);
                heldTableItem = null;
                heldTableItemScript = null;
                Destroy(item);
                GameObject newItemPrefab = masterManager.itemDatabaseSO.GetItem(combinationResult).Prefab;
                heldTableItem = Instantiate(newItemPrefab, itemHoldPoint.transform.position, Quaternion.identity);
            break;
        }
        //Place item
        heldTableItem.transform.parent = itemHoldPoint.transform;
        heldTableItem.transform.localPosition = Vector3.zero;
        heldTableItem.transform.localRotation = Quaternion.identity;
        //Get item script
        heldTableItemScript = heldTableItem.GetComponent<Items>();
        heldTableItemScript.currentTableScript = this;

        //Destroy item if bin
        if (mainTableType == TableType.Trash)
        {
            Destroy(heldTableItem);
            heldTableItem = null;
            heldTableItemScript = null;
            return;
        }

        if (heldTableItemScript.itemData.IsAPlate)
        {
            if (mainTableType == TableType.Delivery)
            {
                Plate plate = heldTableItemScript.plateScript;
                if (diningTableScript.MatchingOrder(plate.itemTypeList))
                {
                    Debug.Log("Matching");
                    diningTableScript.OrderFulfilled(heldTableItem);
                    heldTableItem = null;
                    heldTableItemScript = null;
                    return;
                }
                else
                {
                    //Debug.Log("Not Matching");
                }
            }
        }
    }

    public void ToggleActivation()
    {
        if (mainTableType == TableType.ToggleActivation)
        {
            toggleActivationTableScript.ToggleProsses();
        }
    }

    public void ProgressPlayerInteraction()
    {
        if (!HasItemOnTable())
            return;
        if (!heldTableItemScript.itemData.usedInPlayerInteractionTable)
            return;
        if (mainTableType == TableType.PlayerInteraction)
        {
            playerInteractionTableScript.ProssesItemProgress();
        }

    }

}
