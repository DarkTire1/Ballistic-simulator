using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // установка камеры в стартовое положение
    public static void ChangeCameraState(Camera camera, Vector3 position, Quaternion rotation, bool isOrthographic)
    {
        if (camera == null) return;

        var rb = camera.GetComponent<Rigidbody>();
        var transform = camera.transform;

        if (rb != null)
        {
            rb.position = position;
            rb.rotation = rotation;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            transform.position = position;
            transform.rotation = rotation;
        }

        camera.orthographic = isOrthographic;
    }
}
