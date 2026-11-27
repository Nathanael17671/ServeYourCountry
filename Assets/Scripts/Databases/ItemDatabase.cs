using System;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    None = 0,
    Ingridient = 1,
    Container = 2
    
}

public enum ContainerID
{
    None = 0,
    DeliveryPlate = 1,
    MixerBowl = 2,
}

public enum IngridientID 
{
    None = 0,
    ContrabandPart1 = 1,
    ContrabandPart2 = 2,
    ContrabandPart1n2 = 3,
    Water = 4,
    Flour = 5,
    Contraband = 6,
    Dough = 7,
    C_Dough = 8,
    Bread = 9,
    C_Bread = 10,
    UnbeatDough = 11,
    Burned = 12,
}

[CreateAssetMenu(menuName = "Resturant/ItemDatabaseSO", order = 1)]
public class ItemDatabaseSO : ScriptableObject
{
    public List<ContainerData> containers;
    public List<IngridientData> ingridients;

    private Dictionary<IngridientID, IngridientData> _I_lookup;
    private Dictionary<ContainerID, ContainerData> _C_lookup;


    public IngridientData GetIngridient(IngridientID id)
    {
        if (_I_lookup == null)
        {
            _I_lookup = new Dictionary<IngridientID, IngridientData>();
            foreach (var ingridient in ingridients)
                _I_lookup[ingridient.ingridientID] = ingridient;
        }
        return _I_lookup.TryGetValue(id, out var data) ? data : null;
    }

    public ContainerData GetContainer(ContainerID id)
    {
        if (_C_lookup == null)
        {
            _C_lookup = new Dictionary<ContainerID, ContainerData>();
            foreach (var container in containers)
                _C_lookup[container.containerID] = container;
        }
        return _C_lookup.TryGetValue(id, out var data) ? data : null;
    }
}

[Serializable]
public class IngridientData
{
    public string Name;
    public IngridientID ingridientID;
    public GameObject Prefab;
    [Header("Placement")]
    public PlacementData placementData;
    public List<ContainerID> canBeStoredIn;
}

[Serializable]
public class PlacementData
{
    public bool normalTable = true;
    public bool binTable = true;
    public bool deliveryTable = true;
    [Header("Workbenches")]
    public bool oven = false;
    public bool counter = true;
    public bool stove = false;
    public bool mixer = false;
    public bool toolBench = false;
}

[Serializable]
public class ContainerData
{
    public string Name;
    public ContainerID containerID;
    public GameObject Prefab;
    [Header("Placement")]
    public bool isForDelivery;
    public int storageSpace = 4;
    public PlacementData placementData;
}