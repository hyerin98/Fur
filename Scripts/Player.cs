using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using IMFINE.Utils.JoyStream.Communicator;
using System;
using System.Diagnostics;

public class Player : MonoBehaviour
{
    [Header("Settings")]
    private KeyCode downKeyCode = 0;
    public Rigidbody rigid;
    public delegate void OnPlayerEnd(Player target);
    //public event OnPlayerEnd onPlayerEnd;
    public List<Rigidbody> childRigidbodies;
    private PlayerSelector playerSelector;

    [Header("Bool")]
    public bool isFalled = false;
    public bool isActive = false;

    [Header("String & Int")]
    public string playerColor;
    public string playerID;
    public int userIndex;

    [Header("PlayerMovement")]
    public float forceMagnitude = 10f;
    public float pushMagnitude;
     private HingeJoint[] hingeJoints;

    private SpringJoint[] springJoints;
    CameraShake Camera;

    public float pushForce;

   


    [Header("DOTween")]
    public Ease ease;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        hingeJoints = GetComponentsInChildren<HingeJoint>();
        springJoints = GetComponentsInChildren<SpringJoint>();

        playerSelector = FindObjectOfType<PlayerSelector>();
        if (playerSelector == null)
        {
            //Debug.LogError("PlayerSelector component not found in the scene!");
        }
    }

    void Start()
    {
        PlayerStart();
        GetComponent<Player>().enabled = false; // 4.30 테스트 -> 생성된 털들만 움직이도록

        childRigidbodies = new List<Rigidbody>(GetComponentsInChildren<Rigidbody>());
        childRigidbodies.Remove(rigid);
        Camera = GameObject.FindWithTag("MainCamera").GetComponent<CameraShake>();
    }

    private void Update()
    {
        if (downKeyCode == KeyCode.UpArrow)
        {
            PushHingeJoint("fur","pull", 10f);
        }

        else if (downKeyCode == KeyCode.DownArrow)
        {
            PushHingeJoint("fur","push", 10f);
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
            rigid.isKinematic = false;
            isFalled = true;
        }
    }


    void PushHingeJoint(string jointName, string action, float pushForce)
    {
        foreach (SpringJoint springJoint in springJoints)
        {
            if (springJoint.name == jointName)
            {
                if (action == "push")
                {
                    Transform furTransform = springJoint.transform.Find("fur");
                    if (furTransform != null)
                    {
                        Rigidbody furRigidbody = furTransform.GetComponent<Rigidbody>();
                        if (furRigidbody != null)
                        {
                            furRigidbody.AddForce(furTransform.up * pushForce, ForceMode.Impulse);
                        }
                    }
                }
                else if(action == "pull")
                {
                    Transform furTransform = springJoint.transform.Find("fur");
                    if (furTransform != null)
                    {
                        Rigidbody furRigidbody = furTransform.GetComponent<Rigidbody>();
                        if (furRigidbody != null)
                        {
                            furRigidbody.AddForce(-furTransform.up * pushForce, ForceMode.Impulse);
                        }
                    }
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
            rigid.isKinematic = true;
            

            foreach (var childRigidbody in childRigidbodies)
            {
                childRigidbody.isKinematic = false;
                childRigidbody.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            }

            if (playerSelector != null)
            {
                PlayerData playerData = new PlayerData
                {
                    conn_id = playerID
                };
                playerSelector.RemoveUser(playerData);
            }
            Camera.VibrateForTime(0.05f);
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