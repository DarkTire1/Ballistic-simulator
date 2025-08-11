using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Dropdown))]
public class ResettableTMPDropdown : MonoBehaviour, IResettableUiElement
{
    [SerializeField, HideInInspector]
    private TMP_Dropdown _dropdown;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_dropdown == null)
        {
            _dropdown = GetComponent<TMP_Dropdown>();
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
#endif

    private void Awake()
    {
        if (_dropdown == null)
            _dropdown = GetComponent<TMP_Dropdown>();
    }

    public void ResetUiElement()
    {
        // —брасываем в первый элемент списка
        if (_dropdown.options.Count > 0)
            _dropdown.SetValueWithoutNotify(0);

        _dropdown.RefreshShownValue();
    }
}


