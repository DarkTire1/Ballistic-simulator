using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static void ChangeCameraState(Camera camera, Vector3 position, Quaternion rotation, bool isOrthographic)
    {
        if (camera == null) return;

        var rb = camera.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.position = position;
            rb.rotation = rotation;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            camera.transform.position = position;
            camera.transform.rotation = rotation;
        }

        camera.orthographic = isOrthographic;
    }
}
