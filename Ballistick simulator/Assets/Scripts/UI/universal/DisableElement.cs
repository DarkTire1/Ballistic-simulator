using UnityEngine;

public class DisableElement : MonoBehaviour
{
    [SerializeField]
    private GameObject[] uiElements;

    public void SetUiActive(bool isActive)
    {

        foreach (GameObject uiElement in uiElements)
        {
            if (uiElement != null)
            {
                uiElement.SetActive(isActive);
            }
        }
    }

    public static void SetUiActiveStatic(GameObject[] uiElements, bool isActive)
    {
        foreach (GameObject uiElement in uiElements)
        {
            if (uiElement != null)
            {
                uiElement.SetActive(isActive);
            }
        }
    }

    public void EnableUi() => SetUiActive(true);
    

    public void DisableUi() => SetUiActive(false);
}
