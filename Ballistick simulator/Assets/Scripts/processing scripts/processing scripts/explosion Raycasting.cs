using UnityEngine;
using System.Collections.Generic;

public class SpawnMarkersFromPoint : MonoBehaviour
{
    public GameObject markerPrefab; // Префаб сферы (необязательно)
    public float rayDistance; // Длина лучей
    [SerializeField] public int latitudeCount = 10; // Разбиение по широте
    [SerializeField] private int longitudeCount = 20; // Разбиение по долготе
    [SerializeField] private float markerSize = 0.1f; // Размер маркеров (изменено для примера)

    // ВНИМАНИЕ: Для учета ВСЕХ коллизий, включая триггеры, используем этот параметр.
    // Если нужно игнорировать триггеры, используйте QueryTriggerInteraction.Ignore
    private const QueryTriggerInteraction TriggerMode = QueryTriggerInteraction.Collide;

    private List<GameObject> markers = new List<GameObject>();

    /// <summary>
    /// Выпускает лучи из точки, смещенной на 1 метр вверх, и создает маркеры на местах попадания.
    /// </summary>
    /// <param name="origin">Центральная точка, откуда выпускаются лучи.</param>
    public void SpawnRays(Vector3 origin)
    {
        ClearMarkers();

        Vector3 startOrigin = origin + Vector3.up * 1f;

        const int allLayers = ~0;

        for (int lat = 0; lat <= latitudeCount; lat++)
        {
            float theta = Mathf.PI * lat / latitudeCount;
            for (int lon = 0; lon < longitudeCount; lon++)
            {
                float phi = 2 * Mathf.PI * lon / longitudeCount;

                Vector3 direction = new Vector3(
                    Mathf.Sin(theta) * Mathf.Cos(phi),
                    Mathf.Cos(theta),
                    Mathf.Sin(theta) * Mathf.Sin(phi)
                );

                if (Physics.Raycast(startOrigin, direction, out RaycastHit hit, rayDistance, allLayers, TriggerMode))
                {
                    CreateMarker(hit.point);
                }

            }
        }
    }

    private void CreateMarker(Vector3 position)
    {
        // ... (остальная часть метода CreateMarker остается без изменений)
        GameObject marker;
        if (markerPrefab != null)
        {
            marker = Instantiate(markerPrefab, position, Quaternion.identity);
            marker.transform.localScale = Vector3.one * markerSize;
        }
        else
        {
            // Создание сферы по умолчанию
            marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marker.transform.position = position;
            marker.transform.localScale = Vector3.one * markerSize;

            var renderer = marker.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.red;
            }

            // Удаляем коллайдер, чтобы маркеры сами не блокировали другие лучи
            var col = marker.GetComponent<Collider>();
            if (col != null)
            {
                Destroy(col);
            }
        }

        markers.Add(marker); // добавляем в список для последующего удаления
    }

    private void ClearMarkers()
    {
        foreach (var marker in markers)
        {
            if (marker != null)
                Destroy(marker);
        }
        markers.Clear();
    }
}