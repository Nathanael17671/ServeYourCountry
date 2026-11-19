using UnityEngine;

public class CharacterSelectPlayerScript : MonoBehaviour
{
    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
