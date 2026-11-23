using System;
using System.Collections.Generic;
using UnityEngine;

public enum ItemID {
    None = 0,
    Burnt = 1,
    Plate = 2,
    Wheat = 3,
    Bread = 4,
    Toast = 5,
    BurntWheat = 6
}

[CreateAssetMenu]
public class ItemDatabaseSO : ScriptableObject
{
    public List<ItemData> items;

    private Dictionary<ItemID, ItemData> _lookup;

    public ItemData GetItem(ItemID id)
    {
        if (_lookup == null)
        {
            _lookup = new Dictionary<ItemID, ItemData>();
            foreach (var item in items)
                _lookup[item.itemID] = item;
        }
        return _lookup.TryGetValue(id, out var data) ? data : null;
    }

    internal ItemDataMultiplayer GetItem(ItemIDMultiplayer itemIDMultiplayer)
    {
        throw new NotImplementedException();
    }
}

[Serializable]
public class ItemData
{
    public string Name;
    public ItemID itemID;
    public GameObject Prefab;
    [Header("Plate")]
    public bool IsAPlate;
    public bool canBePlated;
    [Header("Cook")]
    public bool canBeCooked = false;
    public CookData cookData;
    [Header("Chop")]
    public bool canBeChopped = false;
    public ChopData chopData;
    [Header("Combine")]
    public bool canBeCombined = false;
    public List<CombineData> combineData;

    public ItemIDMultiplayer ItemIDMultiplayer { get; internal set; }
}

[Serializable]
public class CookData
{
    public float cooktime;
    public ItemID cookResult;
}

[Serializable]
public class ChopData
{
    public float choptime;
    public ItemID chopResult;
}

[Serializable]
public class CombineData
{
    public ItemID combineItem;
    public ItemID combineResult;
}

    