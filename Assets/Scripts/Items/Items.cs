using UnityEngine;
using UnityEngine.UI;

public class Items : MonoBehaviour
{
    public ItemID itemID;

    [Header("Data")]
    [SerializeField] internal ItemDatabaseSO itemDatabaseSO;
    internal ItemData itemData;
    internal Plate plateScript;
    
    [Header("ProgressBar")]
    [SerializeField] private Image fillImage;
    [SerializeField] private GameObject progressBar;
    private float progressBarTimeOut = 2;
    private float currentProgressBarTimeOut;
    private float currentBarFill;
    private float neededBarFill;

    [Header("Location")]
    internal MainTableScript currentTableScript;
    internal float currentToggleActivationTime;
    internal float currentPlayerInteractionTime;
    internal float currentAlwaysActiveTime;

    void Awake()
    {
        currentTableScript = null;
        itemData = itemDatabaseSO.GetItem(itemID);
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

        if (currentProgressBarTimeOut >= 0)
        {
            currentProgressBarTimeOut -= Time.deltaTime;
        } 
        else 
        {
            ResetTimers();
        }
        CheckIngridientProgress();
    }

    void CheckIngridientProgress()
    {
        if (currentAlwaysActiveTime >= itemData.alwaysActiveData.time && itemData.usedInAlwaysActiveTable)
        {
            ChangeIngridient(TableType.AlwaysActive);
        }
        else if (currentPlayerInteractionTime >= itemData.playerInteractionData.time && itemData.usedInPlayerInteractionTable)
        {
            ChangeIngridient(TableType.PlayerInteraction);
        } 
        else if (currentToggleActivationTime >= itemData.toggleActivationData.time && itemData.usedInToggleActivationTable)
        {
            ChangeIngridient(TableType.ToggleActivation);
        }
    }

    void ChangeIngridient(TableType tableType)
    {
        ItemData newIngridient = null;
        switch (tableType)
        {
            case TableType.AlwaysActive:
                newIngridient = itemDatabaseSO.GetItem(itemData.alwaysActiveData.result);
            break;
            case TableType.PlayerInteraction:
                newIngridient = itemDatabaseSO.GetItem(itemData.playerInteractionData.result);
            break;
            case TableType.ToggleActivation:
                newIngridient = itemDatabaseSO.GetItem(itemData.toggleActivationData.result);
            break;
        }
        
        if (newIngridient == null) 
        {
            Debug.LogError("Missing result asset");
            return;
        }
        currentTableScript.heldTableItem = Instantiate(newIngridient.Prefab, currentTableScript.itemHoldPoint.transform.position, Quaternion.identity);
        
        currentTableScript.heldTableItem.transform.parent = currentTableScript.itemHoldPoint.transform;
        currentTableScript.heldTableItemScript = currentTableScript.heldTableItem.GetComponent<Items>();
        currentTableScript.heldTableItemScript.currentTableScript = currentTableScript;
        Destroy(gameObject);
    }

    public void UpdateProgressBar(TableType tableType)
    {
        if (progressBar == null) return;
        if (itemData.IsAPlate)
        {
            Debug.LogError("NOOOO");
            return;
        }
        Quaternion lookRotation = Camera.main.transform.rotation;
        progressBar.transform.rotation = lookRotation;

        switch (tableType)
        {
            case TableType.AlwaysActive:
                currentBarFill = currentAlwaysActiveTime;
                neededBarFill = itemData.alwaysActiveData.time;
            break;
            case TableType.PlayerInteraction:
                currentBarFill = currentPlayerInteractionTime;
                neededBarFill = itemData.playerInteractionData.time;
            break;
            case TableType.ToggleActivation:
                currentBarFill = currentToggleActivationTime;
                neededBarFill = itemData.toggleActivationData.time;
            break;
        }

        if (currentBarFill / neededBarFill > 0)
        {
            progressBar.SetActive(true);
            fillImage.fillAmount = currentBarFill / neededBarFill;
        } else progressBar.SetActive(false);
        currentProgressBarTimeOut = progressBarTimeOut;
    }

    public void ResetTimers()
    {
        currentAlwaysActiveTime = 0f;
        currentPlayerInteractionTime = 0f;
        currentToggleActivationTime = 0f;
        progressBar.SetActive(false);
    }

}
