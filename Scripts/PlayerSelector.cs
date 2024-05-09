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

    [Header("DOtween & GameObject & Bool")]
    public Ease ease;
    public GameObject furPrefab;
    public GameObject newFurEffect;
    public bool isSpawn;
    public GameObject particlePrefab;
    [SerializeField] AnimationCurve curve;
    Tweener floatTweener;
    Sequence showSequence, hideSequence;
    public Transform pos;

    void Start()
    {
        DOTween.Init();
    }
    private void Awake()
    {
        ProtocolManager.instance.onWebControllerEvent += OnWebControllerEvent;
        ProtocolManager.instance.onUserConnectEvent += OnUserConnectEvent;
        isSpawn = false;
        InitializeFurPositions();
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
            playerDataList[playerData.conn_id] = playerData;  // 연결된 사용자 정보 저장
            OnAddUser(playerData);
        }
        else if (protocolType == ProtocolType.CONTROLLER_DISCONNECT)
        {
            if (playerDataList.TryGetValue(playerData.conn_id, out PlayerData storedPlayerData)) //저장된 PlayerData를 사용하여 제거
            {
                RemoveUser(storedPlayerData.color_id);
                playerDataList.Remove(playerData.conn_id);  // 더 이상 필요 없으므로 삭제
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
                TraceBox.Log("위키누름!!");
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


    // public static string set_color = "set_color";
    // public static readonly string set_colro2 = "aaa";
    // public const string set_colro3 = "asaaaa";

    public void OnAddUser(PlayerData playerData)
    {
        // var r = JoyStreamCommunicator.instance.CustomSample(10, 20);
        // TraceBox.Log(">>>>>>>>>" +r);
        //JoyStreamCommunicator.instance.SendMessage("set_color");

        //JoyStreamCommunicator.instance.SendToMobile("user_connect",playerData.color_id);

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

            int randomIndex = UnityEngine.Random.Range(0, availableFur.Count);
            int furIndex = availableFur[randomIndex];
            GameObject assignedFur = furs[furIndex];
            playerData.player_index = furIndex;

            // 색상 할당 및 플레이어 설정 로직...
            Player targetPlayer = assignedFur.GetComponent<Player>();

            targetPlayer.SetUserIndex(playerData.player_index);
            if (targetPlayer != null)
            {
                Renderer renderer = assignedFur.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Material material = renderer.material;
                    Color targetColor;

                    // Emission을 활성화합니다.
                    material.EnableKeyword("_EMISSION");

                    if (UnityEngine.ColorUtility.TryParseHtmlString("#" + playerData.color_id, out targetColor))
                    {
                        // 색상이 파싱되면 서서히 색상 변경을 시작
                        DOVirtual.Color(material.color, targetColor, 3.5f, value =>
                        {
                            material.color = value;

                            // Emission 색상도 같이 변경합니다.
                            material.SetColor("_EmissionColor", value);
                        });
                    }

                    // 랜덤한 색상과 강도로 emission 색상을 설정
                    Color randomEmissionColor = UnityEngine.Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.75f, 1f);
                    float intensity = 10f; 
                    randomEmissionColor *= intensity; // 색상에 강도를 곱하여 밝기 조절
                    material.SetColor("_EmissionColor", randomEmissionColor);

                    targetPlayer.enabled = true;
                    changedColors.Add(targetColor); // 변경된 색상을 추적
                }
                targetPlayer.playerID = playerData.color_id;
                targetPlayer.SetPlayerColor(playerData.color_id);
                targetPlayer.SetUserIndex(furIndex);

                Sequence mySequence = DOTween.Sequence();

                mySequence.Append(targetPlayer.transform.DOScale(3f, 0.25f).SetEase(Ease.OutBack))
                .Append(targetPlayer.transform.DOScale(1f, 0.15f).SetEase(Ease.InQuad))
                .Append(targetPlayer.transform.DOScale(1.8f, 0.1f).SetEase(Ease.InBack))
                .Play();

                usedFur.Add(furIndex);
                players.Add(playerData.color_id, targetPlayer);
                GameObject tempEffect = Instantiate(newFurEffect, targetPlayer.transform.position, Quaternion.identity);
                Destroy(tempEffect, 1.0f);
                playerData.player_index = players.Count;  // 플레이어 인덱스 설정

                //JoyStreamCommunicator.instance.SendToMobile(playerData.conn_id, "user_connect", playerData.color_id);
            }
            else
            {
                TraceBox.Log("할당된 GameObject에 Player 컴포넌트가 없습니다.");
            }
        }
    }


    public void RemoveUser(string playerID)
    {
        if (players.ContainsKey(playerID))
        {
            Player player = players[playerID];
            GameObject furObject = player.gameObject; // 4.30 계속 삭제 시 missing이슈 -> 5번눌러서 삭제하는거랑 겹쳐서그런듯 
            if (player.playerColor != null)
            {
                ColorManager.instance.ReturnColor(player.playerColor);
            }
            if (furObject != null)
            {
                int furIndex = furs.IndexOf(furObject);
                if (furIndex != -1)
                {
                    Vector3 initialPosition = furPositions[furIndex];
                    furs.RemoveAt(furIndex);
                    furPositions.RemoveAt(furIndex);
                    usedFur.Remove(furIndex);

                    // 파티클 효과 실행
                    //GameObject particles = Instantiate(particlePrefab, furObject.transform.position, Quaternion.identity);
                    //Destroy(particles, 2.0f); // 파티클이 자동으로 사라지도록 설정

                    // furObject.transform.DOMove(pos.transform.position, 3f).SetEase(Ease.InExpo).SetEase(Ease.OutBounce); // 5.7 위코드에서 수정했는데 다시 수정필
                    // furObject.transform.DOScale(1f, 0.5f).OnComplete(() =>
                    // {
                    //     Destroy(furObject);
                    //     StartCoroutine(RespawnFur(initialPosition));
                    // });
                    hideSequence = DOTween.Sequence().SetAutoKill(true)
                    .Join(furObject.transform.DOLocalRotate(new Vector3(70, 30, 50), 0.5f).SetEase(Ease.InElastic, 0.5f))
                    .Join(furObject.transform.DOScale(0, 3f).SetEase(Ease.InElastic, 0.1f))
                    .OnComplete(() =>
                    {
                        if(player.isFalled)
                        {
                            Destroy(furObject);
                            StartCoroutine(RespawnFur(initialPosition));
                        }
                        
                    });
                }
            }
            ColorManager.instance.ReturnColor(player.playerColor);
            players.Remove(playerID);
        }
        else
        {
            TraceBox.Log("Player not found with ID: " + playerID);
        }
    }

    public IEnumerator RespawnFur(Vector3 position)
    {
        yield return new WaitForSeconds(3.0f); // 3초 대기

        if (furPrefab != null)
        {
            GameObject newFur = Instantiate(furPrefab, position, Quaternion.identity);
            furs.Add(newFur);
            furPositions.Add(position); // 새 fur의 위치를 리스트에 추가

            Renderer furRenderer = newFur.GetComponent<Renderer>(); // Renderer 컴포넌트를 가져옴
            if (furRenderer != null)
            {
                // Material 투명도를 초기에 0으로 설정
                Color initialColor = furRenderer.material.color;
                initialColor.a = 0f;
                furRenderer.material.color = initialColor;

                Sequence seq = DOTween.Sequence();
                seq.Append(newFur.transform.DOScale(1f, 0.5f)) // 처음 등장시 효과
                    .Join(furRenderer.material.DOFade(3f, 1f)) // 동시에 페이드 인
                    .Append(newFur.transform.DOScale(2.0f, 1f).SetEase(Ease.InOutElastic)); // 커지는 효과
            }
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (players.Count > 0)
            {
                string firstPlayerID = players.Keys.FirstOrDefault(); // 딕셔너리에서 첫 번째 플레이어의 ID를 가져옵니다.
                if (!string.IsNullOrEmpty(firstPlayerID))
                {
                    RemoveUser(firstPlayerID);
                    Debug.Log("삭제된 유저 컬러값 - " + firstPlayerID);
                }
            }
            else
            {
                Debug.Log("삭제할 플레이어가 없습니당");
            }
        }
    }
}