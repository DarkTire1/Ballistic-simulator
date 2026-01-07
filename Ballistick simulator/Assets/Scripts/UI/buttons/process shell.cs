using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine; // Содержит UnityEngine.Vector3, UnityEngine.Vector2

public class ShellLaunchController : MonoBehaviour
{
    public ShellParametersHolder Holder;
    [SerializeField] SpawnMarkersFromPoint markerSpawner;
    ShellSimulator simulator = new ShellSimulator();

    [SerializeField] private RaycastProfileGenerator _profileGenerator;
    [SerializeField] private string savePath = "D:\\bal\\params.json";

    [Header("Внешнее приложение для запуска")]
    [SerializeField] private string externalExePath;

    public void OnButtonPressed()
    {
        ShellParameters shell = Holder.GetParameters();

        // --- Подготовка координат ---
        SerializableVector3 startPos = new SerializableVector3(shell.StartPosition);
        SerializableVector3 targetPos = new SerializableVector3(shell.TargetPosition);

        Vector3 startPosUnity = startPos.ToUnityVector();
        Vector3 targetPosUnity = targetPos.ToUnityVector();

        float targetXInProfile;
        List<float> heightProfile = GetHeightProfileWithTargetX(startPosUnity, targetPosUnity, out targetXInProfile);

        if (heightProfile == null || heightProfile.Count == 0)
        {
            UnityEngine.Debug.LogWarning("Профиль высот отсутствует.");
            return;
        }

        // --- Сохранение данных ---
        var dataToSave = new ShellProfileData
        {
            Shell = new SerializableShellParameters(shell),
            TargetXInProfile = targetXInProfile,
            HeightProfile = heightProfile
        };

        try
        {
            string json = JsonUtility.ToJson(dataToSave, true);
            File.WriteAllText(savePath, json);
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError($"Ошибка при сохранении JSON: {e.Message}");
        }

        // --- Запуск визуализации маркеров ---
        markerSpawner.rayDistance = shell.RadiusOfDestruction;
        markerSpawner.SpawnRays(targetPosUnity);

        // --- Запуск внешнего .exe ---
        LaunchExternalExe();
    }

    private void LaunchExternalExe()
    {
        if (string.IsNullOrEmpty(externalExePath))
        {
            UnityEngine.Debug.LogWarning("Путь к внешнему приложению не указан!");
            return;
        }

        try
        {
            System.Diagnostics.Process.Start(externalExePath);
            UnityEngine.Debug.Log($"Запущено внешнее приложение: {externalExePath}");
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError($"Не удалось запустить файл: {e.Message}");
        }
    }

    private List<float> GetHeightProfileWithTargetX(Vector3 startPos, Vector3 targetPos, out float targetXInProfile)
    {
        if (_profileGenerator == null)
        {
            UnityEngine.Debug.LogError("RaycastProfileGenerator не назначен.");
            targetXInProfile = -1f;
            return null;
        }

        return _profileGenerator.GetRaycastHeightProfile(startPos, targetPos, out targetXInProfile);
    }

    [System.Serializable]
    public class ShellProfileData
    {
        public SerializableShellParameters Shell;
        public float TargetXInProfile;
        public List<float> HeightProfile;
    }

    // --- Обёртка для ShellParameters с Serializable Vector3 ---
    [System.Serializable]
    public class SerializableShellParameters
    {
        public float Mass;
        public float Diameter;
        public float InitialSpeed;
        public SerializableVector2 WindVelocity;
        public float InitialHeight;
        public float LaunchAngle;
        public SerializableVector3 StartPosition;
        public SerializableVector3 TargetPosition;

        public SerializableShellParameters() { }

        public SerializableShellParameters(ShellParameters p)
        {
            Mass = p.Mass;
            Diameter = p.Diameter;
            InitialSpeed = p.InitialSpeed;
            WindVelocity = new SerializableVector2(p.WindVelocity);
            InitialHeight = p.InitialHeight;
            LaunchAngle = p.LaunchAngle;
            StartPosition = new SerializableVector3(p.StartPosition);
            TargetPosition = new SerializableVector3(p.TargetPosition);
        }
    }

    // --- Serializable обёртка для Vector3 ---
    [System.Serializable]
    public class SerializableVector3
    {
        public float X;
        public float Y;
        public float Z;

        public SerializableVector3() { }

        public SerializableVector3(System.Numerics.Vector3 v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
        }

        public Vector3 ToUnityVector() => new Vector3(X, Y, Z);
    }

    // --- Serializable обёртка для Vector2 ---
    [System.Serializable]
    public class SerializableVector2
    {
        public float X;
        public float Y;

        public SerializableVector2() { }

        public SerializableVector2(System.Numerics.Vector2 v)
        {
            X = v.X;
            Y = v.Y;
        }

        public Vector2 ToUnityVector() => new Vector2(X, Y);
    }
}
