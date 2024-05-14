using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using IMFINE.Utils.JoyStream.Communicator;
using System;

public class Player : MonoBehaviour
{
    [Header("Settings")]
    private KeyCode downKeyCode = 0;
    public Rigidbody rigid;
    public delegate void OnPlayerEnd(Player target);
    public event OnPlayerEnd onPlayerEnd;
    public List<Rigidbody> childRigidbodies;
    public PlayerSelector playerSelector;

    [Header("Bool")]
    public bool isFalled = false;
    public bool isActive = false;

    [Header("String & Int")]
    public string playerColor;
    public string playerID;
    public int userIndex;

    [Header("PlayerMovement")]
    public float forceMagnitude = 10f;
    public float pushForce = 3f;
    private HingeJoint[] hingeJoints;


    [Header("DOTween")]
    public Ease ease;

    private void Awake()
    {
        //playerID = System.Guid.NewGuid().ToString();
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
        GetComponent<Player>().enabled = false; // 4.30 테스트 -> 생성된 털들만 움직이도록

        childRigidbodies = new List<Rigidbody>(GetComponentsInChildren<Rigidbody>());
        childRigidbodies.Remove(rigid);
    }

    private void Update()
    {
        if (downKeyCode == KeyCode.UpArrow)
        {
            PushHingeJoint("fur", "pull", 1000f);
        }

        else if (downKeyCode == KeyCode.DownArrow)
        {
            PushHingeJoint("fur", "push", 1000f);
        }
        else if (downKeyCode == KeyCode.LeftArrow)
        {
            ApplyForceToHingeJoints(-transform.right);

        }
        else if (downKeyCode == KeyCode.RightArrow)
        {
            ApplyForceToHingeJoints(transform.right);
        }
        else if (downKeyCode == KeyCode.Space)
        {
            this.rigid.isKinematic = false;
            isFalled = true;
        }

        else if (Input.GetKeyDown(KeyCode.Space))
        {
            rigid.isKinematic = false;
            isFalled = true;
        }
    }

    void PushHingeJoint(string jointName, string action, float pushForce)
    {
        foreach (HingeJoint hingeJoint in hingeJoints)
        {
            if (hingeJoint.name == jointName)
            {
                if (action == "pull")
                {
                   JointSpring spring = hingeJoint.spring;
                    spring.targetPosition = -2f;
                    spring.spring = pushForce; 
                    hingeJoint.spring = spring;

                    hingeJoint.connectedBody.MovePosition(hingeJoint.transform.position + hingeJoint.transform.up * 0.3f); 

                }
                else if (action == "push")
                {
                    JointSpring spring = hingeJoint.spring;
                    spring.targetPosition = 2f;
                    spring.spring = pushForce; 
                    hingeJoint.spring = spring;

                    hingeJoint.connectedBody.MovePosition(hingeJoint.transform.position + hingeJoint.transform.up * 0.3f); 
                }
                return;
            }
        }
    }


    void ApplyForceToHingeJoints(Vector3 forceDirection)
    {
        foreach (HingeJoint hingeJoint in hingeJoints)
        {
            Vector3 force = forceDirection.normalized * forceMagnitude;
            hingeJoint.connectedBody.AddForce(force);
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
                TraceBox.Log("누름");
                downKeyCode = KeyCode.Space;
                //RemovePlayer();
                //  PlayerData playerData = new PlayerData();
                //  playerSelector.RemoveUser(playerData);
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isFalled = true;
            // 부모 객체의 물리 시뮬레이션 비활성화
            rigid.isKinematic = true;

            // 자식 객체들의 물리 시뮬레이션 비활성화 및 떨어뜨리기
            foreach (var childRigidbody in childRigidbodies)
            {
                childRigidbody.isKinematic = false;
                childRigidbody.AddForce(Vector3.down * 5f, ForceMode.Impulse); // 특정 힘을 가해 아래로 떨어지게 함
            }

            if (playerSelector != null)
            {
                PlayerData playerData = new PlayerData();
                playerSelector.RemoveUser(playerData);
            }

            // 자식 객체들을 제거하기 위한 코루틴 시작
            StartCoroutine(DestroyChildrenAfterDelay(3f));
        }
    }

    private IEnumerator DestroyChildrenAfterDelay(float delay)
    {
        yield return new WaitForSeconds(5f);

        foreach (var childRigidbody in childRigidbodies)
        {
            if (childRigidbody != null && childRigidbody.gameObject != null)
            {
                Destroy(childRigidbody.gameObject);
            }
        }

        // 플레이어 객체 파괴
        Destroy(gameObject);
    }


    // public void RemovePlayer()
    // {
    //     if (!isFalled)
    //         return;
    //     else
    //     {
    //         Debug.Log("삭제!");
    //         DOVirtual.DelayedCall(3, PlayerEnd).SetId(playerID);
    //     }
    // }


    // private void PlayerEnd()
    // {
    //     Debug.Log("진짜삭제");
    //     onPlayerEnd?.Invoke(this);
    //     //Destroy(gameObject);
    // }

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

}