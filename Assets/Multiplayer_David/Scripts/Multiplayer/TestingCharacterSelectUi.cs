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
        Lobby lobby = ServeYourCountryLobbyScript.Instance.GetLobby();

        lobbyNameText.text = "Lobby Name: " + lobby.Name;
        lobbyCodeText.text = "Lobby Code: " + lobby.LobbyCode;
    }


}
