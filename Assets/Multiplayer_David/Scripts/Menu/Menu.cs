using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    // [SerializeField] private GameObject menuObj;

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
        // menuObj.SetActive(false);
    }

    public void OnMainMenuPressed()
    {
        SceneManager.LoadScene("LobbyScene");
    }
}
