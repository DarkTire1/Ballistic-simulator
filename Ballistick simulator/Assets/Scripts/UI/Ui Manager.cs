using UnityEngine;

public class UiManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] uiElments;
    private bool isUiHidden;
    private void SetUiActive(bool isActive)
    {
        foreach (GameObject uiElement in uiElments)
        {
            if (uiElement != null)
            {
                uiElement.SetActive(isActive);
            }
        }
    }

    public void EnableUi() => SetUiActive(true);
    public void DisableUi() => SetUiActive(false);
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (isUiHidden)
                EnableUi();
            else
                DisableUi();
            isUiHidden = !isUiHidden;
        }
    }
}
