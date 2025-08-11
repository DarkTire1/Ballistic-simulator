using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class ResettableTMPText : MonoBehaviour, IResettableUiElement
{
    [SerializeField, HideInInspector]
    private TMP_Text _text;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_text == null)
        {
            _text = GetComponent<TMP_Text>();
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
#endif

    private void Awake()
    {
        if (_text == null)
            _text = GetComponent<TMP_Text>();
    }

    public void ResetUiElement()
    {
        _text.text = string.Empty;
    }
}


