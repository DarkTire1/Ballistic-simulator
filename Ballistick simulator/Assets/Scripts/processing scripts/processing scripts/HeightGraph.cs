#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class HeightProfileWindow : EditorWindow
{
    private List<float> _heights;
    private float _maxDist;
    private List<TrajectoryPoint> _trajectory; // добавили траекторию
    private Vector2 _scroll;
    private float _zoom = 1f;

    public static void ShowWindow(List<float> heights, float maxDist, List<TrajectoryPoint> trajectory = null)
    {
        var window = GetWindow<HeightProfileWindow>("Профиль высот");
        window._heights = heights;
        window._maxDist = maxDist;
        window._trajectory = trajectory;
        window.minSize = new Vector2(500, 250);
        window.Show();
    }

    private void OnGUI()
    {
        if (_heights == null || _heights.Count < 2)
        {
            GUILayout.Label("Нет данных для отображения.", EditorStyles.boldLabel);
            return;
        }

        GUILayout.Label("Профиль ландшафта", EditorStyles.boldLabel);

        _zoom = GUILayout.HorizontalSlider(_zoom, 0.5f, 5f);
        GUILayout.Label($"Масштаб: {_zoom:F1}x");

        _scroll = GUILayout.BeginScrollView(_scroll, true, true, GUILayout.ExpandHeight(true));

        Rect graphRect = GUILayoutUtility.GetRect(_heights.Count * 5f * _zoom, 300f * _zoom);
        DrawGraph(graphRect);

        GUILayout.EndScrollView();
    }

    private void DrawGraph(Rect rect)
    {
        if (_heights.Count < 2)
            return;

        Handles.BeginGUI();

        // 1. Расчет масштаба
        float minH = Mathf.Min(_heights.ToArray());
        float maxH = Mathf.Max(_heights.ToArray());
        float range = Mathf.Max(1f, maxH - minH);
        float scaleY = rect.height / range;

        // Используем _maxDist (общую дальность) для масштаба по X
        float scaleX = rect.width / _maxDist;

        Handles.color = new Color(1f, 1f, 1f, 0.3f);
        Handles.DrawLine(new Vector3(rect.x, rect.yMax), new Vector3(rect.xMax, rect.yMax));
        Handles.DrawLine(new Vector3(rect.x, rect.y), new Vector3(rect.x, rect.yMax));

        // 2. Рисование ландшафта (Профиль высот)
        Handles.color = Color.green;
        Vector3 prev = Vector3.zero;

        // xStep для ландшафта - это _maxDist, поделенный на количество интервалов
        float landscapeXStep = _maxDist / (_heights.Count - 1);

        for (int i = 0; i < _heights.Count; i++)
        {
            float x = rect.x + i * landscapeXStep * scaleX; // Правильный расчет X
            float y = rect.yMax - (_heights[i] - minH) * scaleY;
            Vector3 point = new Vector3(x, y);
            if (i > 0) Handles.DrawLine(prev, point);
            prev = point;
        }

        // 3. Рисование траектории
        if (_trajectory != null && _trajectory.Count > 1)
        {
            Handles.color = Color.red;
            prev = Vector3.zero;

            for (int i = 0; i < _trajectory.Count; i++)
            {
                // Используем реальную горизонтальную дистанцию
                float distance = _trajectory[i].HorizontalDistance;

                // TrajectoryPoint.Position.Y (System.Numerics)
                float height = _trajectory[i].Position.Y;

                float x = rect.x + distance * scaleX; // Правильный расчет X: Дистанция * масштаб
                float y = rect.yMax - (height - minH) * scaleY;

                Vector3 point = new Vector3(x, y);
                if (i > 0) Handles.DrawLine(prev, point);
                prev = point;
            }

            // Отметка попадания
            if (_trajectory.Last().HitTarget)
            {
                float impactDist = _trajectory.Last().HorizontalDistance;
                float impactH = _trajectory.Last().Height; // или Position.Y
                float x = rect.x + impactDist * scaleX;
                float y = rect.yMax - (impactH - minH) * scaleY;

                Handles.color = Color.yellow;
                Handles.DrawSolidDisc(new Vector3(x, y), Vector3.back, 5f);
            }
        }

        Handles.EndGUI();

        // ... (метки GUI) ...
    }
}
#endif
