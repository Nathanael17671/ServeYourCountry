using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyButtonManager : MonoBehaviour
{
    [SerializeField] private LobbyCreateui lobbyCreateui;

    [SerializeField] private TMP_InputField joinCodeInputField;

    public void OnMainMenuPressed()
    {
        SceneManager.LoadScene(0);
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

}
