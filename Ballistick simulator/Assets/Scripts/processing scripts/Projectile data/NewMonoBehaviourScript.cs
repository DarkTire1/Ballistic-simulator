using UnityEngine;

public enum TrajectoryType
{
    Low,
    High
}

public class ShellParameters
{
    public float Mass;
    public float InitialSpeed;
    public float Diameter;
    public float DragCoefficient;
    public TrajectoryType TrajectoryType;
    public Vector3 StartPosition;
    public Vector3 TargetPosition;
}
public class RocketParameters
{
    public float StartMass;      // стартова€ масса (кг)
    public float EndMass;        // конечна€ масса (кг)
    public float AverageThrust;  // средн€€ т€га (Ќ)
    public float ThrustTime;     // врем€ работы т€ги (сек)
    public float Diameter;       // диаметр в метрах!
    public float DragCoefficient;// коэффициент лобового сопротивлени€
    public TrajectoryType TrajectoryType;
    public Vector3 StartPosition;
    public Vector3 TargetPosition;
}
