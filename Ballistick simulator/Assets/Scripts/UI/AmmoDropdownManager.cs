using UnityEngine;
using TMPro;

public class AmmoDropdownManager : MonoBehaviour
{
    public GameObject shellWindow;
    public GameObject rocketWindow;
    public TMP_Dropdown ammoDropdown;
    public bool isStart = true;
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
}
