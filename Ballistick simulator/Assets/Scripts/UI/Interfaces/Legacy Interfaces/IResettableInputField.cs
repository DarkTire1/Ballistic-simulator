using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_InputField))]
public class ResettableTMPInputField : MonoBehaviour, IResettableUiElement
{
    [SerializeField, HideInInspector]
    private TMP_InputField _inputField;

#if UNITY_EDITOR
    // Кешируем ссылку прямо в инспекторе, чтобы она была и в prefab
    private void OnValidate()
    {
        if (_inputField == null)
        {
            _inputField = GetComponent<TMP_InputField>();
            UnityEditor.EditorUtility.SetDirty(this); // Чтобы prefab сохранил изменения
        }
    }
#endif

    private void Awake()
    {
        // На случай, если кто-то удалил ссылку руками
        if (_inputField == null)
            _inputField = GetComponent<TMP_InputField>();
    }

    public void ResetUiElement()
    {
        // Тут точно null не будет
        _inputField.text = string.Empty;
    }
}


