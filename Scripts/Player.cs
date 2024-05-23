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
    public bool isSmall = false;

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
            

        }
        else if (downKeyCode == KeyCode.RightArrow)
        {
            ApplyForceToHingeJoints(transform.right);
            
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
                ApplyForceToHingeJoints(transform.right);
            }
            Invoke("idleMotion3",1.0f);
            Invoke("idleMotion4",2f);
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            if (gameObject.CompareTag("Fur1"))
            {
                ApplyForceToHingeJoints(-transform.right);
            }
            Invoke("idleMotion4",1f);
            Invoke("idleMotion3",2.0f);
        }
        
        if(Input.GetKeyDown(KeyCode.C) && !isSmall)
        {
            isSmall= true;
            Vector3 smallFur = new Vector3(0.2f,0.2f,0.2f);
            this.transform.DOScale(smallFur, 1f).SetEase(ease);
        }
        if(Input.GetKeyDown(KeyCode.O) && isSmall)
        {
            isSmall = false;
            Vector3 oriFur = new Vector3(0.3f,0.35f,0.3f);
            this.transform.DOScale(oriFur, 1f).SetEase(ease);
        }
    }

    public void idleMotion3()
    {
        if(gameObject.CompareTag("Fur2"))
        {
            ApplyForceToHingeJoints(-transform.right);
        }
    }

    public void idleMotion4()
    {
        if(gameObject.CompareTag("Fur3"))
        {
            ApplyForceToHingeJoints(transform.right);
        }
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

    public void PushHingeJoint(string jointName, string action, float pushForce)
    {
        colSound.Play();
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


    public void ApplyForceToHingeJoints(Vector3 forceDirection)
    {
        colSound.Play();
        foreach (HingeJoint hingeJoint in hingeJoints)
        {
            if(!isSmall)
            {
                Vector3 force = forceDirection.normalized * forceMagnitude;
                hingeJoint.connectedBody.AddForce(force);
            }
            else if(isSmall)
            {
                Vector3 force = forceDirection.normalized * forceMagnitude / 3f;
                hingeJoint.connectedBody.AddForce(force);
            }
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
                childRigidbody.AddForce(Vector3.down * 2f, ForceMode.Impulse);
            }
            this.transform.DOPunchPosition(Vector3.down, 0.02f, 10, 0.05f, false); // 5.22 추가
            
        }
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

}