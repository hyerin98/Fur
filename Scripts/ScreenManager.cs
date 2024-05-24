using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    void Awake() 
    {
        Screen.SetResolution(1920,1080,true);   
        ResolutionFix();
    }

    void Start()
    {
        Application.targetFrameRate = 60;
    }

    private void ResolutionFix()
{
    // 목표 비율
    float targetWidthAspect = 16.0f;
    float targetHeightAspect = 9.0f;

    // 현재 화면의 비율
    float currentAspect = (float)Screen.width / Screen.height;

    // 현재 화면이 목표 비율보다 가로로 더 긴 경우
    if (currentAspect > targetWidthAspect)
    {
        // 카메라의 시야를 위아래로 잘라냅니다.
        float widthRatio = targetWidthAspect / currentAspect;
        Camera.main.rect = new Rect(0, (1 - widthRatio) / 2, 1, widthRatio);
    }
    else // 현재 화면이 목표 비율보다 세로로 더 긴 경우
    {
        // 카메라의 시야를 좌우로 잘라냅니다.
        float heightRatio = currentAspect / targetHeightAspect;
        Camera.main.rect = new Rect((1 - heightRatio) / 2, 0, heightRatio, 1);
    }
}


}
