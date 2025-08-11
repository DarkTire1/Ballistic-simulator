using UnityEngine;

public class ClearUi : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _shellUiElements;
    [SerializeField]
    private GameObject[] _rocketUiElements;

    public void ClearShellUiElements()
    {
        ClearUiElements(_shellUiElements);
    }

    public void ClearRocketUiElements()
    {
        ClearUiElements(_rocketUiElements);
    }

    private void ClearUiElements(GameObject[] uiElements)
    {
        foreach (var obj in uiElements)
        {
            if (obj == null) continue;

            // »щем все компоненты, реализующие интерфейс и вызываем сброс
            var resettableComponents = obj.GetComponents<IResettableUiElement>();
            foreach (var resettable in resettableComponents)
            {
                resettable.ResetUiElement();
            }
        }
    }
}


