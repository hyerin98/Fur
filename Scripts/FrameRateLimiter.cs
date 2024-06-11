using UnityEngine;

public class FrameRateLimiter : MonoBehaviour
{
    void Start()
    {
        // 프레임레이트를 60으로 고정
        Application.targetFrameRate = 60;
        
        // VSync 설정
        QualitySettings.vSyncCount = 1;
    }

    void Update()
    {
        // 현재 프레임레이트 확인
        Debug.Log("Current Frame Rate: " + (1.0f / Time.deltaTime));
    }
}
