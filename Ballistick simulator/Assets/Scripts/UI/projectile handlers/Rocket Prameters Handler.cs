using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System; // Для StringSplitOptions

public class RocketParametersHolder : MonoBehaviour
{
    [Header("Параметры ракеты")]
    public TMP_InputField startMassInput;
    public TMP_InputField endMassInput;
    public TMP_InputField averageThrustInput;
    public TMP_InputField thrustTimeInput;
    public TMP_InputField diameterInput; 
    public TMP_InputField dragInput;
    public TMP_InputField destructionRadiusInput; 

    [Header("Координаты")]
    public TextMeshProUGUI startPositionText;
    public TextMeshProUGUI targetPositionText;

    public RocketParameters GetParameters()
    {
        // --- 1. Парсинг численных данных ---
        float startMass = TryParseFloat(startMassInput.text, 1f);
        float endMass = TryParseFloat(endMassInput.text, 50f);
        float avgThrust = TryParseFloat(averageThrustInput.text, 1000f);
        float thrustTime = TryParseFloat(thrustTimeInput.text, 10f);
        

        float diameterMm = TryParseFloat(diameterInput.text, 100f);
        float diameter = diameterMm / 1000f; // Конвертация мм в метры

        float destructionRadius = TryParseFloat(destructionRadiusInput.text, 5f);

        // --- 2. Парсинг координат (получаем UnityEngine.Vector3) ---
        UnityEngine.Vector3 startPosUnity = ParseVector3(startPositionText.text);
        UnityEngine.Vector3 targetPosUnity = ParseVector3(targetPositionText.text);

        // --- 3. Конвертация в System.Numerics.Vector3 ---
        System.Numerics.Vector3 startPosSystem = new System.Numerics.Vector3(
            startPosUnity.x, startPosUnity.y, startPosUnity.z
        );
        System.Numerics.Vector3 targetPosSystem = new System.Numerics.Vector3(
            targetPosUnity.x, targetPosUnity.y, targetPosUnity.z
        );

        return new RocketParameters
        {
            StartMass = startMass,
            EndMass = endMass,
            AverageThrust = avgThrust,
            ThrustTime = thrustTime,
            Diameter = diameter,

            // Новое поле для радиуса поражения
            RadiusOfDestruction = destructionRadius,

            // Присваиваем System.Numerics.Vector3
            StartPosition = startPosSystem,
            TargetPosition = targetPosSystem
        };
    }

    private float TryParseFloat(string input, float fallback)
    {
        if (string.IsNullOrEmpty(input))
            return fallback;

        input = input.Replace(",", ".");
        if (float.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
            return result;

        return fallback;
    }

    // --- ИСПРАВЛЕННЫЙ МЕТОД ДЛЯ ПАРСИНГА ФОРМАТА "x:XXX y:YYY z:ZZZ" ---
    private UnityEngine.Vector3 ParseVector3(string input)
    {
        // Очистка и унификация разделителей (для формата x:-663 y:126 z:77)
        string cleanInput = input.Replace(" ", "").ToLower();

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

        return new UnityEngine.Vector3(x, y, z);
    }
}