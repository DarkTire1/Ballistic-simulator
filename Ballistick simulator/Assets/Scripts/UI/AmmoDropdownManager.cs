using UnityEngine;
using TMPro;

public class AmmoDropdownManager : MonoBehaviour
{
    public GameObject shellWindow;
    public GameObject rocketWindow;
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
}
