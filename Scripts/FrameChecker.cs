using System.Collections;
using UnityEngine;

public class FrameChecker : MonoBehaviour
{
    private float deltaTime = 0.0f;
    private GUIStyle style;
    private Rect rect;
    private float msec;
    private float fps;
    private float worstFps = 100f;
    private string text;

    void Awake()
    {
        int w = Screen.width, h = Screen.height;
        rect = new Rect(0, 0, w, h * 2 / 100); // 텍스트 사이즈를 적절히 조정
        style = new GUIStyle
        {
            alignment = TextAnchor.UpperLeft,
            fontSize = h * 4 / 100, // 텍스트 크기를 적절히 조정
            normal = { textColor = Color.cyan }
        };
        StartCoroutine(WorstReset());
    }

    IEnumerator WorstReset() // 15초 간격으로 최저 프레임을 리셋하는 코루틴
    {
        while (true)
        {
            yield return new WaitForSeconds(15f);
            worstFps = 100f;
        }
    }

    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float newFps = 1.0f / deltaTime;
        if (newFps < worstFps)
        {
            worstFps = newFps;
        }
    }

    void OnGUI()
    {
        msec = deltaTime * 1000.0f;
        fps = 1.0f / deltaTime;
        text = $"{msec:F1}ms ({fps:F1} FPS) - worst: {worstFps:F1}";
        GUI.Label(rect, text, style);
    }
}
