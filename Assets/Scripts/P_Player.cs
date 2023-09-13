using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class P_Player : S_Singleton<P_Player>
{
    [SerializeField] Camera renderCamera;
    [SerializeField] Transform cameraSocket;
    [SerializeField] CharacterController characterController;
    [SerializeField, Range(0.1f, 50f)] float moveSpeed = 1f;
    [SerializeField, Range(0.1f, 100f)] float rotateSpeed = 1f;
    [SerializeField, Range(0f, 90f)] float minRotationX = 85f;
    [SerializeField, Range(270, 360f)] float maxRotationX = 275f;

    public Camera Camera => renderCamera;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        InitCamera();
    }

    void InitCamera()
    {
        if (!cameraSocket)
        {
            Debug.Log("No camera socket");
            return;
        }
        renderCamera = cameraSocket.GetComponent<Camera>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }


    private void Update()
    {
        MovePlayer();
        RotateHorizontal();
        RotateVertical();
    }

    void MovePlayer()
    {
        Vector3 _normalizedMovement = (transform.right * Input.GetAxis("Horizontal") +
                                       transform.forward * Input.GetAxis("Vertical"))
                                       .normalized;
        characterController.Move(_normalizedMovement * moveSpeed * Time.deltaTime);
    }

    void RotateHorizontal()
    {
        Vector3 _euler = transform.eulerAngles;
        _euler.y += Input.GetAxis("Mouse X") * rotateSpeed;
        transform.eulerAngles = _euler;
    }
    void RotateVertical()
    {
        Vector3 _euler = cameraSocket.eulerAngles;
        float _eulerX = _euler.x - Input.GetAxis("Mouse Y") * rotateSpeed;
        
        if (_eulerX > minRotationX && _eulerX <= 180f)
            _euler.x = minRotationX;
        else if (_eulerX < maxRotationX && _eulerX > 180f)
            _euler.x = maxRotationX;
        else
            _euler.x = _eulerX;

        cameraSocket.eulerAngles = _euler;
    }
}