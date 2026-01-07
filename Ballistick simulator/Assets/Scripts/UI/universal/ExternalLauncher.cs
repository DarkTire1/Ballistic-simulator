using UnityEngine;
using System.Diagnostics; // Для Process

public class ExternalLauncher : MonoBehaviour
{
    [Header("Путь к исполняемому файлу (.exe)")]
    public string exePath;
    public void Launch()
    {
        if (string.IsNullOrEmpty(exePath))
        {
            UnityEngine.Debug.LogError("Путь к программе не указан!");
            return;
        }

        try
        {
            Process.Start(exePath);
            UnityEngine.Debug.Log($"Программа запущена: {exePath}");
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError($"Не удалось запустить файл: {e.Message}");
        }
    }
}
