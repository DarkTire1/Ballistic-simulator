using UnityEngine;

public class camera_movement : MonoBehaviour
{
    [SerializeField]
    private float speedStep = 250f;
    [SerializeField]
    private float moveSpeed = 100f;
    [SerializeField]
    private float sensitivity = 100f;
    [SerializeField]
    private Rigidbody cameraRb;

    private float xRotation = 0f;
    private float yRotation = 0f;

    private bool cameraMode = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            cameraMode = !cameraMode;
            if (cameraMode)
                CameraManager.ChangeCameraState(Camera.main, new Vector3 (0, 300, 0), Quaternion.Euler(0, 0, 0), false);
            else
                CameraManager.ChangeCameraState(Camera.main, new Vector3(0, 12000, 0), Quaternion.Euler(90, 0, 0), true);
            Cursor.lockState = cameraMode ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !cameraMode;
        }

        if (cameraMode == true)
        {
            float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

            xRotation -= mouseY;
            yRotation += mouseX;

            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);

            float mouse = Input.GetAxis("Mouse ScrollWheel");

            moveSpeed = Mathf.Clamp(moveSpeed + mouse * speedStep, 0f, 25000f);
        }
    }

    void FixedUpdate()
    {
        if (cameraMode)
        {
            Vector3 lookDirection = transform.forward;
            Vector3 rightDirection = transform.right;


            Vector3 movement = Vector3.zero;

            if (Input.GetKey(KeyCode.A))
                movement -= rightDirection;

            if (Input.GetKey(KeyCode.D))
                movement += rightDirection;

            if (Input.GetKey(KeyCode.W))
                movement += new Vector3 (lookDirection.x, 0f, lookDirection.z);

            if (Input.GetKey(KeyCode.S))
                movement -= new Vector3(lookDirection.x, 0f, lookDirection.z);

            if (Input.GetKey(KeyCode.Space))
                movement += Vector3.up;

            if (Input.GetKey(KeyCode.LeftShift))
                movement += Vector3.down;

            movement = movement.normalized * moveSpeed;

            cameraRb.linearVelocity = movement;
        }
    }

}
