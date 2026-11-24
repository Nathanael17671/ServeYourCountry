using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class PlayerInGameMenu : NetworkBehaviour
{
    [SerializeField] private GameObject inGameMenu;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !inGameMenu.activeInHierarchy)
        {
            inGameMenu.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && inGameMenu.activeInHierarchy)
        {
            inGameMenu.SetActive(false);
        }
    }
}
