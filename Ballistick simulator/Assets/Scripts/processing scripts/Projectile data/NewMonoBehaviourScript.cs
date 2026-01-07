using System;
using System.Numerics; // Используем System.Numerics для всех векторных типов

// --- Параметры артиллерийского снаряда (С System.Numerics) ---
public struct ShellParameters
{
    // Физические параметры снаряда
    public float Mass;
    public float Diameter;
    public float InitialSpeed;
    public float RadiusOfDestruction;

    // Условия запуска
    public float InitialHeight;
    public float LaunchAngle;

    // Внешние условия
    public Vector2 WindVelocity; // System.Numerics.Vector2
    public Vector3 StartPosition; // System.Numerics.Vector3
    public Vector3 TargetPosition; // System.Numerics.Vector3
}

// --- Параметры ракеты (С System.Numerics) ---
public class RocketParameters
{
    public float StartMass;
    public float EndMass;
    public float AverageThrust;
    public float ThrustTime;
    public float RadiusOfDestruction;
    public float Diameter;
    public Vector3 StartPosition; // System.Numerics.Vector3
    public Vector3 TargetPosition; // System.Numerics.Vector3
}

public struct TrajectoryPoint
{
    // Время и Энергия
    public float Time;
    public float Speed;
    public float KineticEnergy;
    public float PotentialEnergy;

    // Положение
    public Vector3 Position; // System.Numerics.Vector3
    public float Height;
    public float HorizontalDistance;

    // Результат
    public Vector2 ImpactPosition; // System.Numerics.Vector2
    public bool HitTarget;
}