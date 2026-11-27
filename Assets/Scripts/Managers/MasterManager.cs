using UnityEngine;

public class MasterManager : MonoBehaviour
{
    [Header("Databases")]
    [SerializeField] internal ItemDatabaseSO itemDatabaseSO;
    [SerializeField] internal RecipeDatabaseSO recipeDatabaseSO;
    [SerializeField] internal OrderDatabaseSO orderDatabaseSO;

    [Header("Managers")]
    [SerializeField] internal OrderManager orderManager;
    [SerializeField] internal TableManager tableManager;

    [Header("Other")]
    [SerializeField] internal TableValidityChecker tableValidityChecker;
    [SerializeField] internal RecipeChecker recipeChecker;
}
