using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private Transform playerPrefab;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += CheckAllReady;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= CheckAllReady;
        }
    }

    private void CheckAllReady(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsDisconnected)
    {
        if (!IsServer) return;

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject != null)
                continue;

            Transform playerTransform = Instantiate(playerPrefab);

            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }
    }
}
