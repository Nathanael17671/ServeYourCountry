using System.Collections.Generic;
using UnityEngine;

public class ToggleActivationTable : MonoBehaviour
{
    private MainTableScript mainTableScript;
    internal bool tableIsActive = false;
    [SerializeField] private GameObject tableActiveDisplay;
    [SerializeField] private GameObject tableInactiveDisplay;

    void Awake()
    {
        mainTableScript = GetComponent<MainTableScript>();
    }

    void Start()
    {
        if (tableActiveDisplay != null)
            tableActiveDisplay.SetActive(false);
        if (tableInactiveDisplay != null)
            tableInactiveDisplay.SetActive(true);
    }

    public void ToggleProsses()
    {
        if (tableIsActive)
        {
            //Debug.Log("OFF");
            ProssesToggleOff();
        }
        else
        {
            //Debug.Log("ON");
            ProssesToggleOn();
        }
    }

    void ProssesToggleOn()
    {
        tableActiveDisplay.SetActive(true);
        tableInactiveDisplay.SetActive(false);
        tableIsActive = true;
        mainTableScript.workbenchScript.canPickupPutDown = false;
    }


    public void ProssesToggleOff()
    {
        tableActiveDisplay.SetActive(false);
        tableInactiveDisplay.SetActive(true);
        tableIsActive = false;
        mainTableScript.workbenchScript.canPickupPutDown = true;
    }

    void Update()
    {
        if (!tableIsActive) return;
        if (!mainTableScript.HasItemOnTable()) return;
        if (!mainTableScript.workbenchScript.hasValidRecipe)
        {
            ProssesToggleOff();
            return;
        }
        mainTableScript.tableItemScript.currentTime += Time.deltaTime;
        mainTableScript.tableItemScript.UpdateProgressBar(WorkbenchType.ToggleActivation);        
    }
}
