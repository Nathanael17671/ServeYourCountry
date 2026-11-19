using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Movement : MonoBehaviour
{ 
    [Header("Movement Settings")]
    public float moveSpeed = 6f;
    public float rotationSpeed = 1500f;

    [Header("Smoothing Settings")]
    public float accelerationTime = 0.25f;
    public float decelerationTime = 0.05f;

    private CharacterController controller;
    private Camera mainCamera;

    private float currentSpeed;
    private float speedSmoothVelocity;
    private Vector3 lastMoveDirection;
    private float verticalVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        //Movement
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        Quaternion cameraYaw = Quaternion.Euler(0f, mainCamera.transform.eulerAngles.y, 0f);
        Vector3 rotatedMove = cameraYaw * moveDirection;
        
        //Rotation
        bool isMoving = moveDirection.magnitude >= 0.1f;
        if (isMoving)
        {
            lastMoveDirection = rotatedMove;
            Quaternion targetRotation = Quaternion.LookRotation(rotatedMove);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        //Smooth
        float targetSpeed = isMoving ? moveSpeed : 0f;
        float activeSmoothTime = isMoving ? accelerationTime : decelerationTime;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, activeSmoothTime);
        Vector3 horizontalMovement = lastMoveDirection * currentSpeed;

        //Gravity
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity +=  9.81f * Time.deltaTime;
        }

        //Apply Movement
        Vector3 finalMovement = horizontalMovement + (Vector3.down * verticalVelocity);
        controller.Move(finalMovement * Time.deltaTime);
    }
}
