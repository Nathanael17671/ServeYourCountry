using System;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyButtonManager : MonoBehaviour
{
    [SerializeField] private LobbyCreateui lobbyCreateui;
    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] private Transform lobbyContainer;
    [SerializeField] private Transform lobbyTemplate;

    private void Awake()
    {
        if (lobbyTemplate != null)
        {
            lobbyTemplate.gameObject.SetActive(false);
        }
    }

    public async void OnReturnToMainMenu()
    {
        await ServeYourCountryLobbyScript.Instance.LeaveLobby();

        SceneManager.LoadScene("LobbyScene");
    }

    public void OnCreateLobby()
    {
        lobbyCreateui.Show();
    }

    public void OnQuickJoin()
    {
        ServeYourCountryLobbyScript.Instance.QuickJoin();
    }

    public void OnJoinCode()
    {
        ServeYourCountryLobbyScript.Instance.JoinWithCode(joinCodeInputField.text);
    }

    private void Start()
    {
        ServeYourCountryLobbyScript.Instance.onLobbyListChanged += ServeYourCountryLobbyScript_onLobbyListChanged;
        UpdateLobbyList(new List<Lobby>());
    }

    private void ServeYourCountryLobbyScript_onLobbyListChanged(object sender, ServeYourCountryLobbyScript.OnLobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.lobbyList);
    }

    private void UpdateLobbyList(List<Lobby> lobbyList)
    {
        if (lobbyContainer != null)
        {
            foreach (Transform child in lobbyContainer)
            {
                if (child == lobbyTemplate)
                    continue;
                Destroy(child.gameObject);
            }

            foreach (Lobby lobby in lobbyList)
            {
                Transform lobbyTransform = Instantiate(lobbyTemplate, lobbyContainer);
                lobbyTransform.gameObject.SetActive(true);
                lobbyTransform.GetComponent<LobbyListSingleUi>().SetLobby(lobby);
            }
        }
    }
}
