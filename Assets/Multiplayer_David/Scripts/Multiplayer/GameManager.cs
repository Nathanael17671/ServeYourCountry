using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private Transform playerPrefab;

    void Update()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += CheckAllReady;
        }
    }



    private void CheckAllReady(string sceneName, LoadSceneMode loadSceneMode, List<ulong> ClinetsCompleted, List<ulong> ClientsDisconnected)
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Transform playerTransform = Instantiate(playerPrefab);
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }
    }
}
