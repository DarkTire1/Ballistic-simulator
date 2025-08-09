using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClearUi : MonoBehaviour
{
    [SerializeField]
    private GameObject[] uiElements;

    public void ClearUiElements()
    {
        foreach (GameObject uiElement in uiElements)
        {
            if (uiElement != null)
            {
                ResetUIElement(uiElement);
            }
        }
    }
    private static void ResetUIElement(GameObject obj)
    {
        // Берем все нужные компоненты и обрабатываем каждый
        foreach (var component in obj.GetComponents<Component>())
        {
            switch (component)
            {
                case TMP_InputField tmpInput:
                    tmpInput.text = "";
                    break;

                case InputField legacyInput:
                    legacyInput.text = "";
                    break;

                case TMP_Dropdown tmpDropdown:
                    tmpDropdown.value = 0;
                    break;

                case Dropdown legacyDropdown:
                    legacyDropdown.value = 0;
                    break;

                case Toggle toggle:
                    toggle.isOn = false;
                    break;

                case Slider slider:
                    slider.value = slider.minValue;
                    break;

                case TextMeshProUGUI tmpText:
                    tmpText.text = "";
                    break;

                case Text legacyText:
                    legacyText.text = "";
                    break;

                case Image image:
                    image.color = Color.white;
                    break;
            }
        }
    }
}

