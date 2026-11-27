using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ItemManager))]
public class IngridientScript : MonoBehaviour
{
    public IngridientID ingridientID;

    [Header("Data")]
    internal ItemManager itemManager;
    internal IngridientData ingridientData;
    
    

    void Awake()
    {
        itemManager = GetComponent<ItemManager>();
        ingridientData = itemManager.itemDatabaseSO.GetIngridient(ingridientID);
    }

    
}
