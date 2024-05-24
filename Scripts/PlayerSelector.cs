using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using IMFINE.Utils.JoyStream.Communicator;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting.Dependencies.Sqlite;
//using IMFINE.Utils.JoyStream.Communicator.ext;

public class PlayerSelector : MonoBehaviour
{
    [Header("Settings")]
    private Dictionary<string, Player> players = new Dictionary<string, Player>();
    public HashSet<Color> changedColors = new HashSet<Color>(); // 변경된 색상을 추적하기 위한 HashSet
    public List<GameObject> furs = new List<GameObject>(); // 색을 할당받을 털 리스트
    private List<Vector3> furPositions = new List<Vector3>(); // 삭제 후 다시 생기기위한 털위치 리스트
    private HashSet<int> usedFur = new HashSet<int>(); // 사용된 fur 해시셋
    private Dictionary<string, PlayerData> playerDataList = new Dictionary<string, PlayerData>(); // 플레이어데이터 딕셔너
    //private Dictionary<string, string> colorToConnIdMap = new Dictionary<string, string>();

    [Header("DOtween & GameObject & Bool")]
    public Ease ease;
    public GameObject furPrefab;
    public bool isSpawn;
    public GameObject particlePrefab;
    CameraShake Camera;
    public bool idle = true;

    [Header("Idle")]
    private Coroutine idleMotionCoroutine; // 5.24 - idle진행되다가 한명이라도 접속 시 idleMotion 중지시키는 코루틴 (테스트 많이 해봐야함)
    private bool isIdleMotionRunning = false;
    public List<GameObject> idleFurs = new List<GameObject>(); // idle일때 움직이는 털 리스트 
    private Dictionary<GameObject, bool> furColorAssigned = new Dictionary<GameObject, bool>(); // fur의 색상 할당 상태를 추적

    private void Awake()
    {
        ProtocolManager.instance.onWebControllerEvent += OnWebControllerEvent;
        ProtocolManager.instance.onUserConnectEvent += OnUserConnectEvent;
        isSpawn = false;
        InitializeFurPositions();
    }

    void Start()
    {
        DOTween.Init();
        Camera = GameObject.FindWithTag("MainCamera").GetComponent<CameraShake>();
        idle = true;

        // 초기화: 모든 fur의 색상 할당 상태를 false로 설정
        foreach (var fur in furs)
        {
            furColorAssigned[fur] = false;
        }
        StartCoroutine(CheckPlayerCount(10f));
    }

    private IEnumerator CheckPlayerCount(float interval)
    {
        while (true)
        {
            if (players.Count == 0 && idle)
            {

                Debug.Log("플레이어가 없어서 다시 idle모드 발동");
                IdleMotion();
            }
            else if (players.Count > 0)
            {
                idle = false;
                Debug.Log("한명이라도 접속했기 때문에 idle모드 안댐");
            }


            yield return new WaitForSeconds(interval);
        }
    }




    private void InitializeFurPositions()
    {
        //furPositions.Clear();
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
                Debug.LogWarning("No player data found for disconnection.");
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

    public void OnAddUser(PlayerData playerData)
    {
        if (isIdleMotionRunning) // idle모드가 진행되고 있는 상황이라면
        {
            StopCoroutine("AssignColorsWithDelay"); // idle모드 멈추기
            isIdleMotionRunning = false; // idle모드 false
        }

        if (idleMotionCoroutine != null)
        {
            StopCoroutine(idleMotionCoroutine);
            idleMotionCoroutine = null;
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

                        DOVirtual.Color(childLight.color, targetColor, 2f, value =>
                        {
                            childLight.color = value;
                        });
                    }

                    targetPlayer.enabled = true;
                    changedColors.Add(targetColor); // 변경된 색상을 추적
                    furColorAssigned[assignedFur] = true; // 색상 할당 상태를 true로 설정
                }

                targetPlayer.playerID = playerData.conn_id;
                targetPlayer.SetPlayerColor(playerData.color_id);
                targetPlayer.SetUserIndex(furIndex);

                Sequence mySequence = DOTween.Sequence();

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
            if (!isIdleMotionRunning) // 만약 idle모드가 진행중이 아니라면
            {
                IdleMotion(); // idle 모드 실행 
            }
        }
    }

    public void IdleMotion()
    {
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

        // if(idle)
        // {
        //     StartCoroutine(AssignColorsWithDelay());
        // }
        
    }

    private IEnumerator AssignColorsWithDelay()
    {
        // 빨간계열과 파란계열 색상 리스트 생성
        List<Color> redColors = new List<Color> { Color.red, new Color(1f, 0.5f, 0.5f), new Color(1f, 0.2f, 0.2f) };
        List<Color> blueColors = new List<Color> { Color.blue, new Color(0.5f, 0.5f, 1f), new Color(0.2f, 0.2f, 1f) };

        // 색상 계열을 무작위로 선택
        List<Color> selectedColors = Random.Range(0, 2) == 0 ? redColors : blueColors;

        for (int i = 0; i < idleFurs.Count; i++)
        {
            GameObject idleFur = idleFurs[i];
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
                });

                StartCoroutine(RevertColorAfterDelay(material, childLight, initialColor, 1f, 1f));
            }
            else
            {
                Debug.LogWarning("Renderer가 존재하지 않습니다: " + idleFur.name);
            }

            if (player != null)
            {
                player.PushHingeJoint("fur", "push", 20f); // Player 컴포넌트의 PushHingeJoint 함수 호출
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



    public void RemoveUser(PlayerData playerData)
{
    string playerID = playerData.conn_id;
    if (players.ContainsKey(playerID))
    {
        Player player = players[playerID];
        GameObject furObject = player.gameObject;

        if (furObject != null)
        {
            int furIndex = furs.IndexOf(furObject);
            Vector3 initialPosition = furPositions[furIndex];

            furs.RemoveAt(furIndex);
            furPositions.RemoveAt(furIndex);
            usedFur.Remove(furIndex);

            // 해당 위치를 빈 자리로 만듭니다.
            furs.Insert(furIndex, null);
            furPositions.Insert(furIndex, Vector3.zero);

            // idleFurs에서 직접 요소 제거
            idleFurs.Remove(furObject);

            Light childLight = furObject.GetComponentInChildren<Light>();
            Renderer renderer = furObject.GetComponent<Renderer>();
            Rigidbody furRigidbody = furObject.GetComponent<Rigidbody>();
            if (childLight != null && renderer != null && furRigidbody != null)
            {
                furRigidbody.isKinematic = false;
                StartCoroutine(DimLightIntensity(childLight, 3f));

                renderer.material.DOFade(0f, 3f).SetEase(ease);
                Destroy(furObject, 4f);
            }
        }
        ColorManager.instance.ReturnColor(player.playerColor);
        players.Remove(playerID);
        TraceBox.Log("삭제된 플레이어의 아이디: " + playerID);

        if (players.Count == 0)
        {
            idle = true;
        }
    }
}

public IEnumerator RespawnFur(Vector3 position)
{
    yield return new WaitForSeconds(3.0f);

    if (furPrefab != null)
    {
        // 삭제된 위치에 새로운 fur를 추가합니다.
        for (int i = 0; i < furPositions.Count; i++)
        {
            if (furPositions[i] == Vector3.zero) // 5.24 수정필 -> 삭제된 해당 자리 찾기
            {
                // 빈 자리를 찾았을 때, 해당 위치에 새로운 fur를 추가합니다.
                GameObject newFur = Instantiate(furPrefab, position, Quaternion.identity);
                furs[i] = newFur;
                furPositions[i] = position;

                Renderer furRenderer = newFur.GetComponent<Renderer>();
                if (furRenderer != null)
                {
                    Color initialColor = furRenderer.material.color;
                    initialColor.a = 1f;
                    furRenderer.material.color = initialColor;
                }

                // idleFurs에 추가하기 전에 missing 항목을 정리
                idleFurs = idleFurs.Where(fur => fur != null).ToList();
                idleFurs.Add(newFur);
                furColorAssigned[newFur] = false;

                // 새로운 fur가 추가되었으므로 함수를 종료합니다.
                yield return null;
            }
        }

        // 만약 삭제된 위치에 새로운 fur를 추가할 위치를 찾지 못한 경우,
        // 리스트의 끝에 새로운 fur를 추가합니다.
        GameObject newFurAtEnd = Instantiate(furPrefab, position, Quaternion.identity);
        furs.Add(newFurAtEnd);
        furPositions.Add(position);

        Renderer furRendererAtEnd = newFurAtEnd.GetComponent<Renderer>();
        if (furRendererAtEnd != null)
        {
            Color initialColor = furRendererAtEnd.material.color;
            initialColor.a = 1f;
            furRendererAtEnd.material.color = initialColor;
        }

        // idleFurs에 추가하기 전에 missing 항목을 정리
        idleFurs = idleFurs.Where(fur => fur != null).ToList();
        idleFurs.Add(newFurAtEnd);
        furColorAssigned[newFurAtEnd] = false;
    }
}






    private IEnumerator DimLightIntensity(Light light, float duration)
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
            IdleMotion();
        }
    }
}