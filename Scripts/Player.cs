using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using IMFINE.Utils.JoyStream.Communicator;
using System;
using Unity.VisualScripting;
using Unity.Mathematics;
using System.Net.Security;


public class Player : MonoBehaviour
{
    [Header("Settings")]
    private KeyCode downKeyCode = 0;
    public Rigidbody rigid;
    public delegate void OnPlayerEnd(Player target);
    //public event OnPlayerEnd onPlayerEnd;
    public List<Rigidbody> childRigidbodies;
    Light playerLight;
    [Header("Bool")]
    public bool isFalled = false;
    public bool isFalling = false;
    public bool isActive = false;
    public bool isSmall = false;
    public bool lightScene;
    public bool furScene;
    public bool isMove = true;


    [Header("String & Int")]
    public string playerColor;
    public string playerID;
    public int userIndex;

    [Header("PlayerMovement")]
    public float forceMagnitude;
    //public float pushMagnitude;
    public HingeJoint[] hingeJoints;

    public SpringJoint[] springJoints;
    CameraShake cameraShake;

    [Header("Sound")]
    public AudioClip[] lightFallingClips;
    public AudioClip[] lightMoveClips;
    public AudioClip[] furFallingClips;
    public AudioClip[] furMoveClips;

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
        cameraShake = GameObject.FindWithTag("MainCamera").GetComponent<CameraShake>();
        playerLight = GetComponentInChildren<Light>();
    }

    private void Update()
    {
        if (downKeyCode == KeyCode.UpArrow && isMove)
        {
            if (lightScene)
            {
                PushHingeJoint("fur", "pull", 10f);
            }
            else if (furScene)
            {
                PushHingeJoint("fur", "pull", 10f);
            }
        }

        else if (downKeyCode == KeyCode.DownArrow & isMove)
        {
            if (lightScene)
            {
                PushHingeJoint("fur", "push", 10f);
            }
            else if (furScene)
            {
                PushHingeJoint("fur", "push", 10f);
            }
        }
        else if (downKeyCode == KeyCode.LeftArrow && isMove)
        {
            if (lightScene)
            {
                ApplyForceToHingeJoints(-transform.right, 1.0f);
            }
            else if (furScene)
            {
                ApplyForceToHingeJoints(-transform.right, 3.0f);
            }
        }
        else if (downKeyCode == KeyCode.RightArrow && isMove)
        {
            if (lightScene)
            {
                ApplyForceToHingeJoints(transform.right, 1.0f);
            }
            else if (furScene)
            {
                ApplyForceToHingeJoints(transform.right, 3.0f);
            }
        }
        else if (downKeyCode == KeyCode.Space)
        {
            //rigid.isKinematic = false;
            isFalling = true;
        }

        if (Input.GetKeyDown(KeyCode.C) && !isSmall)
        {
            isSmall = true;
            Vector3 smallFur = new Vector3(0.2f, 0.25f, 0.2f);
            this.transform.DOScale(smallFur, 1f).SetEase(ease);
        }
        if (Input.GetKeyDown(KeyCode.O) && isSmall)
        {
            isSmall = false;
            Vector3 oriFur = new Vector3(0.3f, 0.35f, 0.3f);
            this.transform.DOScale(oriFur, 1f).SetEase(ease);
        }

        if(Input.GetKeyDown(KeyCode.X))
        {
            if(this.CompareTag("Fur1"))
            {
                Invoke("PushIdleMotion",1f);
            }
            if(this.CompareTag("Fur2"))
            {
                Invoke("PushIdleMotion",2f);
            }
            if(this.CompareTag("Fur3"))
            {
                Invoke("PushIdleMotion",3f);
            }
            if(this.CompareTag("Fur4"))
            {
                Invoke("PushIdleMotion",4f);
            }
            if(this.CompareTag("Fur5"))
            {
                Invoke("PushIdleMotion",5f);
            }
            // if(gameObject.CompareTag("Fur1") || gameObject.CompareTag("Fur2") || gameObject.CompareTag("Fur3") || gameObject.CompareTag("Fur4") ||  gameObject.CompareTag("Fur5"))
            // {
            //     Invoke("PushIdleMotion",6f);
            // }
        }
    }

    void PushIdleMotion()
    {
        Renderer renderer = this.GetComponent<Renderer>();
        Material material = renderer.material;
        Color initialColor = material.color;
        Color targetColor = new Color(0.1561499f, 0.263519f, 0.2735849f);
        DOVirtual.Color(playerLight.color, targetColor, 1f, value =>
                   {
                       playerLight.color = value;
                   });

        Transform transform = this.transform;
        foreach (SpringJoint springJoint in springJoints)
        {
            PushHingeJoint("fur", "push", 20f);
            StartCoroutine(RevertPushColorDelay(playerLight, initialColor, 1f, 1f));
        }
    }

    private IEnumerator RevertPushColorDelay(Light playerLight, Color initialColor, float delay, float duration)
    {
        yield return new WaitForSeconds(delay);
        if (playerLight != null)
        {
            DOVirtual.Color(playerLight.color, initialColor, duration, value =>
            {
                playerLight.color = value;
            });
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
                if (lightScene)
                {
                    Vector3 force = forceDirection.normalized * forceMagnitude / 3f;
                    hingeJoint.connectedBody.AddForce(force);
                }
                else if (furScene)
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
                if (lightScene)
                    SoundManager.instance.SFXMovePlay("", lightMoveClips);
                else if (furScene)
                    SoundManager.instance.SFXMovePlay("", furMoveClips);
                break;
            case ProtocolType.CONTROLLER_UP_RELEASE:
                downKeyCode = KeyCode.None;
                break;
            case ProtocolType.CONTROLLER_DOWN_PRESS:
                downKeyCode = KeyCode.DownArrow;
                if (lightScene)
                    SoundManager.instance.SFXMovePlay("", lightMoveClips);
                else if (furScene)
                    SoundManager.instance.SFXMovePlay("", furMoveClips);
                break;
            case ProtocolType.CONTROLLER_DOWN_RELEASE:
                downKeyCode = KeyCode.None;
                break;
            case ProtocolType.CONTROLLER_LEFT_PRESS:
                downKeyCode = KeyCode.LeftArrow;
                if (lightScene)
                    SoundManager.instance.SFXMovePlay("", lightMoveClips);
                else if (furScene)
                    SoundManager.instance.SFXMovePlay("", furMoveClips);
                break;
            case ProtocolType.CONTROLLER_LEFT_RELEASE:
                downKeyCode = KeyCode.None;
                break;
            case ProtocolType.CONTROLLER_RIGHT_PRESS:
                downKeyCode = KeyCode.RightArrow;
                if (lightScene)
                    SoundManager.instance.SFXMovePlay("", lightMoveClips);
                else if (furScene)
                    SoundManager.instance.SFXMovePlay("", furMoveClips);
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
        if (other.gameObject.CompareTag("Ground") && !isFalled) // isFalled가 false일 때만 실행되도록 변경
        {
            isFalled = true;
            //cameraShake.VibrateForTime(0.3f);
            //rigid.isKinematic = true;

            foreach (var childRigidbody in childRigidbodies)
            {
                childRigidbody.isKinematic = false;
                childRigidbody.AddForce(Vector3.down * 2f, ForceMode.Impulse);
            }
           // this.transform.DOPunchPosition(Vector3.down, 0.02f, 10, 0.05f, false); // 5.22 추가

            if(lightScene)
            {
                SoundManager.instance.SFXFallingPlay("", lightFallingClips);
            }
            
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