using UnityEngine;
using DG.Tweening;

public class SceneEvent : MonoBehaviour
{
    public float moveDistance = 0.1f;
    public float moveDuration = 60f;

    public Ease ease;

    void Start()
    {
        // x축으로 왔다갔다하는 애니메이션을 설정
        Vector3 targetPositionRight = transform.position + new Vector3(moveDistance, 0, 0);
        Vector3 targetPositionLeft = transform.position - new Vector3(moveDistance, 0, 0);

        // 반복해서 왔다갔다하는 애니메이션을 DOTween을 사용하여 설정
        transform.DOMoveX(targetPositionRight.x, moveDuration)
            .SetEase(ease)
            .SetLoops(-1, LoopType.Yoyo);
    }
}
