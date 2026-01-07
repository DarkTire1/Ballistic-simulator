using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine; // Содержит UnityEngine.Vector3, UnityEngine.Vector2

public class RocketLaunchController : MonoBehaviour
{
    // --- Поля, адаптированные для ракеты ---
    public RocketParametersHolder parametersHolder;
    [SerializeField] SpawnMarkersFromPoint markerSpawner;


    [SerializeField] private RaycastProfileGenerator _profileGenerator;
    [SerializeField] private string savePath = "D:\\bal\\rocket_params.json"; // Изменено имя файла

    [Header("Внешнее приложение для запуска")]
    [SerializeField] private string externalExePath;

    // Исходная карта высот (если нужна для других целей, оставляем)
    public float[,] HeightMap;

    public void OnLaunchButtonPressed()
    {
        RocketParameters parameters = parametersHolder.GetParameters();


        SerializableVector3 startPos = new SerializableVector3(parameters.StartPosition);
        SerializableVector3 targetPos = new SerializableVector3(parameters.TargetPosition);

        Vector3 startPosUnity = startPos.ToUnityVector();
        Vector3 targetPosUnity = targetPos.ToUnityVector();

        float targetXInProfile;

        List<float> heightProfile = GetHeightProfileWithTargetX(startPosUnity, targetPosUnity, out targetXInProfile);

        if (heightProfile == null || heightProfile.Count == 0)
        {
            UnityEngine.Debug.LogWarning("Карта висот відсутня");
            return;
        }

        var dataToSave = new RocketProfileData
        {
            Rocket = new SerializableRocketParameters(parameters),
            TargetXInProfile = targetXInProfile,
            HeightProfile = heightProfile
        };

        try
        {
            string json = JsonUtility.ToJson(dataToSave, true);
            File.WriteAllText(savePath, json);
            UnityEngine.Debug.Log($"Параметры ракети і карта висот збережені: {savePath}");
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError($"Помилка при збереженні JSON: {e.Message}");
        }

        markerSpawner.rayDistance = parameters.RadiusOfDestruction;
        markerSpawner.SpawnRays(targetPosUnity);

        LaunchExternalExe();
    }

    // --- Методы, перенесенные из ShellLaunchController ---

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

        // Вызов метода генератора профиля
        return _profileGenerator.GetRaycastHeightProfile(startPos, targetPos, out targetXInProfile);
    }

    // --- Вспомогательные структуры (Адаптеры для сериализации) ---

    // Структура для сохранения данных ракеты и профиля
    [System.Serializable]
    public class RocketProfileData
    {
        public SerializableRocketParameters Rocket;
        public float TargetXInProfile;
        public List<float> HeightProfile;
    }

    // Обёртка для RocketParameters с Serializable Vector3
    [System.Serializable]
    public class SerializableRocketParameters
    {
        public float StartMass;
        public float EndMass;
        public float AverageThrust;
        public float ThrustTime;
        public float RadiusOfDestruction;
        public float Diameter;
        public SerializableVector3 StartPosition;
        public SerializableVector3 TargetPosition;

        public SerializableRocketParameters() { }

        public SerializableRocketParameters(RocketParameters p)
        {
            StartMass = p.StartMass;
            EndMass = p.EndMass;
            AverageThrust = p.AverageThrust;
            ThrustTime = p.ThrustTime;
            RadiusOfDestruction = p.RadiusOfDestruction;
            Diameter = p.Diameter;
            // Преобразуем System.Numerics.Vector3 в SerializableVector3
            StartPosition = new SerializableVector3(p.StartPosition);
            TargetPosition = new SerializableVector3(p.TargetPosition);
        }
    }

    // Serializable обёртка для Vector3 (System.Numerics -> Unity)
    [System.Serializable]
    public class SerializableVector3
    {
        public float X;
        public float Y;
        public float Z;

        public SerializableVector3() { }

        // Конструктор для System.Numerics.Vector3
        public SerializableVector3(System.Numerics.Vector3 v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
        }

        public Vector3 ToUnityVector() => new Vector3(X, Y, Z);
    }

    // Serializable обёртка для Vector2 (System.Numerics -> Unity)
    [System.Serializable]
    public class SerializableVector2
    {
        public float X;
        public float Y;

        public SerializableVector2() { }

        // Конструктор для System.Numerics.Vector2
        public SerializableVector2(System.Numerics.Vector2 v)
        {
            X = v.X;
            Y = v.Y;
        }

        public Vector2 ToUnityVector() => new Vector2(X, Y);
    }
}