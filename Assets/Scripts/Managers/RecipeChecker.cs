using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class RecipeChecker : MonoBehaviour
{
    [SerializeField] internal MasterManager masterManager;
    private RecipeDatabaseSO recipeDatabaseSO;

    void Awake()
    {
        //Sort Ingridients in Recipe Database
        recipeDatabaseSO = masterManager.recipeDatabaseSO;
        ItemDatabaseSO itemDatabaseSO = masterManager.itemDatabaseSO;
        for(int i = 0; i < recipeDatabaseSO.workbenches.Count; i++)
        {
            for(int j = 0; j < recipeDatabaseSO.workbenches[i].recipes.Count; j++)
            {
                for(int k = 0; k < recipeDatabaseSO.workbenches[i].recipes.Count; k++)
                {
                    recipeDatabaseSO.workbenches[i].recipes[j].ingridients.Sort();
                }
            }
        }
    }

    public RecipeData CheckWorkbenchRecipe(WorkbenchName workbench, List<IngridientID> ingridients, ContainerID containerID)
    {
        ingridients.Sort();
        RecipeData recipeData = masterManager.recipeDatabaseSO.GetWorkbench(workbench).GetRecipe(ingridients);
        if (recipeData == null) return null;
        if (recipeData.neededContainer == containerID)
        {
            return recipeData;
        }
        return null;   
    }

    public IngridientID CombinationRecipeCheck(IngridientScript firstIngridientScript, IngridientScript secondIngridientScript)
    {
        if (firstIngridientScript == null || secondIngridientScript == null) return IngridientID.None;
        for (int i = 0; i < recipeDatabaseSO.combines.Count; i++)
        {
            if (recipeDatabaseSO.combines[i].item_1 == firstIngridientScript.ingridientID && recipeDatabaseSO.combines[i].item_2 == secondIngridientScript.ingridientID)
            {
                return recipeDatabaseSO.combines[i].result;
            } 
            else if (recipeDatabaseSO.combines[i].item_2 == firstIngridientScript.ingridientID && recipeDatabaseSO.combines[i].item_1 == secondIngridientScript.ingridientID)
            {
                return recipeDatabaseSO.combines[i].result;
            } 
        }
        return IngridientID.None;
    }
}
