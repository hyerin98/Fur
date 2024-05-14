using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using IMFINE.Utils;
using IMFINE.Utils.ConfigManager;
using IMFINE.Utils.JoyStream.Communicator;
using UnityEngine.UIElements;
using Unity.Mathematics;

public class SceneEvent : MonoBehaviour
{
    public bool isLight = false;

    public Vector3 light_originalPosition;
    public float light_moveDistance;
    public float light_delayDuration;
    public Ease light_ease;
    public float light_waitTime;

    void Start()
    {
        if (isLight)
        {
            DOVirtual.DelayedCall(light_waitTime, StartLightMovement);
        }
    }

    void StartLightMovement()
{
    transform.DOMoveX(light_originalPosition.x + light_moveDistance, light_delayDuration)
        .SetEase(light_ease)
        .OnComplete(() =>
        {
            // 도착 후 일정 시간 대기 후 원래 위치로 돌아가기
            DOVirtual.DelayedCall(light_waitTime, () =>
            {
                transform.DOMoveX(light_originalPosition.x, light_delayDuration)
                    .SetEase(light_ease)
                    .OnComplete(() =>
                    {
                        // 다시 처음부터 반복
                        DOVirtual.DelayedCall(light_waitTime, StartLightMovement);
                    });
            });
        });
}


}
