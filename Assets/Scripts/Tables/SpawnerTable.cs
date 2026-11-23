using UnityEngine;

public class SpawnerTable : MonoBehaviour
{
    private MainTableScript mainTableScript;
    [SerializeField] internal ItemID spawnItem;
    private GameObject spawnItemPrefab;

    void Awake()
    {
        mainTableScript = GetComponent<MainTableScript>();
    }

    void Start()
    {     
        spawnItemPrefab = mainTableScript.masterManager.itemDatabaseSO.GetItem(spawnItem).Prefab;
        if (spawnItemPrefab == null)
        {

            Debug.LogError("Error Source: SpawnerTable. Item in database is missing prefab OR spawnItem:" + spawnItem + " is incorrect");
        }
        SpawnNewItem();
    }

    public void SpawnNewItem()
    {
        //Remove current Item
        Destroy(mainTableScript.heldTableItem);
        mainTableScript.heldTableItem = null;
        mainTableScript.heldTableItemScript = null;

        //Create new Item
        mainTableScript.heldTableItem = Instantiate(spawnItemPrefab, mainTableScript.itemHoldPoint.transform.position, Quaternion.identity);
        mainTableScript.heldTableItem.transform.parent = mainTableScript.itemHoldPoint.transform;
        mainTableScript.heldTableItemScript = mainTableScript.heldTableItem.GetComponent<Items>();
        mainTableScript.heldTableItemScript.currentTableScript = mainTableScript;
    
    }

    
}
