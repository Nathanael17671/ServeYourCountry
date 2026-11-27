using UnityEngine;

public class SpawnerTable : MonoBehaviour
{
    private MainTableScript mainTableScript;
    [SerializeField] internal ItemType spawnItemType;
    [SerializeField] internal IngridientID spawnIngridientID;
    [SerializeField] internal ContainerID spawnContainerID;
    private GameObject spawnItemPrefab;

    void Awake()
    {
        mainTableScript = GetComponent<MainTableScript>();
    }

    void Start()
    {     
        ItemDatabaseSO itemDatabaseSO = mainTableScript.masterManager.itemDatabaseSO;
        switch (spawnItemType)
        {
            case ItemType.Container:
                spawnItemPrefab = itemDatabaseSO.GetContainer(spawnContainerID).Prefab;
            break;
            case ItemType.Ingridient:
                spawnItemPrefab = itemDatabaseSO.GetIngridient(spawnIngridientID).Prefab;
            break;
        }
        if (spawnItemPrefab == null)
        {
            Debug.LogError("Error Source: SpawnerTable. Item in database is missing prefab OR spawnItemType:" + spawnItemType + " / spawnIngridientID: " + spawnIngridientID + " / spawnContainerID: " + spawnContainerID + " is incorrect");
        }
        SpawnNewItem();
    }

    public void SpawnNewItem()
    {
        //Remove current Item
        Destroy(mainTableScript.tableItemGameobject);
        mainTableScript.tableItemGameobject = null;
        mainTableScript.tableItemScript = null;

        //Create new Item
        mainTableScript.tableItemGameobject = Instantiate(spawnItemPrefab, mainTableScript.itemHoldPoint.transform.position, Quaternion.identity);
        mainTableScript.tableItemGameobject.transform.parent = mainTableScript.itemHoldPoint.transform;
        mainTableScript.tableItemScript = mainTableScript.tableItemGameobject.GetComponent<ItemManager>();
        mainTableScript.tableItemScript.currentTableScript = mainTableScript;
    
    }

    
}
