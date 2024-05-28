using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IMFINE.Utils;
using IMFINE.Utils.ConfigManager;
using IMFINE.Utils.JoyStream.Communicator;
using UnityEngine.UIElements;
using DG.Tweening;

public class FurManager : MonoSingleton<FurManager>
{
    public float moveDistance = 20f; // Z축으로 이동할 거리
    public float moveDuration = 1f; // 이동하는데 걸리는 시간
    public float delayDuration = 30f; // 원래 위치로 돌아간 후 대기하는 시간

    private Vector3 surprise_originalPosition;
    private Vector3 light_originalPosition;
    public Ease ease;

    public bool isLight;
    public bool isSurprise;

    void Start()
    {
        surprise_originalPosition = transform.position;
        MoveObject();
    }

    void MoveObject()
    {
        if (isSurprise)
        {
            transform.DOMoveZ(surprise_originalPosition.z + -moveDistance, moveDuration).SetEase(Ease.InElastic)
                // 원래 위치로 돌아오기
                .OnComplete(() =>
                {
                    transform.DOMoveZ(surprise_originalPosition.z, moveDuration).SetEase(Ease.InElastic)
                        // 일정 시간 딜레이 후 함수 재호출
                        .OnComplete(() =>
                        {
                            DOVirtual.DelayedCall(delayDuration, MoveObject);
                        });
                });
        }

        else if (isLight)
        {
            transform.DOMoveX(light_originalPosition.x + moveDistance, moveDuration).SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                transform.DOMoveX(light_originalPosition.x, moveDuration).SetEase(Ease.InQuad)
                .OnComplete(()=>
                {
                    DOVirtual.DelayedCall(delayDuration, MoveObject);
                });
            });
        }
    }
}