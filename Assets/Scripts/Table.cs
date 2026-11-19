using UnityEngine;
using System.Collections;
using Mono.Cecil.Cil;
using Unity.VisualScripting;

public enum TableType
{
    Normal,
    Spawner,
    Bin,
    ChoppingBoard,
    Oven,
    Stove,
}

public class Table : MonoBehaviour
{
    [Header("Table References")]
    public GameObject itemHoldPoint;
    public GameObject indicator;
    public ItemDatabaseSO itemDatabaseSO;

    [Header("Table Settings")]
    public TableType tableType;
    public ItemID spawnItem;
    private GameObject spawnItemPrefab;
    internal bool tableIsActive = false;
    public float progressCooldown = 0.25f;
    public float progressAmount = 0.25f;
    private float currentProgressCooldown;

    
    [Header("Table Inventory")]
    public GameObject heldTableItem;
    internal Items heldTableItemScript;

    void Start()
    {
        SetIndicatorActive(false);
        if (itemDatabaseSO.GetItem(spawnItem) != null)        
            spawnItemPrefab = itemDatabaseSO.GetItem(spawnItem).Prefab;
        if (spawnItemPrefab != null)
        {
            Destroy(heldTableItem);
            heldTableItem = null;
            if (heldTableItemScript != null) heldTableItemScript.currentTable = null;
            heldTableItemScript = null;
            heldTableItem = Instantiate(spawnItemPrefab, itemHoldPoint.transform.position, Quaternion.identity);
            heldTableItem.transform.parent = itemHoldPoint.transform;
            heldTableItemScript = heldTableItem.GetComponent<Items>();
            heldTableItemScript.currentTable = this;
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

    public bool CanPickup()
    {
        //Table type check
        if (tableType == TableType.Bin)
        {
            Debug.Log("Cannot pick up item from a " + tableType + "!");
            return false;
        }
        //Is item on table?
        if (!HasItemOnTable())
        {
            Debug.Log("No item to pick up");
            return false;
        }
        return true;
    }

    public bool CanPutDown(GameObject handItem)
    {
        Items handItemScript = handItem.GetComponent<Items>();
        //Table type check
        if (tableType == TableType.Spawner)
        {
            if (handItemScript.itemID == spawnItem)
            {
                Destroy(heldTableItem);
                heldTableItem = null;
                return true;
            }
            Debug.Log("Cannot put down this item on a Spawner!");
            return false;
        }
        //Is there space on table?
        if (HasItemOnTable())
        {
            //Is it a Plate
            if (heldTableItemScript.itemData.IsAPlate && handItemScript.itemData.IsAPlate)
            {
                return false; // Temp
                
                /*/ 
                //Plate Stacking
                Plate plate = heldTableItemScript.plateScript;
                if (plate.CanAddItem(handItemScript))
                {
                    return true;
                }
                else
                {
                    return false;
                }
                /*/
            } 
            else if (heldTableItemScript.itemData.IsAPlate)
            {
                Plate plate = heldTableItemScript.plateScript;
                if (plate.CanAddItem(handItemScript))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            } 
            else if (handItemScript.itemData.IsAPlate)
            {
                Plate plate = handItemScript.plateScript;
                if (plate.CanAddItem(heldTableItemScript))
                {
                    return true;
                }
                else
                {
                    return false;
                }
                
            }

            //Is part of recipe?
            if (IsPartOfCombinationRecipie(handItemScript) != ItemID.None) 
                return true;

            Debug.Log("Table is full");
            return false;
        }
        if (handItemScript.itemData.IsAPlate)
        {
            if (tableType == TableType.Bin)
            {
                
                Plate plate = handItemScript.plateScript;
                plate.ClearItems();
                return false;
            }
            if (tableType != TableType.Normal && tableType != TableType.ChoppingBoard)
            {
                Debug.Log("AAA");
                return false;
            }
        }
        
        return true;
    }

    public ItemID IsPartOfCombinationRecipie(Items handItemScript)
    {
        Debug.Log("BBB");
        for (int i = 0; i < heldTableItemScript.itemData.combineData.Count; i++)
        {
            if (heldTableItemScript.itemData.combineData[i].combineItem == handItemScript.itemID)
            {
                Debug.Log(heldTableItemScript.itemID + " 1+ " + handItemScript.itemID + " = " + heldTableItemScript.itemData.combineData[i].combineResult);
                return heldTableItemScript.itemData.combineData[i].combineResult;
            }
        }
        for (int i = 0; i < handItemScript.itemData.combineData.Count; i++)
        {
            if (handItemScript.itemData.combineData[i].combineItem == heldTableItemScript.itemID)
            {
                Debug.Log(handItemScript.itemID + " 2+ " + heldTableItemScript.itemID + " = " + handItemScript.itemData.combineData[i].combineResult);
                return handItemScript.itemData.combineData[i].combineResult;
            }
        }
        return ItemID.None;
    }

    public bool PutDownItem(GameObject item)
    {
        Items itemScript = item.GetComponent<Items>();
        if (HasItemOnTable())
        {
            ItemID combinationResult = IsPartOfCombinationRecipie(itemScript);

            //Is it a Plate
            if (heldTableItemScript.itemData.IsAPlate)
            {
                Plate plate = heldTableItemScript.plateScript;
                if (plate.CanAddItem(itemScript))
                {
                    plate.AddItem(item);
                }
                else
                {
                    Debug.LogError("WHAT ARE YOU DOING!");
                }
            } 
            else if (itemScript.itemData.IsAPlate)
            {
                Plate plate = itemScript.plateScript;
                if (plate.CanAddItem(heldTableItemScript))
                {
                    plate.AddItem(heldTableItem);
                    heldTableItem = item;
                }
                else
                {
                    Debug.LogError("YOU DUMBASS!");
                }
            } 
            else 
            //Combine Ingridients
            if (combinationResult == ItemID.None)
            {
                Debug.LogError("WTF");
                return true;
            }
            else
            {
                Destroy(heldTableItem);
                heldTableItem = null;
                heldTableItemScript = null;
                Destroy(item);
                GameObject newItemPrefab = itemDatabaseSO.GetItem(combinationResult).Prefab;
                heldTableItem = Instantiate(newItemPrefab, itemHoldPoint.transform.position, Quaternion.identity);
            }
        } 
        else
        {
            //Put item on table
            heldTableItem = item;
            
        }
        heldTableItem.transform.parent = itemHoldPoint.transform;
        heldTableItem.transform.localPosition = Vector3.zero;
        heldTableItem.transform.localRotation = Quaternion.identity;
        
        
        //Destroy item 
        if (tableType == TableType.Bin)
        {
            Destroy(heldTableItem);
            heldTableItem = null;
            return true;
        }

        //Get item script
        heldTableItemScript = heldTableItem.GetComponent<Items>();
        heldTableItemScript.currentTable = this;

        if (heldTableItemScript.itemData.IsAPlate)
        {
            if (tableType != TableType.Normal && tableType != TableType.ChoppingBoard)
            {
                return false;
            }
        }
        return true;
    }

    public void Pickup()
    {
        //Take item from table
        heldTableItem.transform.parent = null;
        heldTableItem = null;
        heldTableItemScript.ResetTimers();
        if (heldTableItemScript != null) heldTableItemScript.currentTable = null;
        heldTableItemScript = null;
        
        //Spawn new item
        if (tableType == TableType.Spawner)
        {
            heldTableItem = Instantiate(spawnItemPrefab, itemHoldPoint.transform.position, Quaternion.identity);
            heldTableItem.transform.parent = itemHoldPoint.transform;
            heldTableItemScript = heldTableItem.GetComponent<Items>();
            heldTableItemScript.currentTable = this;
            return;
        }
    }

    public void ProssesItemStart()
    {
        if (!HasItemOnTable())
        {
            return;
        }
        if (tableIsActive)
        {
            ProssesItemStop();
            return;
        }
        
        if (tableType == TableType.Oven)
        {
            if (heldTableItemScript.itemData.canBeCooked)
            {
                Debug.Log("Start Oven");
                tableIsActive = true;
                return;
            }
        }
    }

    void Update()
    {
        if (!HasItemOnTable())
        {
            return;
        }
        if (currentProgressCooldown > 0)
        {
            currentProgressCooldown -= Time.deltaTime;
        }
        if (tableIsActive)
        {
            if (!heldTableItemScript.itemData.canBeCooked)
            {
                ProssesItemStop();
                return;
            }
            if (tableType == TableType.Oven)
            {
                heldTableItemScript.currentCooktime += Time.deltaTime;
                heldTableItemScript.UpdateProgressBar(tableType);
            }
        }
        if (heldTableItemScript == null) return;
        if (tableType == TableType.Stove && heldTableItemScript.itemData.canBeCooked)
        {
            heldTableItemScript.currentCooktime += Time.deltaTime;
            heldTableItemScript.UpdateProgressBar(tableType);
        }
    }

    public void ProssesItemStop()
    {
        if (tableType == TableType.Oven)
        {
            Debug.Log("Stop Oven");
            tableIsActive = false;
        }
    }

    public void ProssesItemProgress()
    {
        if (!HasItemOnTable())
        {
            return;
        }
        if (tableType == TableType.ChoppingBoard)
        {
            if (heldTableItemScript.itemData.canBeChopped)
            {
                if (currentProgressCooldown <= 0)
                {
                    currentProgressCooldown = progressCooldown;
                    heldTableItemScript.currentChoptime += progressAmount;
                    heldTableItemScript.UpdateProgressBar(tableType);
                }
            }
            return;
        }
    }
}
