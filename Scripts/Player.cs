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
                PushHingeJoint("hingeJointFur", "pull", 10f);
            }
            else if (furScene)
            {
                PushHingeJoint("hingeJointFur", "pull", 10f);
            }
        }

        else if (downKeyCode == KeyCode.DownArrow & isMove)
        {
            if (lightScene)
            {
                PushHingeJoint("hingeJointFur", "push", 10f);
            }
            else if (furScene)
            {
                PushHingeJoint("hingeJointFur", "push", 10f);
            }
        }
        else if (downKeyCode == KeyCode.LeftArrow && isMove)
        {
            if (lightScene)
            {
                ApplyForceToHingeJoints(-transform.right, 1.5f);
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
                ApplyForceToHingeJoints(transform.right, 1.5f);
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
            Light childLight = gameObject.GetComponentInChildren<Light>();
             Renderer renderer = this.GetComponent<Renderer>();
            Material material = renderer.material;
            Color initialColor = material.color;
            childLight.color = new Color(1,1,1,1);
            StartCoroutine(RevertPushColorDelay(playerLight, initialColor, 1f, 1f));
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
            if(this.CompareTag("Fur6"))
            {
                Invoke("PushIdleMotion",6f);
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
        List<Color> redColors = new List<Color>
        { new Color(0.2588235f, 0.01960784f, 0.08627448f),
         new Color(0.4901961f, 0.09803919f, 0.2078431f),
         new Color(0.7058824f, 0.1686274f, 0.317647f),
         new Color(0.859f, 0.2431372f, 0.418f) };

            List<Color> blueColors = new List<Color>
        { new Color(0.03921569f, 0.1490196f, 0.2784314f),
        new Color(0.07843138f, 0.2588235f, 0.4470588f),
        new Color(0.1254902f, 0.3215686f, 0.5843138f),
        new Color(0.172549f, 0.4549019f, 0.7019608f) };

            List<Color> greenColors = new List<Color>
        { new Color(0.02352941f, 0.1607843f, 0.145098f),
        new Color(0.01568628f, 0.2901961f, 0.2588235f),
        new Color(0.227451f, 0.5686274f, 0.5333333f) };

            List<Color> yellowColors = new List<Color>
        { new Color(1f, 0.7333333f, 0.3607843f),
        new Color(1f, 0.6078432f, 0.3137255f),
        new Color(0.8862745f, 0.3686274f, 0.2431372f),
        new Color(0.7764706f, 0.2392156f, 0.1843137f)};

            List<Color> purpleColors = new List<Color>
        { new Color(0.2156862f, 0.1058823f, 0.345098f),
        new Color(0.2980392f, 0.2078431f, 0.4588234f),
        new Color(0.3568628f, 0.2941176f, 0.5411765f),
        new Color(0.4705882f, 0.345098f, 0.6509804f)};

        int randomIndex = UnityEngine.Random.Range(0, 5);
            List<Color> selectedColors;

            switch (randomIndex)
            {
                case 0:
                    selectedColors = blueColors;
                    break;
                case 1:
                    selectedColors = greenColors;
                    break;
                case 2:
                    selectedColors = redColors;
                    break;
                case 3:
                    selectedColors = yellowColors;
                    break;
                case 4:
                    selectedColors = purpleColors;
                    break;
                default:
                    selectedColors = blueColors; // 기본값
                    break;
            }

        Color targetColor = selectedColors[UnityEngine.Random.Range(0, selectedColors.Count)];
        DOVirtual.Color(playerLight.color, targetColor, 1f, value =>
                   {
                       playerLight.color = value;
                   });

        Transform transform = this.transform;
        foreach (SpringJoint springJoint in springJoints)
        {
            PushHingeJoint("hingeJointFur", "push", 40f);
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
                    Transform furTransform = springJoint.transform.Find("hingeJointFur");
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
                    Transform furTransform = springJoint.transform.Find("hingeJointFur");
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