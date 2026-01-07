using UnityEngine;
using System.Collections.Generic;

public class RaycastProfileGenerator : MonoBehaviour
{


    public float ProfileStep = 1.0f;
    public float RaycastStartHeight = 5000f;
    public float MaxProfileDistance = 5000f;

    public List<float> GetRaycastHeightProfile(Vector3 startPos, Vector3 targetPos, out float targetXInProfile)
    {
        List<float> profile = new List<float>();

        Vector2 startXZ = new Vector2(startPos.x, startPos.z);
        Vector2 targetXZ = new Vector2(targetPos.x, targetPos.z);
        Vector2 directionXZ = (targetXZ - startXZ).normalized;

        float rayLength = RaycastStartHeight * 2;
        float currentDistance = 0f;
        targetXInProfile = -1f; // значение по умолчанию, если цель не попадает в профиль

        while (profile.Count < 5000)
        {
            Vector2 currentXZ = startXZ + directionXZ * currentDistance;
            Vector3 rayOrigin = new Vector3(currentXZ.x, RaycastStartHeight, currentXZ.y);

            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, rayLength))
            {
                profile.Add(hit.point.y);

                // Если цель находится между текущей и предыдущей точкой, запоминаем индекс по X
                if (targetXInProfile < 0f && currentXZ.x >= targetPos.x)
                {
                    targetXInProfile = currentDistance; // или можно profile.Count - 1 для индекса массива
                }
            }
            else
            {
                
                break;
            }

            currentDistance += ProfileStep; // шаг фиксированный
        }

        return profile;
    }


}