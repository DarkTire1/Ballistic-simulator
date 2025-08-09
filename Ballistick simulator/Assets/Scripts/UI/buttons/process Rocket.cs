using UnityEngine;

public class RocketLaunchController : MonoBehaviour
{
    public RocketParametersHolder parametersHolder;

    public void OnLaunchButtonPressed()
    {
        RocketParameters parameters = parametersHolder.GetParameters();

        Debug.Log("========== ПАРАМЕТРЫ РАКЕТЫ ==========");
        Debug.Log($"Стартовая масса: {parameters.StartMass} кг");
        Debug.Log($"Конечная масса: {parameters.EndMass} кг");
        Debug.Log($"Средняя тяга: {parameters.AverageThrust} Н");
        Debug.Log($"Время тяги: {parameters.ThrustTime} с");
        Debug.Log($"Диаметр: {parameters.Diameter} м");
        Debug.Log($"Коэффициент сопротивления: {parameters.DragCoefficient}");
        Debug.Log($"Тип траектории: {parameters.TrajectoryType}");
        Debug.Log($"Стартовая позиция: {parameters.StartPosition}");
        Debug.Log($"Целевая позиция: {parameters.TargetPosition}");
        Debug.Log("=====================================");

        // Здесь может быть вызов реальной симуляции, если надо
        // simulator.Simulate(parameters);
    }
}

