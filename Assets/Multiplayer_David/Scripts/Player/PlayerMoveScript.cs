using UnityEngine;
using Unity.Netcode;

public class PlayerMoveScript : NetworkBehaviour
{

    [SerializeField] private float _speed = 5f;


    void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        PlayerMoveWalk();
    }

    private void PlayerMoveWalk()
    {
        Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        transform.position += move * Time.deltaTime * _speed;
    }
}
