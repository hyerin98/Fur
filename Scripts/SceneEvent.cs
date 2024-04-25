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
    public float surprise_moveDistance = 20f; // Z축으로 이동할 거리
    public float surprise_moveDuration = 10f; // 이동하는데 걸리는 시간
    public float surprise_delayDuration = 10f; // 원래 위치로 돌아간 후 대기하는 시간
    private Vector3 surprise_originalPosition;

    [Header("LightObj")]
    public float light_moveDistance =20f;
    public float light_moveDuration = 5f;
    public float light_delayDuration = 10f;
    private Vector3 light_originalPosition;
    

    [Header("Bool")]
    public bool isLight;
    public bool isSurprise;

    [Header("DOTween")]
    public Ease ease;

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
            transform.DOMoveZ(surprise_originalPosition.z + -surprise_moveDistance, surprise_moveDuration).SetEase(Ease.InElastic)
                .OnComplete(() =>
                {
                    transform.DOMoveZ(surprise_originalPosition.z, surprise_moveDuration).SetEase(Ease.InElastic)
                        .OnComplete(() =>
                        {
                            DOVirtual.DelayedCall(surprise_delayDuration, MoveObjects);
                        });
                });
        }

        else if (isLight)
        {
            transform.DOMoveX(light_originalPosition.x + light_moveDistance, light_delayDuration).SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                transform.DOMoveX(light_originalPosition.x, light_delayDuration).SetEase(Ease.InQuad)
                .OnComplete(()=>
                {
                    DOVirtual.DelayedCall(light_delayDuration, MoveObjects);
                });
            });
        }
    }
}