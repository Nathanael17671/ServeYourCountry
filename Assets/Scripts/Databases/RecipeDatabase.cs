using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum WorkbenchName
{
    None = 0,
    Oven = 1,
    Counter = 2,
    Stove = 3,
    Mixer = 4,
    ToolBench = 5,
}

[CreateAssetMenu(menuName = "Resturant/RecipeDatabaseSO", order = 2)]
public class RecipeDatabaseSO : ScriptableObject
{
    public List<WorkbenchData> workbenches;
    public List<CombineData> combines;

    private Dictionary<WorkbenchName, WorkbenchData> _W_lookup;

    public WorkbenchData GetWorkbench(WorkbenchName id)
    {
        if (_W_lookup == null)
        {
            _W_lookup = new Dictionary<WorkbenchName, WorkbenchData>();
            foreach (var workbench in workbenches)
                _W_lookup[workbench.workbenchName] = workbench;
        }
        return _W_lookup.TryGetValue(id, out var data) ? data : null;
    }
}

[Serializable]
public class WorkbenchData
{
    public string Name;
    public WorkbenchName workbenchName;
    public WorkbenchType workbenchID;
    [TextArea(3, 10)] public string description;
    [Header("Recipes")]
    public List<RecipeData> recipes;

    public RecipeData GetRecipe(List<IngridientID> id)
    {
        List<IngridientID> sortedId = id.OrderBy(ingredient => ingredient).ToList();
        foreach (RecipeData recipe in recipes)
        {
            if (recipe.ingridients.Count == sortedId.Count &&
                recipe.ingridients.OrderBy(ingredient => ingredient).SequenceEqual(sortedId))
            {
                return recipe;
            }
        }
        return null;
    }
}

[Serializable]
public class RecipeData
{
    public string Name;
    [Header("Ingridients")]
    public ContainerID neededContainer;
    public List<IngridientID> ingridients; 
    public float time = 5f;
    public IngridientID result;
}

[Serializable]
public class CombineData
{
    public string Name;
    public IngridientID item_1;
    public IngridientID item_2;
    public IngridientID result;
}