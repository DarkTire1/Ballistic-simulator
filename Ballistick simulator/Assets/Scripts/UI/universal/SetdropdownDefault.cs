using UnityEngine;
using TMPro;

public class SetdropdownDefault : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown dropdown;
    public void AddDefaultOption()
    {
        // Добавляем новый пункт в КОНЕЦ списка
        dropdown.options.Add(new TMP_Dropdown.OptionData("Обрати тип"));

        // Выбираем его (он последний)
        dropdown.value = dropdown.options.Count - 1;

        dropdown.RefreshShownValue();
        dropdown.options.RemoveAt(2);
    }
}
