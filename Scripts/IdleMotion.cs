using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using IMFINE.Utils.JoyStream.Communicator;
using UnityEngine;
using System.Linq;

public class IdleMotion : MonoBehaviour
{

    [Header("Idle")]
    public bool idle = true;
    public Coroutine idleMotionCoroutine; // 5.24 - idle진행되다가 한명이라도 접속 시 idleMotion 중지시키는 코루틴 (테스트 많이 해봐야함)
    public Coroutine fallingMotionCoroutine;
    public bool isIdleMotionRunning = false;
    public List<GameObject> idleFurs = new List<GameObject>(); // idle일때 움직이는 털 리스트 
    public Dictionary<GameObject, bool> furColorAssigned = new Dictionary<GameObject, bool>(); // fur의 색상 할당 상태를 추적
    public HashSet<string> removedFurNames = new HashSet<string>(); // 삭제된 fur 이름을 저장할 HashSet
    public GameObject fake_furPrefab;
    public Ease ease;
    
    public void Idle_Motion()
    {
        if (idleMotionCoroutine != null) // 만약 idlemotion코루틴이 돌고있다면
        {
            StopCoroutine(idleMotionCoroutine); // idlemotion코루틴 정지
            idleMotionCoroutine = null;
        }

        if (idle && !isIdleMotionRunning)
        {
            isIdleMotionRunning = true;
            idleMotionCoroutine = StartCoroutine(AssignColorsWithDelay());
            isIdleMotionRunning = false;
        }
        // if(idle)
        // {
        //     StartCoroutine(AssignColorsWithDelay());
        // }
    }

    public IEnumerator AssignColorsWithDelay() // 파도타기 코루틴
    {
        // 삭제된 fur를 제외하고 남은 fur들만 추출
        if (idle)
        {
            List<GameObject> remainingFurs = idleFurs.Where(fur => !removedFurNames.Contains(fur.name)).ToList();

            // fur 이름 숫자 순으로 정렬
            remainingFurs.Sort((fur1, fur2) =>
            {
                int num1 = int.Parse(fur1.name.Replace("fur", ""));
                int num2 = int.Parse(fur2.name.Replace("fur", ""));
                return num1.CompareTo(num2);
            });

            // 피도타기할 때 조화로운 같은 색 계열
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

            // 색상 계열을 무작위로 선택
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

                    DOVirtual.Color(material.color, targetColor, 0.5f, value =>
                    {

                        material.color = value;
                    });
                    DOVirtual.Color(childLight.color, targetColor, 0.5f, value =>
                    {
                        childLight.color = value;
                        //childLight.transform.position = new Vector3(childLight.transform.position.x, 4f, childLight.transform.position.z);
                    });

                    StartCoroutine(RevertColorAfterDelay(material, childLight, initialColor, 1f, 1f));
                }
                else
                {
                    Debug.LogWarning("Renderer가 존재하지 않습니다: " + idleFur.name);
                }

                if (player != null)
                {
                    if (player.CompareTag("Fur1"))
                    {
                        player.ApplyForceToHingeJoints(transform.right, 1.5f);
                    }
                    else if (player.CompareTag("Fur2"))
                    {
                        player.ApplyForceToHingeJoints(-transform.right, 1.5f);
                    }
                    else if (player.CompareTag("Fur3"))
                    {
                        player.ApplyForceToHingeJoints(transform.right, 1.5f);
                    }
                    else if (player.CompareTag("Fur4"))
                    {
                        player.ApplyForceToHingeJoints(-transform.right, 1.5f);
                    }
                    else if (player.CompareTag("Fur5"))
                    {
                        player.ApplyForceToHingeJoints(transform.right, 1.5f);
                    }
                    else if (player.CompareTag("Fur6"))
                    {
                        player.ApplyForceToHingeJoints(-transform.right, 1.5f);
                    }
                }
                else
                {
                    Debug.LogWarning("Player 컴포넌트가 존재하지 않습니다: " + idleFur.name);
                }

                yield return new WaitForSeconds(0.05f); // 다음 fur로 넘어가기 전에 약간의 대기
            }
        }
    }


    public IEnumerator RevertColorAfterDelay(Material material, Light childLight, Color initialColor, float delay, float duration) // 파도타기를 원래의 색으로 돌아오게하는 코루틴
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

    public void FallingMotion()
    {
        if (fallingMotionCoroutine != null)
        {
            StopCoroutine(fallingMotionCoroutine);
            fallingMotionCoroutine = null;
        }

        if (idle && !isIdleMotionRunning)
        {
            isIdleMotionRunning = true;
            fallingMotionCoroutine = StartCoroutine(fallingFur());
            isIdleMotionRunning = false;
        }
    }


    public IEnumerator fallingFur()
    {
        if (idle)
        {
            //yield return new WaitForSeconds(1f);
            List<GameObject> selectedFurs = new List<GameObject>();
            int furCount = Mathf.Min(5, idleFurs.Count);

            while (selectedFurs.Count < furCount)
            {
                GameObject randomFur = idleFurs[Random.Range(0, idleFurs.Count)];
                if (!selectedFurs.Contains(randomFur))
                {
                    selectedFurs.Add(randomFur);
                }
            }
            foreach (GameObject fur in selectedFurs)
            {
                if (fur != null)
                {
                    fur.SetActive(false);

                    GameObject fakeFur = Instantiate(fake_furPrefab, fur.transform.position, fur.transform.rotation);
                    Rigidbody furRigidbody = fakeFur.GetComponent<Rigidbody>();
                    Renderer renderer = fakeFur.GetComponent<Renderer>();
                    Light childLight = fakeFur.GetComponentInChildren<Light>();
                    Material material = renderer.material;

                    List<Color> randomFallingColor = new List<Color> { new Color(0.2156862f, 0.1058823f, 0.345098f),
                    new Color(0.2980392f, 0.2078431f, 0.4588234f),
                    new Color(0.3568628f, 0.2941176f, 0.5411765f),
                    new Color(0.4705882f, 0.345098f, 0.6509804f),new Color(1f, 0.7333333f, 0.3607843f),
                    new Color(1f, 0.6078432f, 0.3137255f),
                    new Color(0.8862745f, 0.3686274f, 0.2431372f),
                    new Color(0.7764706f, 0.2392156f, 0.1843137f),new Color(0.02352941f, 0.1607843f, 0.145098f),
                    new Color(0.01568628f, 0.2901961f, 0.2588235f),
                    new Color(0.227451f, 0.5686274f, 0.5333333f),new Color(0.03921569f, 0.1490196f, 0.2784314f),
                    new Color(0.07843138f, 0.2588235f, 0.4470588f),
                    new Color(0.1254902f, 0.3215686f, 0.5843138f),
                    new Color(0.172549f, 0.4549019f, 0.7019608f),new Color(0.2588235f, 0.01960784f, 0.08627448f),
                    new Color(0.4901961f, 0.09803919f, 0.2078431f),
                    new Color(0.7058824f, 0.1686274f, 0.317647f),
                    new Color(0.859f, 0.2431372f, 0.418f)
                    };
                    Color initialColor = material.color;

                    childLight.color = randomFallingColor[Random.Range(0, randomFallingColor.Count)];

                    if (furRigidbody != null && renderer != null && childLight != null)
                    {
                        furRigidbody.isKinematic = false;
                        childLight.intensity = 80f;
                        yield return new WaitForSeconds(0.4f);
                        renderer.material.DOFade(0f, 3f).SetEase(Ease.Linear);
                        DOVirtual.Color(childLight.color, initialColor, 3f, value =>
                {
                    childLight.color = value;
                });
                        StartCoroutine(DestroyFakeFur(fakeFur, fur, 3f));
                        yield return new WaitForSeconds(0.5f);
                    }
                }
            }
        }
    }

    public IEnumerator DestroyFakeFur(GameObject fakeFur, GameObject originalFur, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(fakeFur);

        originalFur.SetActive(true);

        originalFur.transform.localScale = new Vector3(0f, originalFur.transform.localScale.y, originalFur.transform.localScale.z);
        originalFur.transform.DOScaleX(0.3f, 1f).SetEase(ease);
    }
}
