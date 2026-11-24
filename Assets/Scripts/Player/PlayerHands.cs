using Unity.Netcode;
using UnityEngine;

public class PlayerHands : NetworkBehaviour
{
    public GameObject heldItem;
    public GameObject itemHoldPoint;
    private Collider currentTableCollider;
    private MainTableScript tableScript;
    private Items heldItemScript;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Table"))
        {
            currentTableCollider = other;
            tableScript = currentTableCollider.GetComponent<MainTableScript>();
            other.GetComponent<MainTableScript>().SetIndicatorActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other == currentTableCollider)
        {
            currentTableCollider = null;
            tableScript = null;
            other.GetComponent<MainTableScript>().SetIndicatorActive(false);
        }
    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (currentTableCollider != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Grab();
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                UsePress();
            }
            if (Input.GetKey(KeyCode.Q))
            {
                UseHold();
            }
        }
    }

    void Grab()
    {
        //Check if hands are empty
        if (heldItem == null)
        {
            if (tableScript.tableValidityChecker.CanPickup(tableScript))
            {
                //Move item from table to hands
                GameObject newHandItem = tableScript.heldTableItem;
                tableScript.Pickup();
                AddItemToHands(newHandItem);
            }
        }

        //Check if item in hands
        else if (heldItem != null)
        {
            // 0 = Do Nothing, 
            // 1 = Put on Table,
            // 2 = Put in Hand (Not in use), 
            // 3 = Move to Plate, put on Table, 
            // 4 = Move to Plate, put in Hand
            // 5 = Combine Items, and put on Table
            // 6 = Combine Items, and put in Hands
            GameObject newItem = heldItem;
            switch (tableScript.tableValidityChecker.CanPutDown(tableScript, heldItem))
            {
                case 0:
                break;
                case 1:
                    RemoveItemFromHands();
                    tableScript.PutDownItem(newItem, 0);
                break;
                case 2:
                    Debug.Log("idk man. you should not be able to get this yet");
                break;
                case 3:
                    RemoveItemFromHands();
                    tableScript.PutDownItem(newItem,1);
                    //Debug.Log("Can put down. PLATE!");
                break;
                case 4:
                    RemoveItemFromHands();
                    tableScript.PutDownItem(newItem,1);
                    newItem = tableScript.heldTableItem;
                    tableScript.Pickup();
                    AddItemToHands(newItem);
                    //Debug.Log("Can pick up. PLATE!");
                break;
                case 5:
                    RemoveItemFromHands();
                    tableScript.PutDownItem(newItem,2);
                    //Debug.Log("Can put down. Combination");
                break;
                case 6:
                    RemoveItemFromHands();
                    tableScript.PutDownItem(newItem,2);
                    newItem = tableScript.heldTableItem;
                    tableScript.Pickup();
                    AddItemToHands(newItem);
                    //Debug.Log("Can pick up. Combination");
                break;
            }
        }
    }

    void RemoveItemFromHands()
    {
        heldItem.transform.parent = null;
        heldItem = null;
        heldItemScript = null;
    }
    
    void AddItemToHands(GameObject item)
    {
        heldItem = item;
        heldItem.transform.position = itemHoldPoint.transform.position;
        heldItem.transform.rotation = itemHoldPoint.transform.rotation;
        heldItem.transform.parent = itemHoldPoint.transform;
        heldItemScript = heldItem.GetComponent<Items>();
    }

    void UsePress()
    {
        tableScript.ToggleActivation();
    }

    void UseHold()
    {
        tableScript.ProgressPlayerInteraction();
    }
}
