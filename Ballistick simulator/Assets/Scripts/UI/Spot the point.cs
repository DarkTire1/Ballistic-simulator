using UnityEngine;

public class Spotthepoint : MonoBehaviour
{
    private GameObject currentMarker;

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // ПКМ
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 point = hit.point;

                // Удаляем предыдущую точку, если она есть
                if (currentMarker != null)
                {
                    Destroy(currentMarker);
                }

                // Создаём новый маркер (маленький красный шарик)
                currentMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                currentMarker.transform.position = point;
                currentMarker.transform.localScale = Vector3.one * 30f;

                // Отключаем коллайдер, чтобы не мешал новым кликам
                Destroy(currentMarker.GetComponent<Collider>());

                // Устанавливаем красный цвет
                var renderer = currentMarker.GetComponent<Renderer>();
                renderer.material = new Material(Shader.Find("Standard"));
                renderer.material.color = Color.red;

                // Выводим координаты в консоль
                Debug.Log("Hit point: " + point.ToString("F3"));

                // Рисуем Debug крест (виден 1 сек)
                DebugDrawHitPoint(point);
            }
        }
    }

    void DebugDrawHitPoint(Vector3 point)
    {
        Debug.DrawLine(point + Vector3.up * 0.1f, point - Vector3.up * 0.1f, Color.red, 1f);
        Debug.DrawLine(point + Vector3.right * 0.1f, point - Vector3.right * 0.1f, Color.red, 1f);
        Debug.DrawLine(point + Vector3.forward * 0.1f, point - Vector3.forward * 0.1f, Color.red, 1f);
    }
}

