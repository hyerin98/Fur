using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Test : MonoBehaviour
{
    [SerializeField] float moveY = 0.1f;
    Material material;
    Renderer renderer;

    private void Start()
    {
        float initPosY = transform.localPosition.y;
        transform.DOLocalMoveY(initPosY, 1).From(initPosY + moveY).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        renderer = GetComponent<Renderer>();

        Show();
        Hide();
    }

    void Update()
    {
        //Show();
        //Hide();
    }

    void Show()
    {
        material = renderer.material;
        material.DOOffset(Vector3.zero, 3).SetEase(Ease.Linear);
    }

    void Hide()
    {
        material = renderer.material;
        material.DOOffset(Vector3.down, 3).SetEase(Ease.Linear);
    }

}
