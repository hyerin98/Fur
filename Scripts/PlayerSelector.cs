using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using IMFINE.Utils.JoyStream.Communicator;
using UnityEngine;
using System.Linq;
//using IMFINE.Utils.JoyStream.Communicator.ext;

public class PlayerSelector : MonoBehaviour
{
    [Header("Settings")]
    public Dictionary<string, Player> players = new Dictionary<string, Player>();
    public HashSet<Color> changedColors = new HashSet<Color>(); // 변경된 색상을 추적하기 위한 HashSet
    public List<GameObject> furs = new List<GameObject>(); // 색을 할당받을 털 리스트
    private List<Vector3> furPositions = new List<Vector3>(); // 삭제 후 다시 생기기위한 털위치 리스트
    private HashSet<int> usedFur = new HashSet<int>(); // 사용된 fur 해시셋
    private Dictionary<string, PlayerData> playerDataList = new Dictionary<string, PlayerData>(); // 플레이어데이터 딕셔너
    //private Dictionary<string, string> colorToConnIdMap = new Dictionary<string, string>();
    private Dictionary<string, Color> assignedColors = new Dictionary<string, Color>();

    [Header("DOtween & GameObject & Bool")]
    public Ease ease;
    public GameObject furPrefab;
    public GameObject fake_furPrefab;
    public bool isSpawn;
    public bool idle = true;
    public bool isFurScene;
    public bool isLightScene;
    //public GameObject ground;

    [Header("Idle")]
    public Coroutine idleMotionCoroutine; // 5.24 - idle진행되다가 한명이라도 접속 시 idleMotion 중지시키는 코루틴 (테스트 많이 해봐야함)
    private Coroutine fallingMotionCoroutine;
    public bool isIdleMotionRunning = false;
    public List<GameObject> idleFurs = new List<GameObject>(); // idle일때 움직이는 털 리스트 
    private Dictionary<GameObject, bool> furColorAssigned = new Dictionary<GameObject, bool>(); // fur의 색상 할당 상태를 추적
    private HashSet<string> removedFurNames = new HashSet<string>(); // 삭제된 fur 이름을 저장할 HashSet

    private void Awake()
    {
        ProtocolManager.instance.onWebControllerEvent += OnWebControllerEvent;
        ProtocolManager.instance.onUserConnectEvent += OnUserConnectEvent;
        isSpawn = false;
        InitializeFurPositions();
    }

    private void Start()
    {
        DOTween.Init();

        idle = true;

        // 초기화: 모든 fur의 색상 할당 상태를 false로 설정
        foreach (var fur in furs)
        {
            furColorAssigned[fur] = false;
        }

        // 플레이어 수를 즉시 확인하고, 필요할 경우 idle 모드로 설정
        if (players.Count > 0)
        {
            idle = false;
            Debug.Log("한명이라도 접속해 있기 때문에 idle모드 실행 안함");
        }
        else
        {
            idle = true;
            //StartCoroutine(StartRandomIdleMotion());
        }
    }
    private IEnumerator StartRandomIdleMotion()
    {
        while (idle)
        {
            yield return new WaitForSeconds(5f);

            if (players.Count == 0 && idle)
            {
                int randomMotion = Random.Range(0, 2);
                idle = true;
                isIdleMotionRunning = false;
                switch (randomMotion)
                {
                    case 0:
                        Debug.Log("떨어지는 모션 발동");
                        FallingMotion();
                        break;

                    case 1:
                        Debug.Log("훑는모션 발동");
                        IdleMotion();
                        break;
                }
            }
            yield return new WaitForSeconds(5f);
        }
    }

    private void InitializeFurPositions()
    {
        furPositions.Clear(); // 6.10 주석해제
        foreach (var fur in furs)
        {
            furPositions.Add(fur.transform.position); // 각 fur의 초기 위치를 저장 
        }
    }

    private void OnUserConnectEvent(ProtocolType protocolType, PlayerData playerData)
    {
        if (protocolType == ProtocolType.CONTROLLER_CONNECT)
        {
            playerDataList[playerData.conn_id] = playerData;
            OnAddUser(playerData);
            TraceBox.Log("들어온 유저다: " + playerData.conn_id + " , " + playerData.color_id + " , " + playerData.player_index);
        }
        else if (protocolType == ProtocolType.CONTROLLER_DISCONNECT)
        {
            if (playerDataList.ContainsKey(playerData.conn_id))
            {
                RemoveUser(playerData);
                playerDataList.Remove(playerData.conn_id);
                TraceBox.Log("삭제한 아이디와 컬러값은?: " + playerData.conn_id + " , " + playerData.color_id);
            }
            else
            {
                Debug.LogWarning("플레이어 데이터 연결 X");
            }
        }
    }

    private void OnWebControllerEvent(ProtocolType protocolType, string conID)
    {
        switch (protocolType)
        {
            case ProtocolType.CONTROLLER_UP_PRESS:
                if (players.ContainsKey(conID))
                {
                    players[conID].OnPlayerMoveProtocol(protocolType);
                }
                break;
            case ProtocolType.CONTROLLER_UP_RELEASE:
                if (players.ContainsKey(conID))
                {
                    players[conID].OnPlayerMoveProtocol(protocolType);
                }
                break;
            case ProtocolType.CONTROLLER_DOWN_PRESS:
                if (players.ContainsKey(conID))
                {
                    players[conID].OnPlayerMoveProtocol(protocolType);
                }
                break;
            case ProtocolType.CONTROLLER_DOWN_RELEASE:
                if (players.ContainsKey(conID))
                {
                    players[conID].OnPlayerMoveProtocol(protocolType);
                }
                break;
            case ProtocolType.CONTROLLER_LEFT_PRESS:
                if (players.ContainsKey(conID))
                {
                    players[conID].OnPlayerMoveProtocol(protocolType);
                }
                break;
            case ProtocolType.CONTROLLER_LEFT_RELEASE:
                if (players.ContainsKey(conID))
                {
                    players[conID].OnPlayerMoveProtocol(protocolType);
                }
                break;
            case ProtocolType.CONTROLLER_RIGHT_PRESS:
                if (players.ContainsKey(conID))
                {
                    players[conID].OnPlayerMoveProtocol(protocolType);
                }
                break;
            case ProtocolType.CONTROLLER_RIGHT_RELEASE:
                if (players.ContainsKey(conID))
                {
                    players[conID].OnPlayerMoveProtocol(protocolType);
                }
                break;
            case ProtocolType.CONTROLLER_FALL_PRESS:
                if (players.ContainsKey(conID))
                {
                    players[conID].OnPlayerMoveProtocol(protocolType);
                }
                break;
        }
    }
    public void IdleMotion()
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
        }
        // if(idle)
        // {
        //     StartCoroutine(AssignColorsWithDelay());
        // }
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
        }
    }
    public void OnAddUser(PlayerData playerData)
    {
        if (isIdleMotionRunning) // idle모드가 진행되고 있는 상황이라면
        {
            StopCoroutine("AssignColorsWithDelay"); // idle모드 멈추기
            StopCoroutine("fallingFur");
            isIdleMotionRunning = false; // idle모드 false
        }

        if (idleMotionCoroutine != null)
        {
            StopCoroutine(idleMotionCoroutine);
            idleMotionCoroutine = null;
        }
        if (fallingMotionCoroutine != null)
        {
            StopCoroutine(fallingMotionCoroutine);
            fallingMotionCoroutine = null;
        }

        if (!players.ContainsKey(playerData.conn_id))
        {
            playerData.color_id = ColorManager.instance.AssignUserColor();

            if (playerData.color_id == null || players.Count >= furs.Count)
            {
                Debug.Log("할당가능한 컬러가 없거나 꽉 차서 접속X");
                return;
            }

            List<int> availableFur = Enumerable.Range(0, furs.Count).Where(i => !usedFur.Contains(i)).ToList();
            if (availableFur.Count == 0)
            {
                Debug.LogError("사용가능한 fur이 없다");
                return;
            }

            int randomIndex = Random.Range(0, availableFur.Count);
            int furIndex = availableFur[randomIndex];
            GameObject assignedFur = furs[furIndex];
            playerData.player_index = furIndex;

            // 색상 할당 및 플레이어 설정 로직...
            Player targetPlayer = assignedFur.GetComponent<Player>();

            targetPlayer.SetUserIndex(playerData.player_index);
            if (targetPlayer != null)
            {
                Light childLight = assignedFur.GetComponentInChildren<Light>();
                if (childLight != null)
                {
                    childLight.enabled = true;
                }

                Renderer renderer = assignedFur.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Material material = renderer.material;
                    Color targetColor;

                    if (ColorUtility.TryParseHtmlString("#" + playerData.color_id, out targetColor))
                    {
                        // 색상이 파싱되면 서서히 색상 변경을 시작
                        DOVirtual.Color(material.color, targetColor, 3f, value =>
                        {
                            material.color = value;
                        });

                        if (isLightScene)
                        {
                            DOVirtual.Color(childLight.color, targetColor, 2f, value =>
                        {
                            childLight.color = value;
                        });
                        assignedColors[assignedFur.name] = targetColor; // 할당된 색상을 저장
                        }
                    }

                    targetPlayer.enabled = true;
                    changedColors.Add(targetColor); // 변경된 색상을 추적
                    furColorAssigned[assignedFur] = true; // 색상 할당 상태를 true로 설정
                }

                targetPlayer.playerID = playerData.conn_id;
                targetPlayer.SetPlayerColor(playerData.color_id);
                targetPlayer.SetUserIndex(furIndex);

                usedFur.Add(furIndex);
                players.Add(playerData.conn_id, targetPlayer);
                //colorToConnIdMap.Add(playerData.color_id, playerData.conn_id); // 5.17 수정 -> 최대컬러수 할당받고 나면 컨트롤러 흰색으로 뜨는 이슈 원인 
                playerData.player_index = players.Count;
            }
            else
            {
                TraceBox.Log("할당된 GameObject에 Player 컴포넌트가 없습니다.");
            }

            if (players.Count == 0 || playerData.player_index == 0)
            {
                idle = true;
            }
            else if (playerData.player_index > 0)
            {
                idle = false;
            }
            // if (!isIdleMotionRunning) // 만약 idle모드가 진행중이 아니라면
            // {
            //     IdleMotion(); // idle 모드 실행 
            //     FallingMotion();
            // }
        }
    }

    public void RemoveUser(PlayerData playerData)
    {
        string playerID = playerData.conn_id;
        if (players.ContainsKey(playerID))
        {
            Player player = players[playerID];
            GameObject furObject = player.gameObject;
            player.isMove = false;

            if (furObject != null)
            {
                Vector3 initialPosition = furObject.transform.position; // 현재 위치 저장
                Debug.Log("원래위치: " + initialPosition); 

                removedFurNames.Add(furObject.name);

                // furs 리스트와 furPositions에서 요소 제거
                int furIndex = furs.IndexOf(furObject);
                if (furIndex != -1)
                {
                    furs.RemoveAt(furIndex);
                    furPositions.RemoveAt(furIndex);
                    usedFur.Remove(furIndex); // 사용된 fur 인덱스 제거
                }

                // idleFurs와 furColorAssigned에서 제거
                idleFurs.Remove(furObject);
                furColorAssigned.Remove(furObject);

                // furObject의 컴포넌트 가져오기
                Light childLight = furObject.GetComponentInChildren<Light>();
                Renderer renderer = furObject.GetComponent<Renderer>();
                Rigidbody furRigidbody = furObject.GetComponent<Rigidbody>();

                if (childLight != null && renderer != null && furRigidbody != null)
                {
                    furRigidbody.isKinematic = false;

                    StartCoroutine(DimLightIntensity(childLight, 2f));

                    renderer.material.DOFade(0f, 3f).SetEase(ease);
                    Destroy(furObject, 3.5f);

                    StartCoroutine(RespawnFur(initialPosition, furObject.name)); // 리스폰 코루틴 호출
                }
            }
            // 플레이어 색상 반환
            ColorManager.instance.ReturnColor(player.playerColor);
            players.Remove(playerID);

            TraceBox.Log("삭제된 플레이어의 아이디: " + playerID);

            if (players.Count == 0)
            {
                idle = true;
            }
        }
    }

    private IEnumerator DimLightIntensity(Light light, float duration) // 라이트 서서히 변환시키는 코루틴
    {
        float startIntensity = light.intensity;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {

            float t = timeElapsed / duration;

            light.intensity = Mathf.Lerp(startIntensity, 0f, t);

            timeElapsed += Time.deltaTime;

            yield return null;
        }
        light.intensity = 0f;
    }

    public IEnumerator RespawnFur(Vector3 position, string originalName)
    {
        Debug.Log("리스폰된 위치: " + position);
        yield return new WaitForSeconds(3.0f);

        if (furPrefab != null)
        {
            GameObject newFur = Instantiate(furPrefab, position, Quaternion.identity);
            Debug.Log("새로 생성된 털 위치: " + newFur.transform.position); 
            Player player = newFur.GetComponent<Player>();

            string furName = originalName; // 원래 이름을 사용
            newFur.name = furName;

            furs.Add(newFur);
            furPositions.Add(position);

            Renderer furRenderer = newFur.GetComponent<Renderer>();
            Light childLight = newFur.GetComponentInChildren<Light>();
            if (furRenderer != null && childLight != null)
            {
                Color initialColor = furRenderer.material.color;
                initialColor.a = 1f;
                furRenderer.material.color = initialColor;

                Color startColor = assignedColors.ContainsKey(furName) ? assignedColors[furName] : new Color(0.5283019f, 0.5208259f, 0.5208259f);
                Color targetColor = new Color(0f, 0f, 0f, 0f);
                float duration = 2.5f;
                float elapsedTime = 0f;

                while (elapsedTime < duration)
                {
                    elapsedTime += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsedTime / duration);
                    childLight.color = Color.Lerp(startColor, targetColor, t);
                    yield return null;
                }
            }
            idleFurs = idleFurs.Where(fur => fur != null).ToList();
            idleFurs.Add(newFur);
            furColorAssigned[newFur] = false;

            // furs 리스트를 이름 숫자 순으로 정렬
            furs.Sort((fur1, fur2) =>
            {
                int num1 = int.Parse(fur1.name.Replace("fur", ""));
                int num2 = int.Parse(fur2.name.Replace("fur", ""));
                return num1.CompareTo(num2);
            });

            // idleFurs 리스트도 이름 숫자 순으로 정렬
            idleFurs.Sort((fur1, fur2) =>
            {
                int num1 = int.Parse(fur1.name.Replace("fur", ""));
                int num2 = int.Parse(fur2.name.Replace("fur", ""));
                return num1.CompareTo(num2);
            });
        }
    }

    private IEnumerator AssignColorsWithDelay() // 파도타기 코루틴
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
                    else if(player.CompareTag("Fur6"))
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


    private IEnumerator RevertColorAfterDelay(Material material, Light childLight, Color initialColor, float delay, float duration) // 파도타기를 원래의 색으로 돌아오게하는 코루틴
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            PlayerData newPlayerData = new PlayerData();
            newPlayerData.conn_id = "NewPlayer_" + Time.time.ToString();
            OnAddUser(newPlayerData);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerData playerData = new PlayerData();
            RemoveUser(playerData);
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            //StartCoroutine(AssignColorsWithDelay());
            IdleMotion();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            StartCoroutine(fallingFur());
            //FallingMotion();
        }
    }
}