using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using IMFINE.Utils.JoyStream.Communicator;
using System;
using Unity.VisualScripting;


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
    public bool isFalling = false;
    public bool isActive = false;

    [Header("String & Int")]
    public string playerColor;
    public string playerID;
    public int userIndex;

    [Header("PlayerMovement")]
    public float forceMagnitude = 10f;
    //public float pushMagnitude;
    private HingeJoint[] hingeJoints;

    private SpringJoint[] springJoints;
    CameraShake Camera;
    float time;
    public float idleSpeed;

    [Header("Sound")]
    public AudioSource splatSound;
    public AudioSource colSound;

    [Header("DOTween")]
    public Ease ease;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        hingeJoints = GetComponentsInChildren<HingeJoint>();
        springJoints = GetComponentsInChildren<SpringJoint>();

        playerSelector = FindObjectOfType<PlayerSelector>();
    }

    void Start()
    {
        PlayerStart();
        //GetComponent<Player>().enabled = false; 

        childRigidbodies = new List<Rigidbody>(GetComponentsInChildren<Rigidbody>());
        childRigidbodies.Remove(rigid);
        Camera = GameObject.FindWithTag("MainCamera").GetComponent<CameraShake>();
    }

    private void Update()
    {
        if (downKeyCode == KeyCode.UpArrow)
        {
            PushHingeJoint("fur", "pull", 10f);
            colSound.Play();
        }

        else if (downKeyCode == KeyCode.DownArrow)
        {
            PushHingeJoint("fur", "push", 10f);
            colSound.Play();
        }
        else if (downKeyCode == KeyCode.LeftArrow)
        {
            ApplyForceToHingeJoints(-transform.right);
            colSound.Play();

        }
        else if (downKeyCode == KeyCode.RightArrow)
        {
            ApplyForceToHingeJoints(transform.right);
            colSound.Play();
        }
        else if (downKeyCode == KeyCode.Space)
        {
            //rigid.isKinematic = false;
            isFalling = true;
        }


        if (Input.GetKeyDown(KeyCode.X))
        {
            if(gameObject.CompareTag("Fur1"))
            {
                PushHingeJoint("fur", "push", 30f);
                
            }
            Invoke("idleMotion1",1.0f);
            Invoke("idleMotion2",2f);
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            //MoveChildWithSpringEffect();
            PushHingeJoint("fur", "push", 50f);
        }

        if(Input.GetKeyDown(KeyCode.L))
        {
            if(gameObject.CompareTag("Fur1"))
            {
                ApplyForceToHingeJoints(transform.right*idleSpeed);
            }
            Invoke("idleMotion3",1.0f);
            Invoke("idleMotion4",2f);
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            if (gameObject.CompareTag("Fur1"))
            {
                ApplyForceToHingeJoints(-transform.right*idleSpeed);
            }
            Invoke("idleMotion4",1f);
            Invoke("idleMotion3",2.0f);
            
        }
    }

    public void idleMotion3()
    {
        if(gameObject.CompareTag("Fur2"))
        {
            ApplyForceToHingeJoints(-transform.right*idleSpeed);
        }
    }

    public void idleMotion4()
    {
        if(gameObject.CompareTag("Fur3"))
        {
            ApplyForceToHingeJoints(transform.right*idleSpeed);
        }
    }

    void MoveChildWithSpringEffect()
    {
        foreach (SpringJoint springJoint in springJoints)
        {
            // 원하는 SpringJoint를 찾기 위해 이름이나 다른 기준을 사용하세요
            if (springJoint.CompareTag("springFur"))
            {
                
                StartCoroutine(SpringEffectCoroutine(springJoint));
                break;
            }
        }
    }

    IEnumerator SpringEffectCoroutine(SpringJoint springJoint)
    {
        Rigidbody childRigidbody = springJoint.GetComponent<Rigidbody>();
        Vector3 originalPosition = childRigidbody.transform.localPosition;

        // 아래로 이동
        Vector3 newPosition = originalPosition + Vector3.up * 1.0f; // 원하는 거리만큼 이동
        float duration = 0.2f; // 이동 시간

        childRigidbody.DOMove(newPosition, duration).SetEase(Ease.InBounce);

        yield return new WaitForSeconds(duration);

        // 스프링 효과로 원래 위치로 돌아옴
        springJoint.connectedAnchor = originalPosition;
        springJoint.spring = 5.0f; // 스프링 강도
        springJoint.damper = 5.0f; // 감쇠
    }

    public void idleMotion1()
    {
        if(gameObject.CompareTag("Fur2"))
        {
            PushHingeJoint("fur", "push", 40f);
        }
    }

    public void idleMotion2()
    {
        if(gameObject.CompareTag("Fur3"))
        {
            PushHingeJoint("fur", "push", 50f);
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
                else if (action == "pull")
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
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            splatSound.Play();  // 5.17 수정 필
            isFalled = true;
            //rigid.isKinematic = true;

            foreach (var childRigidbody in childRigidbodies)
            {
                childRigidbody.isKinematic = false;
                childRigidbody.AddForce(Vector3.down * 10f, ForceMode.Impulse);
            }
            //StartCoroutine(DestroyChildrenAfterDelay(3f));
        }
    }

    // private IEnumerator DestroyChildrenAfterDelay(float delay)
    // {
    //     yield return new WaitForSeconds(5f);

    //     foreach (var childRigidbody in childRigidbodies)
    //     {
    //         if (childRigidbody != null && childRigidbody.gameObject != null)
    //         {
    //             Destroy(childRigidbody.gameObject);
    //         }
    //     }

    //     // 플레이어 객체 파괴
    //    Destroy(gameObject);
    // }


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