using UnityEngine;

public class camera_movement : MonoBehaviour
{
    [SerializeField]
    private float mouseSensitivitySpeed = 250f;
    [SerializeField]
    private float speed = 10f;
    [SerializeField]
    private float sensitivity = 100f;

    private float xRotation = 0f;
    private float yRotation = 0f;

    private bool cameraMode;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            cameraMode = !cameraMode;
            if (cameraMode)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            if (!cameraMode)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        if (cameraMode)
        {
            float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

            xRotation -= mouseY;
            yRotation += mouseX;

            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);

            float mouse = Input.GetAxis("Mouse ScrollWheel");

            speed = Mathf.Clamp(speed + mouse * mouseSensitivitySpeed, 0f, 25000f);
        }
    }

    void FixedUpdate()
    {
        if (cameraMode)
        {
            Vector3 lookDirection = transform.forward;

            Vector3 movement = Vector3.zero;

            if (Input.GetKey(KeyCode.A))
            {
                movement.x -= lookDirection.z * speed * Time.deltaTime;
                movement.z += lookDirection.x * speed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.D))
            {
                movement.x += lookDirection.z * speed * Time.deltaTime;
                movement.z -= lookDirection.x * speed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.W))
            {
                movement += lookDirection * speed * Time.deltaTime;
                movement.y = 0f;
            }

            if (Input.GetKey(KeyCode.S))
            {
                movement -= lookDirection * speed * Time.deltaTime;
                movement.y = 0f;
            }

            if (Input.GetKey(KeyCode.Space))
                movement.y += speed * Time.deltaTime;

            if (Input.GetKey(KeyCode.LeftShift))
                movement.y -= speed * Time.deltaTime;

            transform.position += movement;
        }
    }
}
