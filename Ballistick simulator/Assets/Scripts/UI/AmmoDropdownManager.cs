using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AmmoDropdownManager : MonoBehaviour
{
    public GameObject shellWindow;
    public GameObject rocketWindow;
    public TMP_Dropdown ammoDropdown;
    public void OnValueChanged(int index)
    {
        if (index == 0)
        {
            shellWindow.SetActive(true);
            rocketWindow.SetActive(false);
        }
        if (index == 1)
        {
            rocketWindow.SetActive(true);
            shellWindow.SetActive(false);
        }
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
