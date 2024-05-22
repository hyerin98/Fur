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

    private void Awake()
    {
        ProtocolManager.instance.onWebControllerEvent += OnWebControllerEvent;
        ProtocolManager.instance.onUserConnectEvent += OnUserConnectEvent;
        isSpawn = false;
        InitializeFurPositions(); ;
    }

    void Start()
    {
        DOTween.Init();
        Camera = GameObject.FindWithTag("MainCamera").GetComponent<CameraShake>();
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
                }
                targetPlayer.playerID = playerData.conn_id;
                targetPlayer.SetPlayerColor(playerData.color_id);
                targetPlayer.SetUserIndex(furIndex);

                Sequence mySequence = DOTween.Sequence();

                usedFur.Add(furIndex);
                players.Add(playerData.conn_id, targetPlayer);
                //colorToConnIdMap.Add(playerData.color_id, playerData.conn_id); // 5.17 수정 -> 최대컬러수 할당받고 나면 컨트롤러 흰색으로 뜨는 이슈 원인 
                playerData.player_index = players.Count;  // 플레이어 인덱스 설정
            }
            else
            {
                TraceBox.Log("할당된 GameObject에 Player 컴포넌트가 없습니다.");
            }
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

                Light childLight = furObject.GetComponentInChildren<Light>();
                Renderer renderer = furObject.GetComponent<Renderer>();
                Rigidbody furRigidbody = furObject.GetComponent<Rigidbody>();
                if (childLight != null && renderer != null && furRigidbody != null)
                {
                    furRigidbody.isKinematic = false;
                    StartCoroutine(DimLightIntensity(childLight, 3f));

                    renderer.material.DOFade(0f, 3f).SetEase(ease);
                    Destroy(furObject, 10f);
                    StartCoroutine(RespawnFur(initialPosition));
                }
            }
            ColorManager.instance.ReturnColor(player.playerColor);
            players.Remove(playerID);
            TraceBox.Log("삭제된 플레이어의 아이디: " + playerID);
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

    public IEnumerator RespawnFur(Vector3 position)
    {
        yield return new WaitForSeconds(3.0f);

        if (furPrefab != null)
        {
            GameObject newFur = Instantiate(furPrefab, position, Quaternion.identity);
            furs.Add(newFur);
            furPositions.Add(position);

            Renderer furRenderer = newFur.GetComponent<Renderer>();
            if (furRenderer != null)
            {
                Color initialColor = furRenderer.material.color;
                initialColor.a = 1f;
                furRenderer.material.color = initialColor;
            }
        }
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
    }

}