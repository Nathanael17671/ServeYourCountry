using UnityEngine;
using UnityEngine.UI;

public class Items : MonoBehaviour
{
    public ItemID itemID;

    [Header("Data")]
    public ItemDatabaseSO itemDatabaseSO;
    internal ItemData itemData;
    internal Plate plateScript;
    
    [Header("ProgressBar")]
    public Image fillImage;
    public GameObject progressBar;
    private float progressTimeOut;
    private float currentProgress;
    private float neededProgress;

    [Header("Location")]
    internal Table currentTable;
    internal float currentCooktime;
    internal float currentChoptime;

    void Awake()
    {
        ImportDatabaseInfo();
        if (itemData.IsAPlate)
        {
            plateScript = GetComponent<Plate>();
        }
        progressBar.SetActive(false);
        
    }

    void Update()
    {
        if (itemData.IsAPlate)
        {
            return;
        }

        if (progressTimeOut >= 0)
        {
            progressTimeOut -= Time.deltaTime;
        } else 
        {
            ResetTimers();
        }
        CheckIngridientProgress();
    }

    void CheckIngridientProgress()
    {
        if (currentChoptime >= itemData.chopData.choptime && itemData.canBeChopped)
        {
            ChangeIngridient(TableType.ChoppingBoard);
        }
        else if (currentCooktime >= itemData.cookData.cooktime && itemData.canBeCooked)
        {
            ChangeIngridient(TableType.Oven);
        }
    }

    void ChangeIngridient(TableType tableType)
    {
        ItemData newIngridient = null;
        if (tableType == TableType.ChoppingBoard)
        {
            newIngridient = itemDatabaseSO.GetItem(itemData.chopData.chopResult);
        } 
        else if (tableType == TableType.Oven)
        {
            newIngridient = itemDatabaseSO.GetItem(itemData.cookData.cookResult);
        } 
        
        if (newIngridient == null) 
        {
            Debug.LogError("Missing asset");
            return;
        }
        currentTable.heldTableItem = Instantiate(newIngridient.Prefab, currentTable.itemHoldPoint.transform.position, Quaternion.identity);
        
        currentTable.heldTableItem.transform.parent = currentTable.itemHoldPoint.transform;
        currentTable.heldTableItemScript = currentTable.heldTableItem.GetComponent<Items>();
        currentTable.heldTableItemScript.currentTable = currentTable;
        Destroy(gameObject);
    }

    public void UpdateProgressBar(TableType tableType)
    {
        if (itemData.IsAPlate)
        {
            Debug.LogError("NOOOO");
            return;
        }
        Quaternion lookRotation = Camera.main.transform.rotation;
        progressBar.transform.rotation = lookRotation;

        if (tableType == TableType.ChoppingBoard)
        {
            currentProgress = currentChoptime;
            neededProgress = itemData.chopData.choptime;
        }
        else if (tableType == TableType.Oven || tableType == TableType.Stove )
        {
            currentProgress = currentCooktime;
            neededProgress = itemData.cookData.cooktime;
        }

        if (progressBar == null) return;
        if (currentProgress / neededProgress > 0)
        {
            progressBar.SetActive(true);
            fillImage.fillAmount = currentProgress / neededProgress;
        } else progressBar.SetActive(false);
        progressTimeOut = 2;
    }

    public void ResetTimers()
    {
        currentCooktime = 0f;
        currentChoptime = 0f;
        progressBar.SetActive(false);
    }

    void ImportDatabaseInfo()
    {
        itemData = itemDatabaseSO.GetItem(itemID);
    }
}
