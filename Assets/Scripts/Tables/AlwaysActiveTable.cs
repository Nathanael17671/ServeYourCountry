using System.Collections.Generic;
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
        if (!mainTableScript.HasItemOnTable()) return;
        if (!mainTableScript.workbenchScript.hasValidRecipe) return;

        mainTableScript.tableItemScript.currentTime += Time.deltaTime;
        mainTableScript.tableItemScript.UpdateProgressBar(WorkbenchType.AlwaysActive); 
        
    }
}
