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
    [Header("SurpriseObj")]
    public float surprise_moveDistance;
    public float surprise_moveDuration;
    public float surprise_delayDuration;
    private Vector3 surprise_originalPosition;
    public float surprise_waitTime;

    [Header("LightObj")]
    public float light_moveDistance;
    public float light_moveDuration;
    public float light_delayDuration;
    private Vector3 light_originalPosition;
    public float light_waitTime;


    [Header("Bool")]
    public bool isLight;
    public bool isSurprise;

    [Header("DOTween")]
    public Ease surprise_ease;
    public Ease light_ease;

    void Start()
    {
        surprise_originalPosition = transform.position;
        light_originalPosition = transform.position;
        MoveObjects();
    }

    void MoveObjects()
    {
        if (isSurprise)
        {
            transform.DOMoveZ(surprise_originalPosition.z + -surprise_moveDistance, surprise_moveDuration)
                .SetEase(surprise_ease)
                .OnComplete(() =>
                {
                    // 목표 위치에서 원래 위치로 복귀
                    transform.DOMoveZ(surprise_originalPosition.z, surprise_moveDuration)
                        .SetEase(surprise_ease)
                        .OnComplete(() =>
                        {
                            // 원래 위치에서 대기
                            DOVirtual.DelayedCall(surprise_waitTime, () =>
                            {
                                // 대기 후 원하는 추가 동작 실행
                                MoveObjects();
                            });
                        });
                });
        }
        else if (isLight)
        {
            transform.DOMoveX(light_originalPosition.x + light_moveDistance, light_delayDuration)
                .SetEase(light_ease)
                .OnComplete(() =>
                {
                    DOVirtual.DelayedCall(light_waitTime, () => // 도착 후 waitTime만큼 대기!
                    {
                        transform.DOMoveX(light_originalPosition.x, light_delayDuration) // 대기가 끝난 후 원래 위치로
                            .SetEase(light_ease)
                            .OnComplete(() =>
                            {
                                MoveObjects(); // 원래 위치로 돌아온 후 다시 반복
                            });
                    });
                });
        }
    }
}
