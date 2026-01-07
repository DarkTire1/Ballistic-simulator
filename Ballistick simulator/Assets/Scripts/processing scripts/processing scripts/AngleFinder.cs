using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine; // <-- ДОБАВЛЕНО для Debug.Log

public static class LaunchAngleFinder
{
    // Устанавливаем более мелкий шаг для точности линейного перебора
    private const double StepDeg = 0.1;

    // --- Вспомогательные методы (оставляем без изменений) ---
    private static double DegreesToRadians(double deg) => deg * Math.PI / 180.0;

    // (Класс ShellParameters и ShellSimulator должны быть доступны)
    private static ShellParameters CloneParamsWithAngle(ShellParameters src, double angleRad)
    {
        return new ShellParameters
        {
            Mass = src.Mass,
            Diameter = src.Diameter,
            InitialSpeed = src.InitialSpeed,
            WindVelocity = src.WindVelocity,
            LaunchAngle = (float)angleRad,
            InitialHeight = src.InitialHeight
        };
    }

    /// <summary>
    /// Находит угол запуска снаряда, обеспечивающий минимальное горизонтальное отклонение от цели,
    /// используя линейный перебор (для сложного, немонотонного ландшафта).
    /// </summary>
    /// <returns>Наилучший угол в радианах или null, если подходящий угол не найден.</returns>
    public static double? FindBestLaunchAngle(
        ShellSimulator simulator,
        ShellParameters baseParams,
        float dt,
        List<float> landscape,
        float xStep,
        System.Numerics.Vector2 targetPos,
        float targetRadius)
    {
        double targetX = targetPos.X;

        // Переменные для отслеживания лучшего угла и минимального промаха
        double bestAngleRad = double.NaN;
        double minDeviation = double.MaxValue;

        // --- ЛИНЕЙНЫЙ ПЕРЕБОР (от 1.0 до 89.9 градусов) ---
        for (double deg = 1.0; deg < 90.0; deg += StepDeg)
        {
            double currentAngleRad = DegreesToRadians(deg);
            var p = CloneParamsWithAngle(baseParams, currentAngleRad);

            // Запуск симуляции
            var trajectory = simulator.RunSimulation(p, dt, landscape, xStep, targetPos, targetRadius);

            if (trajectory == null || trajectory.Count == 0) continue;

            // Если симуляция уже засчитала попадание в цель, мы нашли идеальный угол.
            if (trajectory[^1].HitTarget) return currentAngleRad;

            double finalRange = trajectory[^1].Position.X;

            // Если конечная дальность слишком мала (значительно меньше цели), игнорируем
            if (finalRange < targetX - targetRadius) continue;

            // Рассчитываем горизонтальное отклонение от центра цели
            double deviation = Math.Abs(finalRange - targetX);

            // Обновляем лучший угол
            if (deviation < minDeviation)
            {
                minDeviation = deviation;
                bestAngleRad = currentAngleRad;
            }
        }

        // --- Завершение поиска (Обработка неудачи) ---

        if (double.IsNaN(bestAngleRad))
        {
            // Сценарий 1: Не найдено ни одной "долетающей" траектории
            Debug.LogWarning("[FindBestLaunchAngle] Неудача: Не удалось найти ни одной траектории, которая достигла бы целевой дальности. Все снаряды, возможно, столкнулись с непреодолимым препятствием или упали слишком рано.");
            return null;
        }

        if (minDeviation > targetRadius * 5.0)
        {
            // Сценарий 2: Минимальный промах слишком велик
            Debug.LogWarning($"[FindBestLaunchAngle] Неудача: Найденный минимальный промах ({minDeviation:F2} м) превышает допустимый предел ({targetRadius * 5.0:F2} м). Цель недостижима с заданными параметрами.");
            return null;
        }

        // Возвращаем лучший найденный угол
        // Debug.Log($"[FindBestLaunchAngle] Успех (аппроксимация): Найден угол с минимальным промахом: {minDeviation:F2} м.");
        return bestAngleRad;
    }
}