using UnityEngine;

public class UiManager : MonoBehaviour
{
    private bool isUiActive = true;
    [SerializeField]
    private GameObject[] uiElements;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (isUiActive)
            {
                DisableElement.SetUiActiveStatic(uiElements, false);
                isUiActive = false;
            }
            else
            {
                DisableElement.SetUiActiveStatic(uiElements, true);
                isUiActive = true;
            }
        }
    }
}
