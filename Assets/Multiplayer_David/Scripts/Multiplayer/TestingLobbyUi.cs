using UnityEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class TestingLobbyUi : MonoBehaviour
{
    [SerializeField] private GameObject menuObj;

    public void OnHostPressed()
    {
        NetworkManager.Singleton.StartHost();

        NetworkManager.Singleton.SceneManager.LoadScene("CharacterSelectScene", LoadSceneMode.Single);
    }

    public void OnClientPressed()
    {
        NetworkManager.Singleton.StartClient();
    }
}
