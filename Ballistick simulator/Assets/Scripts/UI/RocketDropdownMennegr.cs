using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RocketDropdownMenneger : MonoBehaviour
{
    public TMP_Dropdown ammoDropdown;
    public void OnValueChanged(int index)
    {

    }
    public void Start()
    {
        ammoDropdown.options.RemoveAt(2);
    }
    public void AddDefaultOption()
    {
        // Добавляем новый пункт в КОНЕЦ списка
        ammoDropdown.options.Add(new TMP_Dropdown.OptionData("Обрати тип"));

        // Выбираем его (он последний)
        ammoDropdown.value = ammoDropdown.options.Count - 1;

        ammoDropdown.RefreshShownValue();
        ammoDropdown.options.RemoveAt(2);
    }
}
