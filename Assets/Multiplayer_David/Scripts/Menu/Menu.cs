using UnityEngine;
using Unity.Netcode;
using System;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject menuObj;

    private void Start()
    {
        /*
        menuObj.SetActive(true);
        Time.timeScale = 0f;
        */
    }

    public void OnHostPressed()
    {
        NetworkManager.Singleton.StartHost();

        HideMenu();
    }

    public void OnClientPressed()
    {
        NetworkManager.Singleton.StartClient();

        HideMenu();
    }

    void HideMenu()
    {
        Time.timeScale = 1f;
        menuObj.SetActive(false);
    }
}
