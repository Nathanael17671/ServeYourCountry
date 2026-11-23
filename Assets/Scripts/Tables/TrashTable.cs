using UnityEngine;

public class TrashTable : MonoBehaviour
{
    private MainTableScript mainTableScript;

    void Awake()
    {
        mainTableScript = GetComponent<MainTableScript>();
    }
}
