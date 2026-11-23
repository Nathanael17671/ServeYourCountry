using UnityEngine;

public class AlwaysActiveTable : MonoBehaviour
{
    private MainTableScript mainTableScript;

    void Awake()
    {
        mainTableScript = GetComponent<MainTableScript>();
    }

    void Update()
    {
        if (!mainTableScript.HasItemOnTable())
        {
            return;
        }
        
        if (mainTableScript.heldTableItemScript.itemData.usedInAlwaysActiveTable)
        {
            mainTableScript.heldTableItemScript.currentAlwaysActiveTime += Time.deltaTime;
            mainTableScript.heldTableItemScript.UpdateProgressBar(mainTableScript.mainTableType);
        }
    }
}
