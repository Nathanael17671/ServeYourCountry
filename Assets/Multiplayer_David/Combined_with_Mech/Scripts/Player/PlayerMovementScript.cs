using Newtonsoft.Json;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementScript : NetworkBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Camera")]
    private GameObject cameraTransform;

    private Vector3 lookDir;
    private Vector3 move;
    private Rigidbody rb;


    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.freezeRotation = true;

        cameraTransform = GameObject.Find("Main Camera");
    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        PlayerMoveWalk();
    }

    void FixedUpdate()
    {
        if (!IsOwner)
        {
            return;
        }

        PlayerWalk2();
        PlayerRotate();
    }

    private void PlayerMoveWalk()
    {
        //Movement
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Get camera directions
        Vector3 cameraForward = cameraTransform.transform.forward;
        Vector3 cameraRight = cameraTransform.transform.right;

        // Ignore vertical camera angle
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calculate movement relative to camera
        move = (cameraForward * vertical + cameraRight * horizontal).normalized;

        if (move.sqrMagnitude > 0.01f)
        {
            lookDir = move;
        }
    }

    private void PlayerWalk2()
    {
        Vector3 targetvelocity = move * moveSpeed;

        rb.linearVelocity = new Vector3(targetvelocity.x, rb.linearVelocity.y, targetvelocity.z);
    }

    private void PlayerRotate()
    {
        if (lookDir.sqrMagnitude < 0.01f) return;

        Quaternion targetRotation = Quaternion.LookRotation(lookDir);

        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
    }
}
