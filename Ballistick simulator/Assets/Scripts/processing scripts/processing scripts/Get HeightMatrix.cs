using UnityEngine;

public class GetHeightMatrix : MonoBehaviour
{
    [SerializeField]
    private MeshCollider _meshCollider;

    private Renderer targetRenderer;
    private GameObject visualizationQuad;

    public int resolutionX;
    public int resolutionZ;
    public float[,] heightMap;

    public float raycastHeight = 500f;

    public void GenerateHeightMap()
    {
        if (_meshCollider == null)
        {
            Debug.LogWarning("MeshCollider is not assigned!");
            return;
        }

        Bounds bounds = _meshCollider.bounds; // используем world bounds
        Vector3 min = bounds.min;
        Vector3 max = bounds.max;

        resolutionX = Mathf.RoundToInt(max.x - min.x);
        resolutionZ = Mathf.RoundToInt(max.z - min.z);

        heightMap = new float[resolutionX, resolutionZ];

        for (int x = 0; x < resolutionX; x++)
        {
            for (int z = 0; z < resolutionZ; z++)
            {
                float worldX = min.x + x;
                float worldZ = min.z + z;
                Vector3 rayOrigin = new Vector3(worldX, max.y + raycastHeight, worldZ);
                Ray ray = new Ray(rayOrigin, Vector3.down);

                if (_meshCollider.Raycast(ray, out RaycastHit hit, raycastHeight * 2))
                {
                    heightMap[x, z] = hit.point.y;
                }
                else
                {
                    heightMap[x, z] = 0;
                }
            }
        }
    }

    private void Start()
    {
        GenerateHeightMap();

        if (heightMap != null)
        {
            CreateVisualizationQuad();
            VisualizeHeightMap();
        }
    }

    private void CreateVisualizationQuad()
    {
        if (visualizationQuad != null)
            Destroy(visualizationQuad);

        visualizationQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        visualizationQuad.name = "Heightmap Visualization Quad";

        // Позиция — центр heightmap (по X и Z)
        visualizationQuad.transform.position = new Vector3(resolutionX / 2f, 0, -resolutionZ / 2f);

        // Поворот Quad, чтобы он смотрел вверх
        visualizationQuad.transform.rotation = Quaternion.Euler(90, 0, 0);

        // Масштаб Quad под размеры heightmap
        visualizationQuad.transform.localScale = new Vector3(resolutionX, resolutionZ, 1);

        // Материал с Unlit/Texture
        Material mat = new Material(Shader.Find("Unlit/Texture"));
        visualizationQuad.GetComponent<Renderer>().material = mat;

        targetRenderer = visualizationQuad.GetComponent<Renderer>();
    }

    private void VisualizeHeightMap()
    {
        Texture2D tex = CreateHeightmapTexture(heightMap);
        targetRenderer.material.mainTexture = tex;
    }

    private Texture2D CreateHeightmapTexture(float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false);

        float min = float.MaxValue;
        float max = float.MinValue;

        foreach (float val in heightMap)
        {
            if (!float.IsNaN(val))
            {
                if (val < min) min = val;
                if (val > max) max = val;
            }
        }

        if (max - min < 0.0001f) max = min + 1f; // чтобы избежать деления на 0

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float val = heightMap[x, y];
                if (float.IsNaN(val))
                    val = min;

                float normalized = (val - min) / (max - min);
                Color color = new Color(normalized, normalized, normalized);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }
}



