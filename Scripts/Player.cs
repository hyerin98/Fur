using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using IMFINE.Utils.JoyStream.Communicator;
using System;
using Unity.VisualScripting;
using Unity.Mathematics;


public class Player : MonoBehaviour
{
    [Header("Settings")]
    private KeyCode downKeyCode = 0;
    public Rigidbody rigid;
    public delegate void OnPlayerEnd(Player target);
    //public event OnPlayerEnd onPlayerEnd;
    public List<Rigidbody> childRigidbodies;
    [Header("Bool")]
    public bool isFalled = false;
    public bool isFalling = false;
    public bool isActive = false;
    public bool isSmall = false;
    public bool lightScene;
    public bool furScene;


    [Header("String & Int")]
    public string playerColor;
    public string playerID;
    public int userIndex;

    [Header("PlayerMovement")]
    public float forceMagnitude;
    //public float pushMagnitude;
    public HingeJoint[] hingeJoints;

    public SpringJoint[] springJoints;
    CameraShake Camera;

    [Header("Sound")]
    public AudioSource[] sounds;

    [Header("DOTween")]
    public Ease ease;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        hingeJoints = GetComponentsInChildren<HingeJoint>();
        springJoints = GetComponentsInChildren<SpringJoint>();        
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
            if(lightScene)
            {
                PushHingeJoint("fur", "pull", 10f);
            }
            else if(furScene)
            {
                PushHingeJoint("fur", "pull", 10f);
            }
        }

        else if (downKeyCode == KeyCode.DownArrow)
        {
            if(lightScene)
            {
                PushHingeJoint("fur", "push", 10f);
            }
            else if(furScene)
            {
                PushHingeJoint("fur", "push", 10f);
            }
            
        }
        else if (downKeyCode == KeyCode.LeftArrow)
        {
            ApplyForceToHingeJoints(-transform.right, 1.0f);
        }
        else if (downKeyCode == KeyCode.RightArrow)
        {
            ApplyForceToHingeJoints(transform.right, 1.0f);
            
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
                ApplyForceToHingeJoints(transform.right, 1.0f);
            }
            Invoke("idleMotion3",1.0f);
            Invoke("idleMotion4",2f);
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            if (gameObject.CompareTag("Fur1"))
            {
                ApplyForceToHingeJoints(-transform.right, 1.0f);
            }
            Invoke("idleMotion4",1f);
            Invoke("idleMotion3",2.0f);
        }
        
        if(Input.GetKeyDown(KeyCode.C) && !isSmall)
        {
            isSmall= true;
            Vector3 smallFur = new Vector3(0.2f,0.25f,0.2f);
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
            ApplyForceToHingeJoints(-transform.right, 1.0f);
        }
    }

    public void idleMotion4()
    {
        if(gameObject.CompareTag("Fur3"))
        {
            ApplyForceToHingeJoints(transform.right , 1.0f);
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


    public void ApplyForceToHingeJoints(Vector3 forceDirection, float forceMultiplier)
    {
        foreach (HingeJoint hingeJoint in hingeJoints)
        {
            if (!isSmall)
            {
                if (lightScene)
                {
                    Vector3 force = forceDirection.normalized * forceMagnitude * forceMultiplier;
                    hingeJoint.connectedBody.AddForce(force);
                }
                else if (furScene)
                {
                    Vector3 force = forceDirection.normalized * forceMagnitude * 2f;
                    hingeJoint.connectedBody.AddForce(force);
                }

            }
            else if (isSmall)
            {
                if(lightScene)
                {
                    Vector3 force = forceDirection.normalized * forceMagnitude / 3f;
                    hingeJoint.connectedBody.AddForce(force);
                }
                else if(furScene)
                {
                    Vector3 force = forceDirection.normalized * forceMagnitude / 3f;
                    hingeJoint.connectedBody.AddForce(force);
                }
                
            }
        }
    }

    public void OnPlayerMoveProtocol(ProtocolType protocolType)
    {
        switch (protocolType)
        {
            case ProtocolType.CONTROLLER_UP_PRESS:
                downKeyCode = KeyCode.UpArrow;
                PlayRandomMoveSound();
                break;
            case ProtocolType.CONTROLLER_UP_RELEASE:
                downKeyCode = KeyCode.None;
                break;
            case ProtocolType.CONTROLLER_DOWN_PRESS:
                downKeyCode = KeyCode.DownArrow;
                PlayRandomMoveSound();
                break;
            case ProtocolType.CONTROLLER_DOWN_RELEASE:
                downKeyCode = KeyCode.None;
                break;
            case ProtocolType.CONTROLLER_LEFT_PRESS:
                downKeyCode = KeyCode.LeftArrow;
                PlayRandomMoveSound();
                break;
            case ProtocolType.CONTROLLER_LEFT_RELEASE:
                downKeyCode = KeyCode.None;
                break;
            case ProtocolType.CONTROLLER_RIGHT_PRESS:
                downKeyCode = KeyCode.RightArrow;
                PlayRandomMoveSound();
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
            PlayRandomFallingSound();
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

    private void PlayRandomFallingSound()
    {
        int randomIndex = UnityEngine.Random.Range(0, 3); // 0, 1, 2 중 랜덤 선택
        switch (randomIndex)
        {
            case 0:
                sounds[0].Play();
                break;
            case 1:
                sounds[1].Play();
                break;
            case 2:
                sounds[2].Play();
                break;
            case 3:
                sounds[3].Play();
                break;
        }
    }

    private void PlayRandomMoveSound()
    {
        int randomIndex =  UnityEngine.Random.Range(0,3);
        switch (randomIndex)
        {
            case 0:
            sounds[4].Play();
            break;
            case 1:
            sounds[5].Play();
            break;
            case 2:
            sounds[6].Play();
            break;
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