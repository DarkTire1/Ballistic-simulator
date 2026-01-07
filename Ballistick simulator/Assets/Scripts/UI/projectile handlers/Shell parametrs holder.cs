using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShellParametersHolder : MonoBehaviour
{
    [Header("Параметры снаряда")]
    public TMP_InputField massInput;
    public TMP_InputField speedInput;
    public TMP_InputField diameterInput;
    public TMP_InputField DestructionRadius;

    [Header("Координаты")]
    // Поля должны быть InputField, если пользователь вводит данные. 
    // Если TextMeshProUGUI - значит, они отображают данные, введенные из другого места.
    public TextMeshProUGUI startPositionText;
    public TextMeshProUGUI targetPositionText;

    public ShellParameters GetParameters()
    {
        // ... [Парсинг численных значений] ...
        float mass = TryParseFloat(massInput.text, 42f);
        float speed = TryParseFloat(speedInput.text, 500f);
        float diameterMm = TryParseFloat(diameterInput.text, 152f);
        float diameter = diameterMm / 1000f;
        float destructionRadius = TryParseFloat(DestructionRadius.text, 50f);

        // Парсинг координат: здесь мы получаем UnityEngine.Vector3
        // Так как ParseVector3 использует UnityEngine.Vector3
        UnityEngine.Vector3 startPosUnity = ParseVector3(startPositionText.text);
        UnityEngine.Vector3 targetPosUnity = ParseVector3(targetPositionText.text);

        // --- КЛЮЧЕВАЯ КОНВЕРТАЦИЯ ---
        // Создаем векторы System.Numerics
        System.Numerics.Vector3 startPosSystem = new System.Numerics.Vector3(
            startPosUnity.x,
            startPosUnity.y,
            startPosUnity.z
        );
        System.Numerics.Vector3 targetPosSystem = new System.Numerics.Vector3(
            targetPosUnity.x,
            targetPosUnity.y,
            targetPosUnity.z
        );
        // --- КОНЕЦ КОНВЕРТАЦИИ ---

        return new ShellParameters
        {
            Mass = mass,
            InitialSpeed = speed,
            Diameter = diameter,
            RadiusOfDestruction = destructionRadius,
            InitialHeight = 3, 

            // ПРИСВАИВАЕМ System.Numerics.Vector3
            StartPosition = startPosSystem,
            TargetPosition = targetPosSystem,

            LaunchAngle = 0f,
            WindVelocity = System.Numerics.Vector2.Zero // Используем System.Numerics.Vector2
        };
    }
    private float TryParseFloat(string input, float fallback)
    {
        if (string.IsNullOrEmpty(input))
            return fallback;

        // Унифицируем разделитель: запятая -> точка
        input = input.Replace(",", ".");

        if (float.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
            return result;

        return fallback;
    }

    // --- ИСПРАВЛЕННЫЙ МЕТОД ДЛЯ ПАРСИНГА ФОРМАТА "x:XXX y:YYY z:ZZZ" ---
    private Vector3 ParseVector3(string input)
    {
        // Очистка и унификация разделителей
        string cleanInput = input.Replace(" ", "").ToLower(); // "x:-663y:126z:77"

        float x = 0f, y = 0f, z = 0f;

        // Находим и парсим X
        int xIndex = cleanInput.IndexOf("x:");
        int yIndex = cleanInput.IndexOf("y:");
        if (xIndex != -1 && yIndex != -1)
        {
            string xPart = cleanInput.Substring(xIndex + 2, yIndex - (xIndex + 2));
            x = TryParseFloat(xPart, 0f);
        }

        // Находим и парсим Y
        int zIndex = cleanInput.IndexOf("z:");
        if (yIndex != -1 && zIndex != -1)
        {
            string yPart = cleanInput.Substring(yIndex + 2, zIndex - (yIndex + 2));
            y = TryParseFloat(yPart, 0f);
        }

        // Находим и парсим Z (до конца строки)
        if (zIndex != -1)
        {
            string zPart = cleanInput.Substring(zIndex + 2);
            z = TryParseFloat(zPart, 0f);
        }

        return new Vector3(x, y, z);
    }
}