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
            downKeyCode = KeyCode.UpArrow;
            Anim.SetBool("Right", true);
            Anim.SetBool("Left", false);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            downKeyCode = KeyCode.UpArrow;
            Anim.SetBool("Left", true);
            Anim.SetBool("Right", false);
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
            Anim.SetBool("Right", true);
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Bone"))
        {
            Anim.SetBool("Right", true);
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


    // public void AssignUserColor()
    // {
    //     string userColor = ColorManager.instance.AssignUserColor();

    //     if (userColor != null)
    //     {
    //         SetPlayerColor(userColor);
    //         userIndex = ColorManager.instance.GetUserIndex(); // ColorManager에서 사용자의 인덱스 가져오기
    //         Debug.Log(userIndex + " 번째 유저에게 할당된 컬러 : " + userColor);

    //         //playerassignedColors.Add(userColor); // 할당된 컬러를 playerassignedColors 리스트에 추가
    //     }
    //     else
    //     {
    //         Debug.Log(userIndex + " 번째 유저에게 할당할 컬러가 없습니다.");
    //     }
    // }

    // public void RemoveUserAtIndex(int index)
    // {
    //     string userColor = colorManager.GetAssignedColorAtIndex(index);
    //     colorManager.RemoveUserAtIndex(index);
    //     Debug.Log("유저 " + index + "가 삭제되었습니다. 할당된 컬러: " + userColor);
    //     // 삭제된 유저의 컬러를 playerassignedColors 리스트에서 제거
    //     playerassignedColors.RemoveAt(index);
    // }

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