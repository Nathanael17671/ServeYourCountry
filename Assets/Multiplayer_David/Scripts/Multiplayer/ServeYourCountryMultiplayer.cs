using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class ServeYourCountryMultiplayer : NetworkBehaviour
{
    public const int MAX_PLAYER_AMOUNT = 4;

    public static ServeYourCountryMultiplayer Instance { get; private set; }


    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }
}
