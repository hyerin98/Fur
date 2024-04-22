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

    
    public Transform destination;

    // 플레이어가 액션버튼을 눌러 떨어질 때
    public float fallDistance = 1f;
    public float fallTime = 1f;

    public float moveStep = 0.3f;
    private Rigidbody rigid;

    public GameObject childWithHinge; 
    private HingeJoint hinge;

    // 4.15 추가
    public delegate void OnPlayerEnd(Player target);
    public event OnPlayerEnd onPlayerEnd;

    public void Awake()
    {
        colorManager = FindObjectOfType<ColorManager>();
        //playerID = System.Guid.NewGuid().ToString();
        Anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
    }

    void Start()
    {
        PlayerStart();

         if (childWithHinge != null)
        {
            hinge = childWithHinge.GetComponent<HingeJoint>();
        }
    }

    private void Update()
    {
        if (downKeyCode == KeyCode.UpArrow)
        {
            //transform.DOMoveY(transform.position.y + moveSpeed, 1.0f).SetRelative();
            Anim.SetTrigger("doUp");
        }

        else if (downKeyCode == KeyCode.DownArrow)
        {
            Anim.SetTrigger("doDown");
        }
        else if (downKeyCode == KeyCode.LeftArrow)
        {
            Anim.SetTrigger("doLeft");

        }
        else if (downKeyCode == KeyCode.RightArrow)
        {
            Anim.SetTrigger("doRight");
        }


        // 에디터에서 테스트
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Vector3 newPosition = rigid.position + new Vector3(0, moveStep, 0);
            rigid.MovePosition(newPosition);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Vector3 newPosition = rigid.position + new Vector3(0, -moveStep, 0);
            rigid.MovePosition(newPosition);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Vector3 newPosition = rigid.position + new Vector3(moveStep, 0, 0);
            rigid.MovePosition(newPosition);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Vector3 newPosition = rigid.position + new Vector3(-moveStep, 0, 0);
            rigid.MovePosition(newPosition);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // transform.DOMoveY(transform.position.y + (-moveSpeed), 1.0f);

            // Vector3 targetPosition = transform.position + new Vector3(0f, -fallDistance, 0f);
            // LeanTween.move(gameObject, targetPosition, fallTime)
            //     .setEase(LeanTweenType.easeOutQuint);

            this.rigid.isKinematic = false;
            isFalled = true;
        }
    }

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground") && isFalled) // 씬 오브젝트에 부모객체 콜라이더 추가하기
        {
            Debug.Log("땅에 닿았음");
            this.rigid.isKinematic = true;
            isFalled = false;
        }

        if (other.gameObject.CompareTag("Ground"))
        {
            // 이 오브젝트 및 모든 자식 오브젝트에서 HingeJoint 컴포넌트를 찾아 제거
            foreach (var hinge in GetComponentsInChildren<HingeJoint>())
            {
                Destroy(hinge);
                Destroy(this);
            }

            // 충돌 시 시각적 효과가 필요하면 여기에 코드 추가
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bone"))
        {
            Debug.Log("본에 닿았따");
            //Anim.SetTrigger("doPlaying");
        }
    }
    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Bone"))
        {
            //Anim.SetTrigger("doPlaying");
        }
    }


    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bone"))
        {
            //Anim.SetBool("doPlaying", false);
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