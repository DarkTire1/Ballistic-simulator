using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RocketParametersHolder : MonoBehaviour
{
    [Header("Параметры ракеты")]
    public TMP_InputField startMassInput;
    public TMP_InputField endMassInput;
    public TMP_InputField averageThrustInput;
    public TMP_InputField thrustTimeInput;
    public TMP_InputField diameterInput; // мм
    public TMP_InputField dragInput;
    public TMP_Dropdown trajectoryTypeDropdown;

    [Header("Координаты")]
    public TextMeshProUGUI startPositionText;
    public TextMeshProUGUI targetPositionText;

    public RocketParameters GetParameters()
    {
        float startMass = TryParseFloat(startMassInput.text, 1f);
        float endMass = TryParseFloat(endMassInput.text, 50f);
        float avgThrust = TryParseFloat(averageThrustInput.text, 1000f);
        float thrustTime = TryParseFloat(thrustTimeInput.text, 10f);

        float diameterMm = TryParseFloat(diameterInput.text, 100f);
        float diameter = diameterMm / 1000f;

        float drag = TryParseFloat(dragInput.text, 0.5f);

        TrajectoryType trajectoryType = trajectoryTypeDropdown.value == 0
            ? TrajectoryType.High
            : TrajectoryType.Low;

        Vector3 startPos = ParseVector3(startPositionText.text);
        Vector3 targetPos = ParseVector3(targetPositionText.text);

        return new RocketParameters
        {
            StartMass = startMass,
            EndMass = endMass,
            AverageThrust = avgThrust,
            ThrustTime = thrustTime,
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
