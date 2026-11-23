using System;
using System.Collections.Generic;
using UnityEngine;

public enum ItemID {
    None = 0,
    Plate = 1,
    [System.Obsolete] T_Burnt = 2,
    [System.Obsolete] T_Wheat = 3,
    [System.Obsolete] T_Bread = 4,
    [System.Obsolete] T_Toast = 5,
    [System.Obsolete] T_BurntWheat = 6,
    [System.Obsolete] T_SlicedBread = 7,
    ContrabandPart1 = 8,
    ContrabandPart2 = 9,
    Contraband = 10,
    Water = 11,
    Flour = 12,
    WetFlour = 13,
    Dough = 14,
    C_Dough = 15,
    Bread = 16,
    C_Bread = 17
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