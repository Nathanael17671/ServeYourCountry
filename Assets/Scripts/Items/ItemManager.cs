using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    public ItemType itemType;

    [Header("Data")]
    [SerializeField] internal ItemDatabaseSO itemDatabaseSO;
    internal MasterManager masterManager;
    internal IngridientScript ingridientScript;
    internal ContainerScript containerScript;

    [Header("ProgressBar")]
    [SerializeField] private Image fillImage;
    [SerializeField] private GameObject progressBar;
    private float progressBarTimeOut = 2;
    private float currentProgressBarTimeOut;
    private float currentBarFill;
    private float neededBarFill;

    [Header("Location")]
    internal MainTableScript currentTableScript;
    internal float currentTime;
    private RecipeData currentRecipeData;

    void Awake()
    {
        masterManager = FindAnyObjectByType<MasterManager>();
        currentTableScript = null;
        progressBar.SetActive(false);
        switch (itemType)
        {
            case ItemType.None:
                Debug.LogError("No Itemtype Selected");
            break;
            case ItemType.Container:
                containerScript = GetComponent<ContainerScript>();
            break;
            case ItemType.Ingridient:
                ingridientScript = GetComponent<IngridientScript>();
            break;
        }
    }

    void Update()
    {
        if (currentTableScript == null) return;
        if (currentTableScript.tableType != TableType.Workbench) return;

        //Is there a valid recipe in the recipe database
        
        if (currentRecipeData == null) return;
        
        if (currentProgressBarTimeOut >= 0)
        {
            currentProgressBarTimeOut -= Time.deltaTime;
        } 
        else 
        {
            ResetTimers();
        }
        if (currentTime < currentRecipeData.time)
        {
            return;
        }
        
        IngridientData newIngridient = null;
        newIngridient = itemDatabaseSO.GetIngridient(currentRecipeData.result);
        
        if (newIngridient == null) 
        {
            Debug.LogError("Missing result asset");
            return;
        }
        GameObject resultGameObject = Instantiate(newIngridient.Prefab, currentTableScript.itemHoldPoint.transform.position, Quaternion.identity);
        switch (itemType)
        {
            case ItemType.Ingridient:
                currentTableScript.tableItemGameobject = resultGameObject;
                currentTableScript.tableItemGameobject.transform.parent = currentTableScript.itemHoldPoint.transform;
                currentTableScript.tableItemScript = currentTableScript.tableItemGameobject.GetComponent<ItemManager>();
                currentTableScript.tableItemScript.currentTableScript = currentTableScript;
                currentRecipeData = null;
                currentTableScript.workbenchScript.CheckHeldItemRecipe();
                Destroy(gameObject);
            break;
            case ItemType.Container:
                containerScript.ClearItems();
                containerScript.AddItem(resultGameObject);
                currentRecipeData = null;
                ResetTimers();
                currentTableScript.workbenchScript.CheckHeldItemRecipe();
            break;
        }
        
    }

    public void UpdateRecipeData(RecipeData newRecipeData)
    {
        currentRecipeData = newRecipeData;
    }

    public void UpdateProgressBar(WorkbenchType workbenchType)
    {
        if (progressBar == null) return;
        Quaternion lookRotation = Camera.main.transform.rotation;
        progressBar.transform.rotation = lookRotation;

        currentBarFill = currentTime;
        neededBarFill = currentRecipeData.time;
        if (currentBarFill / neededBarFill > 0)
        {
            progressBar.SetActive(true);
            fillImage.fillAmount = currentBarFill / neededBarFill;
        } else progressBar.SetActive(false);
        currentProgressBarTimeOut = progressBarTimeOut;
    }

    public void ResetTimers()
    {
        currentTime = 0f;
        progressBar.SetActive(false);
    }
}
