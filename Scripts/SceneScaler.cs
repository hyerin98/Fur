using UnityEngine;

public class SceneScaler : MonoBehaviour
{
    void Start()
    {
        ScaleObjectsToFitScreen();
    }

    void ScaleObjectsToFitScreen()
    {
        float targetAspect = 9.0f / 16.0f;
        float currentAspect = (float)Screen.width / (float)Screen.height;

        // 현재 화면 비율과 타겟 비율을 비교하여 오브젝트의 스케일을 조정합니다.
        if (currentAspect < targetAspect)
        {
            // 현재 화면이 타겟보다 좁은 경우 (가로가 길어야 함)
            float scaleRatio = targetAspect / currentAspect;

            // 모든 오브젝트의 스케일을 조정합니다.
            foreach (Transform child in transform)
            {
                child.localScale *= scaleRatio;
            }
        }
        else
        {
            // 현재 화면이 타겟과 같거나 더 넓은 경우 (세로가 길어야 함)
            float scaleRatio = currentAspect / targetAspect;

            // 모든 오브젝트의 스케일을 조정합니다.
            foreach (Transform child in transform)
            {
                child.localScale /= scaleRatio;
            }
        }
    }
}
