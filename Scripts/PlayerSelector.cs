using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using IMFINE.Utils.JoyStream.Communicator;
using UnityEngine;
using System.Linq;

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
        // Debug.Log($"Received {protocolType} event with PlayerData: {playerData}");
        if (protocolType == ProtocolType.CONTROLLER_CONNECT)
        {
            //Debug.Log("접속하기 전 유저의 컬러아이디: " + playerData.color_id);
            playerDataList[playerData.conn_id] = playerData;  // 연결된 사용자 정보 저장
            OnAddUser(playerData);
            //Debug.Log("접속 후 유저의 컬러아이디: " + playerData.color_id);
        }
        else if (protocolType == ProtocolType.CONTROLLER_DISCONNECT)
        {
            //저장된 PlayerData를 사용하여 제거
            if (playerDataList.TryGetValue(playerData.conn_id, out PlayerData storedPlayerData))
            {
                //Debug.Log("삭제하기 전 유저의 컬러아이디: " + storedPlayerData.color_id);
                RemoveUser(storedPlayerData.color_id);

                //Debug.Log("삭제 후 유저의 컬러아이디: " + storedPlayerData.color_id);
                playerDataList.Remove(playerData.conn_id);  // 더 이상 필요 없으므로 삭제
            }
            else
            {
                Debug.LogWarning("No player data found for disconnection.");
            }

            //-------------------------------
            //RequestRemovePlayer(playerData.conn_id); // 4.30 수정필
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

            int randomIndex = UnityEngine.Random.Range(0, availableFur.Count);
            int furIndex = availableFur[randomIndex];
            GameObject assignedFur = furs[furIndex];
            playerData.player_index = furIndex;
            

            // 레이어 설정 . .?

            // 색상 할당 및 플레이어 설정 로직...
            Player targetPlayer = assignedFur.GetComponent<Player>();
            
            targetPlayer.SetUserIndex(playerData.player_index);
            if (targetPlayer != null)
            {
                Renderer[] renderers = assignedFur.GetComponentsInChildren<Renderer>();
                Color color;
                if (UnityEngine.ColorUtility.TryParseHtmlString("#" + playerData.color_id, out color))
                {
                    foreach (Renderer renderer in renderers)
                    {
                        Material material = renderer.material;
                        material.color = color;
                        StartCoroutine(ChangeColorGradually(renderer, color, 60f)); // 점진적 색상 변경
                        targetPlayer.enabled = true;
                    }
                    changedColors.Add(color); // 변경된 색상을 추적
                }
                targetPlayer.playerID = playerData.color_id;
                targetPlayer.SetPlayerColor(playerData.color_id);
                targetPlayer.SetUserIndex(furIndex);
                targetPlayer.transform.DOScale(1.3f, 0.5f).SetEase(ease); // 유저 접속 시 두트윈으로 효과주기
                usedFur.Add(furIndex);
                players.Add(playerData.color_id, targetPlayer);
                GameObject tempEffect = Instantiate(newFurEffect, targetPlayer.transform.position, Quaternion.identity);
                Destroy(tempEffect, 2.0f);
                playerData.player_index = players.Count;  // 플레이어 인덱스 설정
                
                Debug.Log("새로운 유저 들어옴: " + furIndex + " , " + "유저의 컬러값과 인덱스는: " + playerData.color_id + " , " + playerData.player_index);
                //Debug.Log("Players before removing: " + string.Join(", ", players.Keys));
            }
            else
            {
                Debug.LogError("할당된 GameObject에 Player 컴포넌트가 없습니다.");
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
                GameObject particles = Instantiate(particlePrefab, furObject.transform.position, Quaternion.identity);
                Destroy(particles, 2.0f); // 파티클이 자동으로 사라지도록 설정

                // 오브젝트를 트윈 효과로 사라지게 함
                furObject.transform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
                {
                    Destroy(furObject);
                    StartCoroutine(RespawnFur(initialPosition)); 
                });
                }
            }
            //ColorManager.instance.ReturnColor(player.playerColor);
            players.Remove(playerID);
            Debug.Log("Removed player with ID: " + playerID);
        }
        else
        {
            Debug.LogWarning("Player not found with ID: " + playerID); // 5.2 디버깅페이지에서 경고뜸
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
        }
    }
    private IEnumerator ChangeColorGradually(Renderer renderer, Color targetColor, float duration)
    {
        Color initialColor = renderer.material.color;
        float timer = 0.0f;

        while (timer < duration)
        {
            if (renderer == null || renderer.gameObject == null)
            {
                yield break;
            }
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);
            renderer.material.color = Color.Lerp(initialColor, targetColor, t);
            yield return null; // 다음 프레임까지 대기
        }
        if (renderer != null && renderer.gameObject != null)
        {
            renderer.material.color = targetColor;
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