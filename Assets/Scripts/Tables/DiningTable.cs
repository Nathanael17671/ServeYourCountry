using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DiningTable : MonoBehaviour
{
    private MainTableScript mainTableScript;
    private OrderManager orderManager;
    [SerializeField] private List<ItemID> customerOrder;
    
    void Start()
    {
        mainTableScript = GetComponent<MainTableScript>();
        orderManager = mainTableScript.masterManager.orderManager;
        orderManager.AddTableToManager(this);
    }

    public bool MatchingOrder(List<ItemID> plateContents)
    {
        if (!customerOrder.SequenceEqual(plateContents))
        {
            //Debug.Log(customerOrder + " != " + plateContents);
            return false;
        }
        //Debug.Log(customerOrder + " == " + plateContents);
        return true;
    }

    public void OrderFulfilled(GameObject fullfilledItem)
    {
        StartCoroutine(FloatToSky(fullfilledItem));
    }

    IEnumerator FloatToSky(GameObject floater)
    {
        Vector3 currentPosition = new Vector3(0, 0.05f, 0);
        int i = 0; 
        while (i < 1000)
        {
            floater.transform.localPosition += currentPosition;
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(10);
        Destroy(floater);
    }

    public void ClearOrder()
    {
        customerOrder.Clear();
    }

    public void NewOrder(OrderData orderData)
    {
        ClearOrder();
        for(int i = 0; i < orderData.ingridients.Count; i++)
        {
            customerOrder.Add(orderData.ingridients[i]);
        }
        customerOrder.Sort();
    }
}
