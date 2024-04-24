using DG.Tweening;
using IMFINE.Utils;
using IMFINE.Utils.ConfigManager;
using IMFINE.Utils.JoyStream.Communicator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Unity.VisualScripting.Antlr3.Runtime;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;

public class PlayerSelector : MonoBehaviour
{
    private Dictionary<string, Player> players = new Dictionary<string, Player>();

    public Ease ease;

    public HashSet<Color> changedColors = new HashSet<Color>(); // 변경된 색상을 추적하기 위한 HashSet

    public float minDistanceBetweenPositions = 0.5f; // 새로운 위치가 얼마나 멀어야 하는지 결정하는 최소 거리


    public List<GameObject> furs = new List<GameObject>();
    public List<Vector3> furPositions = new List<Vector3>();
    public GameObject furPrefab;


    private void Awake()
    {
        ProtocolManager.instance.onWebControllerEvent += OnWebControllerEvent;
        ProtocolManager.instance.onUserConnectEvent += OnUserConnectEvent;
    }


    private void OnUserConnectEvent(ProtocolType protocolType, PlayerData playerData)
    {
        switch (protocolType)
        {
            case ProtocolType.CONTROLLER_CONNECT:
                OnAddUser(playerData);
                break;
            case ProtocolType.CONTROLLER_DISCONNECT:
                OnRemoveUser(playerData);
                break;
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

      private void OnAddUser(PlayerData playerData)
{
    // 사용 가능한 플레이어 슬롯이 남아있는지 확인
    if (players.Count >= furs.Count)
    {
        Debug.Log("모든 플레이어 슬롯이 사용 중입니다. 더 이상 유저를 추가할 수 없습니다.");
        return; // 더 이상 플레이어를 추가할 수 없으므로 함수를 종료합니다.
    }

    ColorManager colorManager = ColorManager.instance;
    playerData.color_id = colorManager.AssignUserColor();
    if (playerData.color_id == null)
    {
        Debug.Log("할당 가능한 색상이 없습니다. 유저를 접속시킬 수 없습니다.");
        return; // 색상을 할당받지 못했다면 함수를 종료합니다.
    }

    int userIndex = players.Count; // 이미 접속한 유저 수를 바탕으로 인덱스 할당
    playerData.player_index = userIndex;

    GameObject assignedObject = furs[userIndex]; // 인덱스에 해당하는 furs 오브젝트를 할당

    // 색상 할당 및 플레이어 설정 로직...
    Player targetPlayer = assignedObject.GetComponent<Player>();
    if (targetPlayer != null)
    {
        Renderer[] renderers = assignedObject.GetComponentsInChildren<Renderer>();
        Color color;
        if (UnityEngine.ColorUtility.TryParseHtmlString("#" + playerData.color_id, out color))
        {
            foreach (Renderer renderer in renderers)
            {
                Material material = renderer.material;
                material.color = color;
                StartCoroutine(ChangeColorGradually(renderer, color, 3f)); // 점진적 색상 변경
            }
            changedColors.Add(color); // 변경된 색상을 추적
        }
        targetPlayer.playerID = playerData.color_id;
        targetPlayer.SetPlayerColor(playerData.color_id);
        targetPlayer.SetUserIndex(playerData.player_index);

        // 사용자 접속 시 효과
        targetPlayer.transform.DOScale(0.9f, 1f).SetEase(Ease.InElastic);

        players.Add(playerData.color_id, targetPlayer);
        Debug.Log("유저인덱스: " + playerData.player_index + " , 할당받은 컬러색: " + playerData.color_id);
    }
    else
    {
        Debug.LogError("할당된 GameObject에 Player 컴포넌트가 없습니다.");
    }
}

    private IEnumerator ChangeColorGradually(Renderer renderer, Color targetColor, float duration)
    {
        Color initialColor = renderer.material.color;
        float timer = 0.0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);
            renderer.material.color = Color.Lerp(initialColor, targetColor, t);
            yield return null; // 다음 프레임까지 대기
        }
        renderer.material.color = targetColor;
    }

    void DestroyFur()
    {
        if(furs.Count > 0)
        {
            GameObject furToDestroy = furs[0];
            furs.RemoveAt(0);
            Vector3 positionOfDestroyed = furToDestroy.transform.position; // 삭제될 오브젝트의 위치를 저장
            Destroy(furToDestroy);

            // 일정 시간 후에 오브젝트를 다시 생성
            StartCoroutine(RespawnFur(positionOfDestroyed));
        }
    }
    
    IEnumerator RespawnFur(Vector3 position)
    {
        yield return new WaitForSeconds(3.0f);

        GameObject newFur = Instantiate(furPrefab, position, Quaternion.identity);
        furs.Add(newFur); 
    }


    private GameObject GetAvailableGameObjectExcludingChangedColors()
    {
        List<GameObject> availableObjects = new List<GameObject>();

        foreach (GameObject obj in furs)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material material = renderer.material;
                Color objectColor = material.color;
                if (!changedColors.Contains(objectColor)) 
                {
                    Player playerComponent = obj.GetComponent<Player>();
                    if (playerComponent != null && playerComponent.isActive)
                    {
                        availableObjects.Add(obj);
                    }
                }
            }
        }

        if (availableObjects.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, availableObjects.Count);
            Player playerComponent = availableObjects[randomIndex].GetComponent<Player>();
            if (playerComponent != null)
            {
                playerComponent.isActive = true;
                return availableObjects[randomIndex];
            }
        }
        return null;
    }


    public GameObject GetAvailableGameObject()
    {
        List<GameObject> availableObjects = new List<GameObject>();

        foreach (GameObject obj in furs)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material material = renderer.material;
                Color objectColor = material.color;
                if (!changedColors.Contains(objectColor)) // 변경된 색상이 아닌 경우에만 추가
                {
                    Player playerComponent = obj.GetComponent<Player>();
                    if (playerComponent != null && playerComponent.isActive)
                    {
                        availableObjects.Add(obj);
                    }
                }
            }
        }

        if (availableObjects.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, availableObjects.Count);
            Player playerComponent = availableObjects[randomIndex].GetComponent<Player>();
            if (playerComponent != null)
            {
                playerComponent.isActive = true;
                return availableObjects[randomIndex];
            }
        }
        return null;
    }

    private void RemoveUser(string playerID)
{
    if (players.ContainsKey(playerID))
    {
        Player player = players[playerID];
        ColorManager.instance.ReturnColor(player.playerColor); // 플레이어 색상 반환
        players.Remove(playerID); // 플레이어 목록에서 제거
        // 여기에 플레이어 오브젝트를 파괴하거나 재활용하는 추가 로직이 들어갈 수 있습니다.
    }
    else
    {
        Debug.LogWarning("삭제하려는 플레이어 ID가 존재하지 않습니다: " + playerID);
    }
}



    private void OnRemoveUser(PlayerData playerData) // 4.15 수정필
    {
        if (players.ContainsKey(playerData.color_id))
        {
            players[playerData.color_id].isActive = false;
            //players[playerData.conn_id].isActive = false; 
            //players[playerData.color_id].RemoveUserAtIndex();
            players[playerData.color_id].RemovePlayer();
            //players[playerData.conn_id].RemovePlayer();
            return;
        }
    }

    private void OnPlayerEnd(Player targetPlayer)
    {
        if (players.ContainsValue(targetPlayer))
        {
            targetPlayer.onPlayerEnd -= OnPlayerEnd;
            players.Remove(targetPlayer.playerID);
            targetPlayer.isActive = false;
        }
    }



    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            PlayerData playerData = new PlayerData(" ", 0);
            OnAddUser(playerData);
            //Debug.Log(playerData.conn_id + "");
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            // players 딕셔너리에 플레이어가 있는지 확인
            if (players.Count > 0)
            {
                DestroyFur();
                 string lastPlayerID = players.Keys.Last(); 
                RemoveUser(lastPlayerID);
            }
            else
            {
                Debug.Log("삭제할 플레이어가 없습니다.");
            }
        }
    }
}