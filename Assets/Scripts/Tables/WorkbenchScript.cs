using System.Collections.Generic;
using UnityEngine;

public enum WorkbenchType
{
    None,
    PlayerInteraction,
    ToggleActivation,
    AlwaysActive
}

[RequireComponent(typeof(MainTableScript))]
public class WorkbenchScript : MonoBehaviour
{
    private MainTableScript mainTableScript;
    [SerializeField] private WorkbenchName workbenchName;
    [SerializeField] internal WorkbenchType workbenchType;
    private WorkbenchData workbenchRecipeData;
    internal bool hasValidRecipe;
    internal bool canPickupPutDown = true;

    private ToggleActivationTable toggleActivationTableScript;
    private PlayerInteractionTable playerInteractionTableScript;
    private AlwaysActiveTable alwaysActiveTableScript;
    
    void Start()
    {
        mainTableScript = GetComponent<MainTableScript>();
        workbenchRecipeData = mainTableScript.masterManager.recipeDatabaseSO.GetWorkbench(workbenchName);
        switch (workbenchType)
        {
            case WorkbenchType.None:
                break;
            case WorkbenchType.PlayerInteraction:
                playerInteractionTableScript = GetComponent<PlayerInteractionTable>();
                break;
            case WorkbenchType.ToggleActivation:
                toggleActivationTableScript = GetComponent<ToggleActivationTable>();
                break;
            case WorkbenchType.AlwaysActive:
                alwaysActiveTableScript = GetComponent<AlwaysActiveTable>();
                break;
        }
    }

    public bool CheckPlacementDataWorkbench(PlacementData placementData)
    {
        switch (workbenchName)
        {
            case WorkbenchName.Oven:
                return placementData.oven;
            case WorkbenchName.Counter:
                return placementData.counter;
            case WorkbenchName.Stove:
                return placementData.stove;
            case WorkbenchName.Mixer:
                return placementData.mixer;
            case WorkbenchName.ToolBench:
                return placementData.toolBench;
        }
        Debug.LogError("Workbench Type Not Found");
        return false;
    }

    public void CheckHeldItemRecipe()
    {
        hasValidRecipe = false;
        List<IngridientID> ingridientIDList = null;
        RecipeData recipeData = null;
        switch (mainTableScript.tableItemScript.itemType)
        {
            case ItemType.Ingridient:
                ingridientIDList = new List<IngridientID>{mainTableScript.tableItemScript.ingridientScript.ingridientID};
                recipeData = mainTableScript.masterManager.recipeChecker.CheckWorkbenchRecipe(workbenchName, ingridientIDList, ContainerID.None);
            break;
            case ItemType.Container:
                ingridientIDList = mainTableScript.tableItemScript.containerScript.ingridientIDList;
                recipeData = mainTableScript.masterManager.recipeChecker.CheckWorkbenchRecipe(workbenchName, ingridientIDList, mainTableScript.tableItemScript.containerScript.containerID);
            break;
        }
        
        if (recipeData == null)
        {
            //Debug.Log("Invalid");
        }

        if (recipeData != null)
        {
            hasValidRecipe = true;
            //Debug.Log("Valid");
            mainTableScript.tableItemScript.UpdateRecipeData(recipeData);
        }
    }

    public void ToggleActivation()
    {
        if (workbenchType == WorkbenchType.ToggleActivation)
        {
            toggleActivationTableScript.ToggleProsses();
        }
    }

    public void ProssesWorkbench()
    {
        if (mainTableScript.tableItemGameobject == null)
            return;
        if (workbenchType == WorkbenchType.PlayerInteraction)
        {
            playerInteractionTableScript.ProssesItemProgress();
        }
    }
}
