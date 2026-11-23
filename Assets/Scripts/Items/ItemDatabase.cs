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
    BurntWheat = 6,
    SlicedBread = 7
}

[CreateAssetMenu]
public class ItemDatabaseSO : ScriptableObject
{
    public List<ItemData> items;
    public List<OrderData> orders;

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
}

[Serializable]
public class ItemData
{
    public string Name;
    public ItemID itemID;
    public GameObject Prefab;
    [Header("Placement")]
    public PlacementData placementData;
    [Header("Plate")]
    public bool IsAPlate;
    public bool canBePlated;
    [Header("AlwaysActive")]
    public bool usedInAlwaysActiveTable = false;
    public ProssesItemData alwaysActiveData;
    [Header("PlayerInteraction")]
    public bool usedInPlayerInteractionTable = false;
    public ProssesItemData playerInteractionData;
    [Header("ToggleActivation")]
    public bool usedInToggleActivationTable = false;
    public ProssesItemData toggleActivationData;
    [Header("Combine")]
    public bool canBeCombined = false;
    public List<CombineData> combineData;
}

[Serializable]
public class PlacementData
{
    public bool normalTable = true;
    public bool binTable = true;
    public bool playerInteractionTable = true;
    public bool toggleActivationTable = true;
    public bool alwaysActiveTable = true;
    public bool deliveryTable = true;
}

[Serializable]
public class ProssesItemData
{
    public float time = 5f;
    public ItemID result;
}

[Serializable]
public class CombineData
{
    public ItemID item;
    public ItemID result;
}

[Serializable]
public class OrderData
{
    public string Name;
    public List<ItemID> ingridients;
}