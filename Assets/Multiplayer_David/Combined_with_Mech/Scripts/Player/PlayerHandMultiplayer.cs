using UnityEngine;
using Unity.Netcode;

public class PlayerHandMultiplayer : NetworkBehaviour
{
    public GameObject heldItem;
    public GameObject itemHoldPoint;
    private Collider currentTableCollider;
    private Table tableTypeMultiplayer;
    private Items heldItemScript;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Table"))
        {
            currentTableCollider = other;
            tableTypeMultiplayer = currentTableCollider.GetComponent<Table>();
            other.GetComponent<Table>().SetIndicatorActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other == currentTableCollider)
        {
            currentTableCollider = null;
            tableTypeMultiplayer = null;
            other.GetComponent<Table>().SetIndicatorActive(false);
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
                GrabServerRpc();
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

    [ServerRpc]
    void GrabServerRpc()
    {
        //Check if table is on
        if (tableTypeMultiplayer.tableIsActive)
        {
            return;
        }

        //Check if hands are empty
        if (heldItem == null)
        {
            if (tableTypeMultiplayer.CanPickup())
            {
                //Move item from table to hands
                GameObject newHandItem = tableTypeMultiplayer.heldTableItem;
                tableTypeMultiplayer.Pickup();
                AddItemToHands(newHandItem);
            }
        }

        //Check if item in hands
        else if (heldItem != null)
        {
            if (tableTypeMultiplayer.CanPutDown(heldItem))
            {
                //Move item from hands to table
                GameObject newTableItem = heldItem;
                RemoveItemFromHands();
                if (!tableTypeMultiplayer.PutDownItem(newTableItem))
                {
                    if (tableTypeMultiplayer.CanPickup())
                    {
                        //Move item from table to hands
                        GameObject newHandItem = tableTypeMultiplayer.heldTableItem;
                        tableTypeMultiplayer.Pickup();
                        AddItemToHands(newHandItem);
                    }
                }
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
        tableTypeMultiplayer.ProssesItemStart();
    }

    void UseHold()
    {
        tableTypeMultiplayer.ProssesItemProgress();
    }
}
