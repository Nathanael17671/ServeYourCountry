using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Resturant/OrderDatabaseSO", order = 3)]
public class OrderDatabaseSO : ScriptableObject
{
    public List<OrderData> orders;
}

[Serializable]
public class OrderData
{
    public string Name;
    public Sprite sprite;
    public List<IngridientID> ingridients;
}