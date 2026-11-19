using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class ServeYourCountryMultiplayer : NetworkBehaviour
{
    /*
    public const int MAX_PLAYER_AMOUNT = 4;

    public static ServeYourCountryMultiplayer Instance { get; private set; }

    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;

    [SerializeField] private ServeYourCountryMultiplayer serveYourCountryMultiplayer;

    private NetworkList<PlayerData> playerDataNetworkList;

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);

        playerDataNetworkList = new NetworkList<PlayerData>();
    }

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest,)
    {

    }
    */
}
