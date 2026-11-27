using UnityEngine;
using System.Collections.Generic;

public class ContainedItem
{
    [SerializeField] public IngridientID ingridientID;
    [SerializeField] public GameObject ingridientGameobject;
    [SerializeField] public bool ingridientTakeOut;
}

[RequireComponent(typeof(ItemManager))]
public class ContainerScript : MonoBehaviour
{
    public ContainerID containerID;

    [Header("Data")]
    internal ItemManager itemManager;
    internal ContainerData containerData;

    [Header("Location")]
    internal MainTableScript currentTableScript;

    [SerializeField] internal List<ContainedItem> containedItemList;

    [SerializeField] private GameObject[] ingridientDisplayPoints;

    public List<IngridientID> ingridientIDList
    {
        get
        {
            var ingridientIDs = new List<IngridientID>();
            foreach (ContainedItem containedItem in containedItemList)
            {
                ingridientIDs.Add(containedItem.ingridientID);
            }
            return ingridientIDs;
        }
    }


    void Awake()
    {
        containedItemList = new List<ContainedItem>();
        currentTableScript = null;
        itemManager = GetComponent<ItemManager>();
        containerData = itemManager.itemDatabaseSO.GetContainer(containerID);
    }

    public bool IsEmpty()
    {
        if (containedItemList.Count == 0) 
            return true;
        return false;
    }

    public void ClearItems()
    {
        foreach (ContainedItem containedItem in containedItemList)
        {
            Destroy(containedItem.ingridientGameobject);
        }
        containedItemList.Clear();
        UpdateDisplay();
    }

    public bool CanAddItem(ItemManager newItemScript)
    {
        if (newItemScript.itemType != ItemType.Ingridient)
            return false;
        
        IngridientID newItemID = newItemScript.ingridientScript.ingridientID;
        //Is plate full
        //WIP
        
        //Can not be plated
        if (!newItemScript.itemDatabaseSO.GetIngridient(newItemID).canBeStoredIn.Contains(containerID)) return false;
        //Is item type already on this plate
        foreach (ContainedItem containedItem in containedItemList)
        {
            if (containedItem.ingridientID == newItemID)
                return false;
        }
        return true;
    }

    public void AddItem(GameObject newItem)
    {
        ItemManager newItemScript = newItem.GetComponent<ItemManager>();
        IngridientID newItemID = newItemScript.ingridientScript.ingridientID;
        AddItem(newItem, !newItemScript.itemDatabaseSO.GetIngridient(newItemID).canBeStoredIn.Contains(containerID));
    }

    public void AddItem(GameObject newItem, bool canBeTakenOut)
    {
        ItemManager itemManager = newItem.GetComponent<ItemManager>();
        containedItemList.Add(new ContainedItem{
            ingridientID = itemManager.ingridientScript.ingridientID,
            ingridientGameobject = newItem,
            ingridientTakeOut = canBeTakenOut});
        containedItemList.Sort((a, b) => a.ingridientID.CompareTo(b.ingridientID));
        UpdateDisplay();
    }

    public GameObject TakeOutFirstAvailableItem()
    {
        for (int i = 0; i < containedItemList.Count; i++)
        {
            ContainedItem containedItem = containedItemList[i];
            if (!containedItem.ingridientTakeOut)
                continue;

            containedItemList.RemoveAt(i);
            GameObject ingridientGameobject = containedItem.ingridientGameobject;
            ingridientGameobject.transform.parent = null;
            UpdateDisplay();
            return ingridientGameobject;
        }
        return null;
    }

    void UpdateDisplay()
    {
        for (int i = 0; i < ingridientDisplayPoints.Length; i++)
        {
            ingridientDisplayPoints[i].SetActive(false);
        }
        switch (containedItemList.Count)
        {
            case 0:
                Debug.Log("Clear");
                break;
            case 1:

                SetOnPlate(ingridientDisplayPoints[0], containedItemList[0].ingridientGameobject);
                break;
            case 2:
                SetOnPlate(ingridientDisplayPoints[1], containedItemList[0].ingridientGameobject);
                SetOnPlate(ingridientDisplayPoints[2], containedItemList[1].ingridientGameobject);
                break;
            case 3:
                SetOnPlate(ingridientDisplayPoints[3], containedItemList[0].ingridientGameobject);
                SetOnPlate(ingridientDisplayPoints[4], containedItemList[1].ingridientGameobject);
                SetOnPlate(ingridientDisplayPoints[5], containedItemList[2].ingridientGameobject);
                break;
            case 4:
                SetOnPlate(ingridientDisplayPoints[6], containedItemList[0].ingridientGameobject);
                SetOnPlate(ingridientDisplayPoints[7], containedItemList[1].ingridientGameobject);
                SetOnPlate(ingridientDisplayPoints[8], containedItemList[2].ingridientGameobject);
                SetOnPlate(ingridientDisplayPoints[9], containedItemList[3].ingridientGameobject);
                break;
            default:
            Debug.Log("Plate is Overflowing");
            break;
        }
    }

    void SetOnPlate(GameObject point, GameObject item)
    {
        point.SetActive(true);
        item.transform.parent = point.transform;
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
        item.transform.localScale = Vector3.one;
        
    }
}
