using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyCreateui : MonoBehaviour
{
    [SerializeField] private TMP_InputField lobbyNameInputField;

    public void OnCreatePrivateGame()
    {
        ServeYourCountryLobbyScript.Instance.CreateLobby(lobbyNameInputField.text, true);
    }

    public void OnCreatePublicGame()
    {
        ServeYourCountryLobbyScript.Instance.CreateLobby(lobbyNameInputField.text, false);
    }

    public void OnBackButton()
    {
        Hide();
    }

    private void Start()
    {
        Hide();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
