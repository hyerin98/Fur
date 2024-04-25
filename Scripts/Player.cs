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
using Unity.Mathematics;

public class Player : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Animator Anim = null;
    private ColorManager colorManager;
    private KeyCode downKeyCode = 0;
    private Rigidbody rigid;
    public delegate void OnPlayerEnd(Player target);
    public event OnPlayerEnd onPlayerEnd;
    public Transform destination;

    [Header("Bool")]
    public bool isFalled = false;
    public bool isActive = false;

    [Header("String & Int")]
    public string playerColor;
    public string playerID;
    public int userIndex;

    [Header("PlayerMovement")]
    Vector3 originalPos;
    public float fallDistance = 1f;
    public float fallTime = 1f;
    public float moveStep = 0.3f;
    public float rotateSpeed = 5f;

    public float rotationAmount = 50f; // 한 번에 회전할 각도
public float currentRotation = 0f; // 현재 회전 각도


    [Header("DOTween")]
    public Ease ease;

    private void Awake()
    {
        colorManager = FindObjectOfType<ColorManager>();
        //playerID = System.Guid.NewGuid().ToString();
        Anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        //rigid.sleepThreshold=0;
        
    }

    void Start()
    {
        PlayerStart();
        originalPos = transform.position;
    }

    private void Update()
    {

        if (downKeyCode == KeyCode.UpArrow)
        {
            transform.DOMoveY(originalPos.y + 0.5f, 0.5f).SetEase(ease)
            .OnComplete(() =>
            {
                transform.DOMoveY(originalPos.y, 1).SetEase(ease);
            });
        }

        else if (downKeyCode == KeyCode.DownArrow)
        {
            transform.DOMoveY(originalPos.y - 0.5f, 0.5f).SetEase(ease)
            .OnComplete(() =>
            {
                transform.DOMoveY(originalPos.y, 1).SetEase(ease);
            });
        }
        else if (downKeyCode == KeyCode.LeftArrow)
        {
            rigid.rotation = Quaternion.Euler(0, 0, 50f);

        }
        else if (downKeyCode == KeyCode.RightArrow)
        {
            rigid.rotation = Quaternion.Euler(0, 0, -50f);
        }


        // 에디터에서 테스트
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.DOMoveY(originalPos.y + 0.5f, 0.5f).SetEase(ease)
            .OnComplete(() =>
            {
                transform.DOMoveY(originalPos.y, 1).SetEase(ease);
            });
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            transform.DOMoveY(originalPos.y - 0.5f, 0.5f).SetEase(ease)
            .OnComplete(() =>
            {
                transform.DOMoveY(originalPos.y, 1).SetEase(ease);
            });
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            rigid.rotation = Quaternion.Euler(0, 0, -50f);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            rigid.rotation = Quaternion.Euler(0, 0, 50f);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.rigid.isKinematic = false;
            isFalled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Ground") && isFalled)
        {
            Debug.Log("바닥과 충~돌");
            rigid.isKinematic = true;
            isFalled = false;

            foreach(var hinge in GetComponentsInChildren<HingeJoint>())
            {
                Destroy(hinge);
            }
            RemovePlayer();
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