using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectReady : NetworkBehaviour
{
    private Dictionary<ulong, bool> playerReadyDictionary;

    private void Awake()
    {
        playerReadyDictionary = new Dictionary<ulong, bool>();
    }

    public void SetPlayerReady()
    {
        SetPlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcparams = default)
    {
        playerReadyDictionary[serverRpcparams.Receive.SenderClientId] = true;

        bool allClientReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {

            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                // this player is not ready
                allClientReady = false;
                break;
            }
        }

        if (allClientReady)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
        }

    }
}
