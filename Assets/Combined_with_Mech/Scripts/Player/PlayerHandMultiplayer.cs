using UnityEngine;
using Unity.Netcode;

public class PlayerHandMultiplayer : NetworkBehaviour
{
  /*/  public GameObject heldItem;
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
        //Check if table is on
        if (tableScript.tableIsActive)
        {
            return;
        }
        //Check if hands are empty
        if (heldItem == null)
        {
            if (tableScript.CanPickup())
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
            if (tableScript.CanPutDown(heldItem))
            {
                //Move item from hands to table
                GameObject newTableItem = heldItem;
                RemoveItemFromHands();
                if (!tableScript.PutDownItem(newTableItem))
                {
                    if (tableScript.CanPickup())
                    {
                        //Move item from table to hands
                        GameObject newHandItem = tableScript.heldTableItem;
                        tableScript.Pickup();
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
        tableScript.ProssesItemStart();
    }

    void UseHold()
    {
        tableScript.ProssesItemProgress();
    }/*/
}
