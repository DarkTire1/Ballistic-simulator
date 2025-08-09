using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Spotthepoint : MonoBehaviour
{
    [SerializeField] private GameObject label;
    [SerializeField] private Color markerColor;
    [SerializeField] private GameObject[] uiList;

    private GameObject currentMarker;
    private Material markerMaterial;

    private void Start()
    {
        markerMaterial = new Material(Shader.Find("Standard"))
        {
            color = markerColor
        };
        this.enabled = false;
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(1)) return;

        // тут происходит получение координат ландшафта из сцены
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 point = hit.point;

            if (currentMarker != null)
                Destroy(currentMarker);

            CreateMarker(point);
            DebugDrawHitPoint(point);

            ActivateUi();
            // подстановка координат в UI
            if (label != null && label.TryGetComponent<TextMeshProUGUI>(out var text))
                text.text = $"x:{point.x:F0} y:{point.y:F0} z:{point.z:F0}";


            this.enabled = false;
        }
    }

    private void CreateMarker(Vector3 position)
    {
        currentMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        currentMarker.transform.SetPositionAndRotation(position, Quaternion.identity);
        currentMarker.transform.localScale = Vector3.one * 15f;

        var renderer = currentMarker.GetComponent<Renderer>();
        renderer.material = markerMaterial;

        Destroy(currentMarker.GetComponent<Collider>());
    }

    private void DebugDrawHitPoint(Vector3 point)
    {
        Debug.DrawLine(point + Vector3.up * 0.1f, point - Vector3.up * 0.1f, Color.red, 1f);
        Debug.DrawLine(point + Vector3.right * 0.1f, point - Vector3.right * 0.1f, Color.red, 1f);
        Debug.DrawLine(point + Vector3.forward * 0.1f, point - Vector3.forward * 0.1f, Color.red, 1f);
    }

    public void EnableSpotting()
    {
        this.enabled = true;
    }
    private void ActivateUi()
    {
        foreach (GameObject uiElement in uiList)
        {
            uiElement.SetActive(true);
        }
    }
    public void RemoveCurrentMarker()
    {
        if (currentMarker != null)
        {
            Destroy(currentMarker);
            currentMarker = null;

            if (label != null && label.TryGetComponent<TextMeshProUGUI>(out var text))
                text.text = "";
        }
    }
}

