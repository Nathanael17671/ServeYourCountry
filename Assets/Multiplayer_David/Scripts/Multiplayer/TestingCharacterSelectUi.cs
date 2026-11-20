using System.Runtime.CompilerServices;
using Unity.Netcode;
using TMPro;
using UnityEngine;
using Unity.Services.Lobbies.Models;

public class TestingCharacterSelectUi : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;
private void Start()
    {
        UpdateLobbyUI();
    }

    private void UpdateLobbyUI()
    {
        if (ServeYourCountryLobbyScript.Instance == null)
        {
            Debug.LogError("Lobby script Instance is NULL!");
            return;
        }

        Lobby lobby = ServeYourCountryLobbyScript.Instance.GetLobby();

        if (lobby == null)
        {
            Debug.LogError("Joined Lobby is NULL!");
            return;
        }

        if (lobbyNameText == null || lobbyCodeText == null)
        {
            Debug.LogError("One or more UI references are missing!");
            return;
        }

        lobbyNameText.text = "Lobby Name: " + lobby.Name;
        lobbyCodeText.text = "Lobby Code: " + lobby.LobbyCode;

        Debug.Log("Lobby UI successfully updated!");
        Debug.Log("Lobby Code: " + lobby.LobbyCode);
    }
}
