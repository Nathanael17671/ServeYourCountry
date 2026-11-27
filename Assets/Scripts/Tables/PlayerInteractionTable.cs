using UnityEngine;

public class PlayerInteractionTable : MonoBehaviour
{
    private MainTableScript mainTableScript;
    public float progressCooldown = 0.25f;
    public float progressAmount = 0.25f;
    private float currentProgressCooldown;

    void Awake()
    {
        mainTableScript = GetComponent<MainTableScript>();
    }

    public void ProssesItemProgress()
    {
        if (!mainTableScript.workbenchScript.hasValidRecipe) return;
        
        if (currentProgressCooldown <= 0)
        {
            currentProgressCooldown = progressCooldown;
            mainTableScript.tableItemScript.currentTime += progressAmount;
            mainTableScript.tableItemScript.UpdateProgressBar(WorkbenchType.PlayerInteraction);
        }
    }

    void Update()
    {
        if (currentProgressCooldown > 0)
            currentProgressCooldown -= Time.deltaTime;
    }
}
