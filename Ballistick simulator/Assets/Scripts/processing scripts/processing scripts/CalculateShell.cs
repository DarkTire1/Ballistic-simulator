using System;
using System.Collections.Generic;
using System.Numerics;

public class ShellSimulator
{
    private const float G0 = 9.80665f;
    private const float R = 287.05287f;
    private const float GAMMA = 1.4f;

    public static float CalculateAsimuth(Vector3 initialPos, Vector3 targetPos)
    {
        Vector3 direction = targetPos - initialPos;

        float azimuthRad = (float)Math.Atan2(direction.X, direction.Z);

        if (azimuthRad < 0f)
            azimuthRad += 2f * (float)Math.PI;

        return azimuthRad;
    }

    public struct State
    {
        public Vector3 Position;
        public Vector3 Velocity;
    }

    public struct Derivatives
    {
        public Vector3 dPosition_dt;
        public Vector3 dVelocity_dt;
    }

    private float GetLandscapeHeight(float xPosition, List<float> landscapeHeights, float xStep)
    {
        if (landscapeHeights == null || landscapeHeights.Count == 0 || xPosition < 0)
        {
            return 0f;
        }

        int index1 = (int)Math.Floor(xPosition / xStep);
        int index2 = index1 + 1;

        if (index1 >= landscapeHeights.Count - 1)
        {
            if (index1 < landscapeHeights.Count)
            {
                return landscapeHeights[index1];
            }
            return 0f;
        }

        float x1 = index1 * xStep;
        float y1 = landscapeHeights[index1];
        float x2 = index2 * xStep;
        float y2 = landscapeHeights[index2];

        if (Math.Abs(x2 - x1) < 1e-6) return y1;

        return y1 + (y2 - y1) * (xPosition - x1) / (x2 - x1);
    }

    public List<TrajectoryPoint> RunSimulation(ShellParameters parameters, float deltaTime, List<float> landscapeHeights, float xStep, Vector2 targetPosition, float targetRadius)
    {
        var trajectoryList = new List<TrajectoryPoint>();

        State currentState = new State
        {
            Position = new Vector3(0, parameters.InitialHeight, 0),
            Velocity = new Vector3(
                parameters.InitialSpeed * (float)Math.Cos(parameters.LaunchAngle),
                parameters.InitialSpeed * (float)Math.Sin(parameters.LaunchAngle),
                0)
        };

        float time = 0f;
        bool hasFinished = false;
        Vector2 finalImpactPosition = Vector2.Zero;
        bool hitTarget = false;

        while (!hasFinished)
        {
            float xPos = currentState.Position.X;
            float yPos = currentState.Position.Y;

            float speed = currentState.Velocity.Length();
            float kineticEnergy = 0.5f * parameters.Mass * speed * speed;
            float potentialEnergy = parameters.Mass * G0 * yPos;

            float landscapeHeight = GetLandscapeHeight(xPos, landscapeHeights, xStep);

            // --- Горизонтальное расстояние до цели в 2D ---
            float horizontalDistanceToTarget = Math.Abs(xPos - targetPosition.X);

            // --- Подцикл для точного столкновения с ландшафтом ---
            if (yPos < landscapeHeight + 5f && currentState.Velocity.Y < 0)
            {
                float subDeltaTime = deltaTime / 10.0f;
                State prevSubState = currentState;
                bool subHit = false;

                for (int i = 0; i < 10; i++)
                {
                    prevSubState = currentState;
                    currentState = Integrate(currentState, parameters, time + i * subDeltaTime, subDeltaTime);
                    float subYPos = currentState.Position.Y;
                    float subXPos = currentState.Position.X;
                    float subLandscapeHeight = GetLandscapeHeight(subXPos, landscapeHeights, xStep);

                    if (subYPos <= subLandscapeHeight)
                    {
                        hasFinished = true;
                        finalImpactPosition = new Vector2(subXPos, subLandscapeHeight);
                        hitTarget = Math.Abs(subXPos - targetPosition.X) <= targetRadius;
                        currentState = prevSubState;
                        time += i * subDeltaTime;
                        subHit = true;
                        break;
                    }
                }

                if (subHit)
                {
                    trajectoryList.Add(new TrajectoryPoint
                    {
                        Time = time,
                        Speed = currentState.Velocity.Length(),
                        KineticEnergy = 0.5f * parameters.Mass * currentState.Velocity.LengthSquared(),
                        PotentialEnergy = parameters.Mass * G0 * currentState.Position.Y,
                        Position = currentState.Position,
                        Height = currentState.Position.Y,
                        HorizontalDistance = currentState.Position.X,
                        ImpactPosition = finalImpactPosition,
                        HitTarget = hitTarget
                    });
                    break;
                }
            }

            // --- Столкновение с ландшафтом ---
            if (yPos <= landscapeHeight)
            {
                hasFinished = true;
                finalImpactPosition = new Vector2(xPos, landscapeHeight);
                hitTarget = horizontalDistanceToTarget <= targetRadius;
            }

            // --- Вылет за пределы ландшафта ---
            float maxX = (landscapeHeights.Count - 1) * xStep;
            if (xPos >= maxX)
            {
                hasFinished = true;
                finalImpactPosition = new Vector2(maxX, Math.Max(yPos, GetLandscapeHeight(maxX, landscapeHeights, xStep)));
                hitTarget = false;
            }

            // --- Добавляем точку траектории ---
            trajectoryList.Add(new TrajectoryPoint
            {
                Time = time,
                Speed = speed,
                KineticEnergy = kineticEnergy,
                PotentialEnergy = potentialEnergy,
                Position = currentState.Position,
                Height = yPos,
                HorizontalDistance = xPos,
                ImpactPosition = hasFinished ? finalImpactPosition : Vector2.Zero,
                HitTarget = hasFinished ? hitTarget : false
            });

            if (hasFinished) break;

            currentState = Integrate(currentState, parameters, time, deltaTime);
            time += deltaTime;
        }

        // --- Вывод финального состояния ---
        if (trajectoryList.Count > 0)
        {
            TrajectoryPoint final = trajectoryList[^1];
            Console.WriteLine($"Final Time: {final.Time:F2}s, Final Position: {final.Position}");
            Console.WriteLine($"Hit Target: {final.HitTarget}, Impact Position: {final.ImpactPosition}");
        }

        return trajectoryList;
    }


    // Вспомогательный метод для Integrate, чтобы использовать его в интерполяции
    private State Integrate(Vector3 position, Vector3 velocity, ShellParameters parameters, float time, float deltaTime)
    {
        State state = new State { Position = position, Velocity = velocity };
        return Integrate(state, parameters, time, deltaTime);
    }

    // --- Остальные методы остаются без изменений (Integrate, AddDerivatives, Evaluate, GetAirDensity, GetSpeedOfSound, GetDragCoefficient) ---

    private State Integrate(State state, ShellParameters parameters, float time, float deltaTime)
    {
        Derivatives a = Evaluate(state, parameters);
        Derivatives b = Evaluate(AddDerivatives(state, a, deltaTime * 0.5f), parameters);
        Derivatives c = Evaluate(AddDerivatives(state, b, deltaTime * 0.5f), parameters);
        Derivatives d = Evaluate(AddDerivatives(state, c, deltaTime), parameters);

        Vector3 dxdt = (deltaTime / 6.0f) * (a.dPosition_dt + 2 * b.dPosition_dt + 2 * c.dPosition_dt + d.dPosition_dt);
        Vector3 dvdt = (deltaTime / 6.0f) * (a.dVelocity_dt + 2 * b.dVelocity_dt + 2 * c.dVelocity_dt + d.dVelocity_dt);

        return new State
        {
            Position = state.Position + dxdt,
            Velocity = state.Velocity + dvdt
        };
    }

    private State AddDerivatives(State state, Derivatives derivatives, float scalar)
    {
        return new State
        {
            Position = state.Position + derivatives.dPosition_dt * scalar,
            Velocity = state.Velocity + derivatives.dVelocity_dt * scalar
        };
    }

    private Derivatives Evaluate(State state, ShellParameters parameters)
    {
        Vector3 gravityForce = new Vector3(0, -parameters.Mass * G0, 0);

        float altitude = state.Position.Y;
        float airDensity = GetAirDensity(altitude);
        float speedOfSound = GetSpeedOfSound(altitude);

        float speed = state.Velocity.Length();
        float machNumber = speed / speedOfSound;
        float dragCoefficient = GetDragCoefficient(machNumber);

        Vector3 relativeVelocity = state.Velocity - new Vector3(parameters.WindVelocity.X, 0, parameters.WindVelocity.Y);

        if (relativeVelocity.LengthSquared() < 1e-12f)
        {
            return new Derivatives { dPosition_dt = state.Velocity, dVelocity_dt = gravityForce / parameters.Mass };
        }

        float area = (float)(Math.PI * Math.Pow(parameters.Diameter / 2.0f, 2));

        float dragMagnitude = 0.5f * airDensity * relativeVelocity.LengthSquared() * dragCoefficient * area;
        Vector3 dragForce = -dragMagnitude * Vector3.Normalize(relativeVelocity);

        Vector3 totalForce = gravityForce + dragForce;
        Vector3 acceleration = totalForce / parameters.Mass;

        return new Derivatives { dPosition_dt = state.Velocity, dVelocity_dt = acceleration };
    }

    private float GetAirDensity(float altitude)
    {
        float[] h = { 0f, 11000f, 20000f, 32000f, 47000f, 51000f, 71000f, 84852f };
        float[] L = { -0.0065f, 0f, 0.001f, 0.0028f, 0f, -0.0028f, -0.002f };
        float T0 = 288.15f;
        float p0 = 101325f;

        if (altitude >= h[h.Length - 1])
        {
            return 1e-6f;
        }

        double T = T0;
        double p = p0;
        int layerIndex = 0;

        while (layerIndex < h.Length - 1 && altitude > h[layerIndex + 1])
        {
            layerIndex++;
        }

        float hBase = h[layerIndex];
        float L_i = L[layerIndex];
        double dh = altitude - hBase;

        for (int i = 0; i < layerIndex; i++)
        {
            if (Math.Abs(L[i]) < 1e-12)
            {
                p = p * Math.Exp(-G0 * (h[i + 1] - h[i]) / (R * T));
            }
            else
            {
                double T_at_top = T + L[i] * (h[i + 1] - h[i]);
                p = p * Math.Pow(T / T_at_top, G0 / (R * L[i]));
                T = T_at_top;
            }
        }

        if (Math.Abs(L_i) < 1e-12)
        {
            p = p * Math.Exp(-G0 * dh / (R * T));
        }
        else
        {
            double T_at = T + L_i * dh;
            p = p * Math.Pow(T / T_at, G0 / (R * L_i));
            T = T_at;
        }

        double rho = p / (R * T);
        if (rho <= 0) rho = 1e-6;
        return (float)rho;
    }

    private float GetSpeedOfSound(float altitude)
    {
        float[] h = { 0f, 11000f, 20000f, 32000f, 47000f, 51000f, 71000f, 84852f };
        float[] L = { -0.0065f, 0f, 0.001f, 0.0028f, 0f, -0.0028f, -0.002f };
        float T0 = 288.15f;

        if (altitude >= h[h.Length - 1])
        {
            return (float)Math.Sqrt(GAMMA * R * (216.65f + L[L.Length - 1] * (h[h.Length - 1] - h[h.Length - 2])));
        }

        double T = T0;
        int layerIndex = 0;

        while (layerIndex < h.Length - 1 && altitude > h[layerIndex + 1])
        {
            layerIndex++;
        }

        float hBase = h[layerIndex];
        float L_i = L[layerIndex];
        double dh = altitude - hBase;

        for (int i = 0; i < layerIndex; i++)
        {
            T = T + L[i] * (h[i + 1] - h[i]);
        }

        T = T + L_i * dh;

        if (T <= 0) T = 1.0;
        double a = Math.Sqrt(GAMMA * R * T);
        return (float)a;
    }

    private static readonly (double mach, double cd)[] CdTable = new[]
    {
        (0.00, 0.230), (0.40, 0.229), (0.50, 0.200), (0.60, 0.171), (0.70, 0.164),
        (0.80, 0.144), (0.825, 0.141), (0.850, 0.137), (0.875, 0.137), (0.900, 0.142),
        (0.925, 0.154), (0.950, 0.177), (0.975, 0.236), (1.000, 0.306), (1.025, 0.334),
        (1.050, 0.341), (1.075, 0.345), (1.100, 0.347), (1.150, 0.348), (1.200, 0.348),
        (1.300, 0.343), (1.400, 0.336), (1.500, 0.328), (1.600, 0.321), (1.800, 0.304),
        (2.000, 0.292), (2.200, 0.282), (2.400, 0.270), (3.000, 0.260), (3.500, 0.255),
        (4.000, 0.250), (5.000, 0.245)
    };

    private float GetDragCoefficient(float mach)
    {
        if (double.IsNaN(mach) || mach < 0.0) mach = 0.0f;

        if (mach <= CdTable[0].mach) return (float)CdTable[0].cd;
        if (mach >= CdTable[CdTable.Length - 1].mach) return (float)CdTable[CdTable.Length - 1].cd;

        for (int i = 0; i < CdTable.Length - 1; i++)
        {
            var (m1, c1) = CdTable[i];
            var (m2, c2) = CdTable[i + 1];

            if (mach >= m1 && mach <= m2)
            {
                double t = (mach - m1) / (m2 - m1);
                return (float)(c1 + t * (c2 - c1));
            }
        }

        return (float)CdTable[CdTable.Length - 1].cd;
    }
}

