using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using IMFINE.Utils.JoyStream.Communicator;
using System;

public class Player : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Animator Anim = null;
    private KeyCode downKeyCode = 0;
    public Rigidbody rigid;
    public delegate void OnPlayerEnd(Player target);
    public event OnPlayerEnd onPlayerEnd;
    public Transform destination;
    //public PlayerData playerData;

    public PlayerSelector playerSelector;

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
    public float torqueAmount = 50f; // 회전 토크 크기
    public float rotateSpeedd = 90f;

    public float forceMagnitude = 10f;
    public float pullMagnitude = 20f;
    private HingeJoint[] hingeJoints;


    [Header("DOTween")]
    public Ease ease;

    private void Awake()
    {
        //playerID = System.Guid.NewGuid().ToString();
        Anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        //rigid.sleepThreshold=0;
       hingeJoints = GetComponentsInChildren<HingeJoint>();

        playerSelector = FindObjectOfType<PlayerSelector>();
        if (playerSelector == null)
        {
            Debug.LogError("PlayerSelector component not found in the scene!");
        }
    }

    void Start()
    {
        PlayerStart();
        originalPos = transform.position;
        GetComponent<Player>().enabled = false; // 4.30 테스트 -> 생성된 털들만 움직이도록
    }

    private void Update()
    {
        // if (downKeyCode == KeyCode.UpArrow)
        // {
        //     transform.DOMoveY(originalPos.y + 1f, 0.5f).SetEase(ease)
        //     .OnComplete(() =>
        //     {
        //         transform.DOMoveY(originalPos.y, 1).SetEase(ease);
        //     });
        // }

        // else if (downKeyCode == KeyCode.DownArrow)
        // {
        //     transform.DOMoveY(originalPos.y - 1f, 0.5f).SetEase(ease)
        //     .OnComplete(() =>
        //     {
        //         transform.DOMoveY(originalPos.y, 1).SetEase(ease);
        //     });
        // }
        // else if (downKeyCode == KeyCode.LeftArrow)
        // {
        //     rigid.rotation = Quaternion.Euler(0, 0, 30f);

        // }
        // else if (downKeyCode == KeyCode.RightArrow)
        // {
        //     rigid.rotation = Quaternion.Euler(0, 0, -30f);
        // }
        // else if(downKeyCode == KeyCode.Space)
        // {
        //     this.rigid.isKinematic = false;
        //     isFalled = true;
        // }

       if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ApplyForceToHingeJoints(-transform.right); // 왼쪽 방향으로 힘을 가하도록 변경
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ApplyForceToHingeJoints(transform.right); // 오른쪽 방향으로 힘을 가하도록 변경
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            PushHingeJoint(); // HingeJoint 이름을 사용하여 잡아당기기
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            PushHingeJoint(); // HingeJoint 이름을 사용하여 밀어당기기
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            this.rigid.isKinematic = false;
            isFalled = true;
        }
    }

    void PushHingeJoint()
    {
        foreach (HingeJoint hingeJoint in hingeJoints) 
        {
            if (hingeJoint.name == "fur")
            {
                hingeJoint.connectedBody.AddForce(transform.up * pullMagnitude); // 위 방향으로 힘을 가하여 밀어내기
            }
        }

        Debug.LogWarning("No HingeJoint found with the name: fur");
}




     void ApplyForceToHingeJoints(Vector3 forceDirection)
    {
        foreach (HingeJoint hingeJoint in hingeJoints)
        {
            // Calculate the force vector
            Vector3 force = forceDirection.normalized * forceMagnitude;

            // Apply force to the connected body of each HingeJoint
            hingeJoint.connectedBody.AddForce(force);
        }
    }

// 밑에는 velocity를 이용한 방법  // 아니 위에가 ㅁ에드포스코드고 밑에가 벨로쉬튀인데 아까 돼서 주석처리 해노사는데;;엇 아니 계속 지금 벨로시티야 밑에 코드
//     void ApplyForceToHingeJoints(Vector3 forceDirection)
// {
//     foreach (HingeJoint hingeJoint in hingeJoints)
//     {
//         // Calculate the force vector
//         Vector3 force = forceDirection.normalized * forceMagnitude;

//         // Apply force to the connected body of each HingeJoint using Rigidbody's velocity
//         hingeJoint.connectedBody.velocity = force;
//     }
// }

 

    
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
                downKeyCode = KeyCode.Space;
                //RemovePlayer();
                playerSelector.RemoveUser(playerID);
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isFalled = true;
            //this.rigid.isKinematic = false;
            Debug.Log("바닥과 충~돌");
            //rigid.isKinematic = true;

            foreach (var hinge in GetComponentsInChildren<HingeJoint>())
            {
                Destroy(hinge);
            }
            playerSelector.RemoveUser(playerID);
            //Invoke("playerSelector.RemoveUser(playerID)",6f);
        }
    }

    

    public void RemovePlayer()
    {
        if (!isFalled)
            return;
        else
        {
            Debug.Log("삭제!");
            DOVirtual.DelayedCall(3, PlayerEnd).SetId(playerID);
        }
    }


    private void PlayerEnd()
    {
        Debug.Log("진짜삭제");
        onPlayerEnd?.Invoke(this);
        //Destroy(gameObject);
    }

    private void PlayerStart()
    {
        isActive = true;
    }


    public void SetPlayerColor(string color)
    {
        playerColor = color;
    }

    public void SetUserIndex(int index)
    {
        userIndex = index;
    }

    public void Test()
    {
        Debug.Log(playerID + "!!!!!!!!!!!!!!!!!!!!!!!!" + GetInstanceID());
    }
}