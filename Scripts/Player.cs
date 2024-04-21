using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using IMFINE.Utils;
using IMFINE.Utils.ConfigManager;
using IMFINE.Utils.JoyStream.Communicator;
using Unity.VisualScripting;
using System;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    [SerializeField] private Animator Anim = null;
    [SerializeField] private float moveSpeed = .01f;
    private ColorManager colorManager;

    private KeyCode downKeyCode = 0;

    public bool isFalled = false;
    public string playerColor;
    public string playerID;
    public int userIndex;
    public bool isActive = false;

    // 플레이어가 액션버튼을 눌러 떨어질 때
    public Transform destination;

    public float fallDistance = 1f;
    public float fallTime = 1f;

    bool isRight;
    bool isLeft;

    // 4.15 추가
    public delegate void OnPlayerEnd(Player target);
    public event OnPlayerEnd onPlayerEnd;

    public void Awake()
    {
        colorManager = FindObjectOfType<ColorManager>();
        //playerID = System.Guid.NewGuid().ToString();
        Anim = GetComponent<Animator>();
    }

    void Start()
    {
        PlayerStart();
    }

    private void Update()
    {
        if (downKeyCode == KeyCode.UpArrow)
        {
            //transform.DOMoveY(transform.position.y + moveSpeed, 1.0f).SetRelative();
        }

        else if (downKeyCode == KeyCode.DownArrow)
        {
            transform.DOLocalMoveZ(-moveSpeed, .3f).SetRelative();
        }
        else if (downKeyCode == KeyCode.LeftArrow)
        {
            //transform.DOLocalMoveX(-moveSpeed, .3f).SetRelative();

        }
        else if (downKeyCode == KeyCode.RightArrow)
        {
            //transform.DOLocalMoveX(moveSpeed, .3f).SetRelative();
        }


        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Anim.SetTrigger("doRight");

        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Anim.SetTrigger("doLeft");
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            transform.DOMoveY(transform.position.y + (-moveSpeed), 1.0f);
            transform.DOShakeScale(1, 0.5f).SetEase(Ease.InOutFlash);
            //LeanTween.move(gameObject, destination.position, 3f).setEase(LeanTweenType.easeOutBounce);

            Vector3 targetPosition = transform.position + new Vector3(0f, -fallDistance, 0f);
            LeanTween.move(gameObject, targetPosition, fallTime)
                .setEase(LeanTweenType.easeOutBounce);

        }

    }

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            //Debug.Log("땅에 닿았음");
            isFalled = true;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bone"))
        {
            Anim.SetTrigger("doRight");
            Debug.Log("본에 닿았따");
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Bone"))
        {
            Anim.SetTrigger("doRight");
            Debug.Log("본에 닿는중");
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bone"))
        {
            Anim.SetBool("Right", false);
        }
    }

    public void OnPlayerMoveProtocol(ProtocolType protocolType)
    {
        switch (protocolType)
        {
            case ProtocolType.CONTROLLER_UP_PRESS:
                downKeyCode = KeyCode.UpArrow;
                break;
            case ProtocolType.CONTROLLER_UP_RELEASE:
                downKeyCode = KeyCode.None;
                break;
            case ProtocolType.CONTROLLER_DOWN_PRESS:
                downKeyCode = KeyCode.DownArrow;
                break;
            case ProtocolType.CONTROLLER_DOWN_RELEASE:
                downKeyCode = KeyCode.None;
                break;
            case ProtocolType.CONTROLLER_LEFT_PRESS:
                downKeyCode = KeyCode.LeftArrow;
                break;
            case ProtocolType.CONTROLLER_LEFT_RELEASE:
                downKeyCode = KeyCode.None;
                break;
            case ProtocolType.CONTROLLER_RIGHT_PRESS:
                downKeyCode = KeyCode.RightArrow;
                break;
            case ProtocolType.CONTROLLER_RIGHT_RELEASE:
                downKeyCode = KeyCode.None;
                break;
            case ProtocolType.CONTROLLER_FALL_PRESS:
                downKeyCode = KeyCode.None;
                PlayerEnd(); // 4.15 수정필
                break;
        }
    }

    public void RemovePlayer()
    {
        if (!isFalled) return;
        else
        {
            Debug.Log("삭제!");
            DOVirtual.DelayedCall(6, PlayerEnd);
        }

    }

    private void PlayerEnd()
    {
        onPlayerEnd?.Invoke(this);
        Destroy(gameObject);
    }

    private void PlayerStart()
    {
        isActive = true;
        PlayerReady();
    }

    private void PlayerReady()
    {
        // 플레이어 기본 idle동작 들어가기 
        // 애니메이션
    }

    public void SetPlayerColor(string color)
    {
        playerColor = color;
    }

    public void SetUserIndex(int index)
    {
        userIndex = index;
    }


    public string GetPlayerColor()
    {
        return playerColor;
    }

    public int GetUserIndex()
    {
        return userIndex;
    }

    public void Test()
    {
        Debug.Log(playerID + "!!!!!!!!!!!!!!!!!!!!!!!!" + GetInstanceID());
    }
}