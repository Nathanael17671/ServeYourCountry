using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyListSingleUi : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    private Lobby lobby;


    public void OnJoinLobby()
    {
        ServeYourCountryLobbyScript.Instance.JoinWithLobbyId(lobby.Id);
        Debug.Log("Hei");
    }

    public void SetLobby(Lobby lobby)
    {
        this.lobby = lobby;
        lobbyNameText.text = lobby.Name;
    }
}
