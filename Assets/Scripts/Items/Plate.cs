using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;

public class Plate : NetworkBehaviour
{
    /*/
    [SerializeField] public List<ItemID> itemTypeList;
    [SerializeField] private List<GameObject> itemGameobjectList;

    [SerializeField] private GameObject[] itemDisplayPoints;

    void Awake()
    {
        itemTypeList = new List<ItemID>();
    }

    public bool CanAddItem(Items newItem)
    {
        ItemID newItemID = newItem.itemData.itemID;
        //Is plate full
        //WIP
        
        //Can not be plated
        if (!newItem.itemDatabaseSO.GetItem(newItemID).canBePlated) return false;
        //Is also a plate
        if (newItemID == ItemID.Plate)
        {
            return false; 
        }
        //Is item type already on this plate
        return !itemTypeList.Contains(newItemID);
    }

    public bool IsEmpty()
    {
        if (itemTypeList.Count == 0) 
            return true;
        return false;
    }

    public void ClearItems()
    {
        itemTypeList.Clear();
        itemGameobjectList.Clear();
        UpdateDisplay();
    }

    public void AddItem(GameObject newItem)
    {
        Items itemScript = newItem.GetComponent<Items>();
        itemTypeList.Add(itemScript.itemID);
        itemGameobjectList.Add(newItem);
        itemTypeList.Sort();
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {

        for (int i = 0; i < itemDisplayPoints.Length; i++)
        {
            itemDisplayPoints[i].SetActive(false);
        }
        switch (itemTypeList.Count)
        {
            case 0:
                Debug.Log("Clear");
                break;
            case 1:

                SetOnPlate(itemDisplayPoints[0], itemGameobjectList[0]);
                break;
            case 2:
                SetOnPlate(itemDisplayPoints[1], itemGameobjectList[0]);
                SetOnPlate(itemDisplayPoints[2], itemGameobjectList[1]);
                break;
            case 3:
                SetOnPlate(itemDisplayPoints[3], itemGameobjectList[0]);
                SetOnPlate(itemDisplayPoints[4], itemGameobjectList[1]);
                SetOnPlate(itemDisplayPoints[5], itemGameobjectList[2]);
                break;
            case 4:
                SetOnPlate(itemDisplayPoints[6], itemGameobjectList[0]);
                SetOnPlate(itemDisplayPoints[7], itemGameobjectList[1]);
                SetOnPlate(itemDisplayPoints[8], itemGameobjectList[2]);
                SetOnPlate(itemDisplayPoints[9], itemGameobjectList[3]);
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
        
    }/*/
}
