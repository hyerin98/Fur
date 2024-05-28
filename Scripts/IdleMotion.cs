using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using IMFINE.Utils.JoyStream.Communicator;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting.Dependencies.Sqlite;
using System.Xml.Serialization;
//using IMFINE.Utils.JoyStream.Communicator.ext;

public class IdleMotion : MonoBehaviour
{
    private PlayerSelector playerSelector;

    public Coroutine idleMotionCoroutine; // 5.24 - idle진행되다가 한명이라도 접속 시 idleMotion 중지시키는 코루틴 (테스트 많이 해봐야함)
    public bool isIdleMotionRunning = false;
    public List<GameObject> idleFurs = new List<GameObject>(); // idle일때 움직이는 털 리스트 
    public Dictionary<GameObject, bool> furColorAssigned = new Dictionary<GameObject, bool>(); // fur의 색상 할당 상태를 추적
    public HashSet<string> removedFurNames = new HashSet<string>(); // 삭제된 fur 이름을 저장할 HashSet
    public int furCounter = 0; // fur 이름에 사용할 인덱스
    public bool idle = true;
    private List<Vector3> idleFurPositions = new List<Vector3>();


    void Awake()
    {
        InitializeIdleFurPositions();
    }

    void Start()
    {
        playerSelector = GetComponent<PlayerSelector>();
        if (playerSelector != null)
        {
            foreach (var fur in playerSelector.furs)
            {
                furColorAssigned[fur] = false;
            }
            StartCoroutine(CheckPlayerCount(15f));
        }
        else
        {
            Debug.LogError("PlayerSelector component not found on this GameObject or any of its parents.");
        }
    }

    private void InitializeIdleFurPositions()
    {
        foreach(var fur in idleFurs)
        {
            idleFurPositions.Add(fur.transform.position);
        }
    }

    private IEnumerator CheckPlayerCount(float interval)
    {
        while (true)
        {
            if (playerSelector.players.Count == 0 && idle)
            {
                yield return new WaitForSeconds(5f);
                Debug.Log("플레이어가 없어서 다시 idle모드 발동");
                StartRandomIdleMotion();

            }
            else if (playerSelector.players.Count > 0)
            {
                idle = false;
                Debug.Log("한명이라도 접속했기 때문에 idle모드 안댐");
            }
            yield return new WaitForSeconds(interval);
        }
    }

    public void StartRandomIdleMotion()
    {
        if (idleMotionCoroutine != null)
        {
            StopCoroutine(idleMotionCoroutine);
            idleMotionCoroutine = null;
        }

        if (idle && !isIdleMotionRunning)
        {
            isIdleMotionRunning = true;
            int randomMotion = Random.Range(0, 1);
            switch (randomMotion)
            {
                case 0:
                    idleMotionCoroutine = StartCoroutine(IdleMotion1());
                    break;
                // case 1:
                //     idleMotionCoroutine = StartCoroutine(IdleMotion2());
                //     break;
            }
        }
    }

    // public void idleMotion1()
    // {
    //     if (idleMotionCoroutine != null)
    //     {
    //         StopCoroutine(idleMotionCoroutine);
    //         idleMotionCoroutine = null;
    //     }

    //     if (idle && !isIdleMotionRunning)
    //     {
    //         isIdleMotionRunning = true;
    //         idleMotionCoroutine = StartCoroutine(AssignColorsWithDelay());
    //     }
    // }
    private IEnumerator IdleMotion1()
    {
        Debug.Log("IdleMotion1 시작");
        if (idleMotionCoroutine != null)
        {
            StopCoroutine(idleMotionCoroutine);
            idleMotionCoroutine = null;
        }
        if (idle && !isIdleMotionRunning)
        {
            isIdleMotionRunning = true;
            idleMotionCoroutine = StartCoroutine(AssignColorsWithDelay());
        }

        yield return StartCoroutine(AssignColorsWithDelay());
    }

    private IEnumerator IdleMotion2()
    {
        Debug.Log("IdleMotion2 시작");
        if (idleMotionCoroutine != null)
        {
            StopCoroutine(idleMotionCoroutine);
            idleMotionCoroutine = null;
        }
        if (idle && !isIdleMotionRunning)
        {
            isIdleMotionRunning = true;
            idleMotionCoroutine = StartCoroutine(fallingFur());
        }
        yield return StartCoroutine(fallingFur());
        isIdleMotionRunning = false;
    }

    private IEnumerator AssignColorsWithDelay()
    {
        // 삭제된 fur를 제외하고 남은 fur들만 추출
        List<GameObject> remainingFurs = idleFurs.Where(fur => !removedFurNames.Contains(fur.name)).ToList();

        // fur 이름 숫자 순으로 정렬
        remainingFurs.Sort((fur1, fur2) =>
        {
            int num1 = int.Parse(fur1.name.Replace("fur", ""));
            int num2 = int.Parse(fur2.name.Replace("fur", ""));
            return num1.CompareTo(num2);
        });

        // 피도타기할 때 조화로운 같은 색 계열 -> 5.27 더 조화롭게 수정필
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


        int randomIndex = Random.Range(0, 5);
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

        for (int i = 0; i < remainingFurs.Count; i++)
        {
            GameObject idleFur = remainingFurs[i];
            Renderer renderer = idleFur.GetComponent<Renderer>();
            Light childLight = idleFur.GetComponentInChildren<Light>();
            Player player = idleFur.GetComponent<Player>(); // idleFur에서 Player 컴포넌트를 가져옴

            if (childLight != null)
            {
                childLight.enabled = true;
            }

            if (renderer != null)
            {
                Material material = renderer.material;
                Color initialColor = material.color; // 초기 색상 저장
                Color targetColor = selectedColors[Random.Range(0, selectedColors.Count)];

                DOVirtual.Color(material.color, targetColor, 1f, value =>
                {
                    material.color = value;
                });
                DOVirtual.Color(childLight.color, targetColor, 1f, value =>
                {
                    childLight.color = value;
                     childLight.intensity = 10f;
                     childLight.range = 3f;
                });

                StartCoroutine(RevertColorAfterDelay(material, childLight, initialColor, 1.2f, 1.2f));
            }
            else
            {
                Debug.LogWarning("Renderer가 존재하지 않습니다: " + idleFur.name);
            }

            if (player != null)
            {
                if (player.CompareTag("Fur1"))
                {
                    player.ApplyForceToHingeJoints(transform.right, 2f);
                }
                else if (player.CompareTag("Fur2"))
                {
                    player.ApplyForceToHingeJoints(-transform.right, 2f);
                }
                else if (player.CompareTag("Fur3"))
                {
                    player.ApplyForceToHingeJoints(transform.right, 2f);
                }

            }
            else
            {
                Debug.LogWarning("Player 컴포넌트가 존재하지 않습니다: " + idleFur.name);
            }

            yield return new WaitForSeconds(0.05f); // 다음 fur로 넘어가기 전에 약간의 대기
        }
    }


    private IEnumerator RevertColorAfterDelay(Material material, Light childLight, Color initialColor, float delay, float duration)
    {
        yield return new WaitForSeconds(delay);

        DOVirtual.Color(material.color, initialColor, duration, value =>
        {
            material.color = value;
        });
        if (childLight != null)
        {
            DOVirtual.Color(childLight.color, initialColor, duration, value =>
            {
                childLight.color = value;
            });
        }
    }

    private IEnumerator fallingFur()
    {
        yield return new WaitForSeconds(1f);

        foreach (GameObject fur in idleFurs)
        {
            if (fur != null)
            {
                Rigidbody furRigidbody = fur.GetComponent<Rigidbody>();
                Renderer renderer = fur.GetComponent<Renderer>();

                if (furRigidbody != null && renderer != null)
                {
                    furRigidbody.isKinematic = false;
                    renderer.material.DOFade(0.1f, 3f).SetEase(Ease.Linear);
                }
            }
        }

        // yield return new WaitForSeconds(1f);

        // for (int i = 0; i < idleFurs.Count; i++)
        // {
        //     GameObject fur = idleFurs[i];
        //     if (fur != null)
        //     {
        //         Rigidbody furRigidbody = fur.GetComponent<Rigidbody>();
        //         Renderer renderer = fur.GetComponent<Renderer>();

        //         if (furRigidbody != null && renderer != null)
        //         {
        //             furRigidbody.isKinematic = true;
        //             renderer.material.DOFade(1f, 0.5f).SetEase(Ease.Linear);
        //             fur.transform.DOMove(idleFurPositions[i], 5f).SetEase(Ease.OutBounce);
        //         }
        //     }
        // }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("실행중");
            StartCoroutine(AssignColorsWithDelay());
        }
    }

}
