using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DiningTable : MonoBehaviour
{
    private MainTableScript mainTableScript;
    OrderDatabaseSO orderDatabaseSO;
    private OrderManager orderManager;
    public SpriteRenderer spriteRenderer;
    [SerializeField] private List<IngridientID> customerOrder;

    
    void Start()
    {
        mainTableScript = GetComponent<MainTableScript>();
        orderManager = mainTableScript.masterManager.orderManager;
        orderManager.AddTableToManager(this);
        orderDatabaseSO = mainTableScript.masterManager.orderDatabaseSO;
    }

    public bool MatchingOrder(List<IngridientID> plateContents)
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
        NewOrder();
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

    public void NewOrder()
    {
        int randomOrder = Random.Range(0, orderDatabaseSO.orders.Count);
        OrderData orderData = orderDatabaseSO.orders[randomOrder];

        ClearOrder();
        if (orderData.sprite != null || spriteRenderer != null)
            spriteRenderer.sprite = orderData.sprite;
        for(int i = 0; i < orderData.ingridients.Count; i++)
        {
            customerOrder.Add(orderData.ingridients[i]);
        }
        customerOrder.Sort();
    }
}
