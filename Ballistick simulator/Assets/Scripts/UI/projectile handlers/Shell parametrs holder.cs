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
    public TMP_InputField dragInput;
    public TMP_Dropdown trajectoryTypeDropdown;

    [Header("Координаты")]
    public TextMeshProUGUI startPositionText;
    public TextMeshProUGUI targetPositionText;

    public ShellParameters GetParameters()
    {
        float mass = TryParseFloat(massInput.text, 1f);
        float speed = TryParseFloat(speedInput.text, 10f);

        float diameterMm = TryParseFloat(diameterInput.text, 100f); // по умолчанию 100 мм (0.1 м)
        float diameter = diameterMm / 1000f; // перевод мм в метры

        float drag = TryParseFloat(dragInput.text, 0.47f); // default: шар

        TrajectoryType trajectoryType = trajectoryTypeDropdown.value == 0
            ? TrajectoryType.High
            : TrajectoryType.Low;

        Vector3 startPos = ParseVector3(startPositionText.text);
        Vector3 targetPos = ParseVector3(targetPositionText.text);

        return new ShellParameters
        {
            Mass = mass,
            InitialSpeed = speed,
            Diameter = diameter,
            DragCoefficient = drag,
            TrajectoryType = trajectoryType,
            StartPosition = startPos,
            TargetPosition = targetPos
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

    private Vector3 ParseVector3(string input)
    {
        float x = 0f, y = 0f, z = 0f;

        input = input.Replace(" ", "").ToLower();

        string[] parts = input.Split(new char[] { 'x', 'y', 'z', ':' }, System.StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length >= 3)
        {
            x = TryParseFloat(parts[0], 0f);
            y = TryParseFloat(parts[1], 0f);
            z = TryParseFloat(parts[2], 0f);
        }

        return new Vector3(x, y, z);
    }
}
