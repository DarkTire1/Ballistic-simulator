using UnityEngine;

public class ShellLaunchController : MonoBehaviour
{
    public ShellParametersHolder holder;

    public void OnButtonPressed()
    {
        ShellParameters shell = holder.GetParameters();

        Debug.Log("========= ПАРАМЕТРЫ СНАРЯДА =========");
        Debug.Log($"Масса: {shell.Mass} кг");
        Debug.Log($"Начальная скорость: {shell.InitialSpeed} м/с");
        Debug.Log($"Диаметр: {shell.Diameter} м");
        Debug.Log($"Коэффициент сопротивления: {shell.DragCoefficient}");
        Debug.Log($"Тип траектории: {shell.TrajectoryType}");
        Debug.Log($"Координаты запуска: {shell.StartPosition}");
        Debug.Log($"Координаты цели: {shell.TargetPosition}");
        Debug.Log("=====================================");
    }
}
