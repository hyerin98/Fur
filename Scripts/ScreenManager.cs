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
    // 가로 세로 비율
    float targetWidthAspect = 16.0f;
    float targetHeightAspect = 9.0f;
 
    Camera.main.aspect = targetWidthAspect / targetHeightAspect;
 
    float widthRatio = (float)Screen.width / targetWidthAspect;
    float heightRatio = (float)Screen.height / targetHeightAspect;
 
    float heightadd = ((widthRatio / (heightRatio / 100)) - 100) / 200;
    float widthadd = ((heightRatio / (widthRatio / 100)) - 100) / 200;
 
    // 시작지점을 0으로 만들어준다.
    if (heightRatio > widthRatio)
        widthRatio = 0.0f;
    else
        heightRatio = 0.0f;
 
    Camera.main.rect = new Rect(
        Camera.main.rect.x + Mathf.Abs(widthadd),
        Camera.main.rect.x + Mathf.Abs(heightadd),
        Camera.main.rect.width + (widthadd * 2),
        Camera.main.rect.height + (heightadd * 2));
}

}
