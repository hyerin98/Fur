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
    //[SerializeField] private GameObject playerPrefab = null;

    private Dictionary<string, Player> players = new Dictionary<string, Player>();

    //public Transform[] furSeets;

    public GameObject[] gameObjects;

    public Ease ease;

    private List<Vector3> generatedPositions = new List<Vector3>(); // 이미 생성된 위치를 추적하는 리스트
    public float minDistanceBetweenPositions = 0.5f; // 새로운 위치가 얼마나 멀어야 하는지 결정하는 최소 거리


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



    // private void OnAddUser(PlayerData playerData)
    // {
    //     ColorManager colorManager = ColorManager.instance;
    //     playerData.color_id = colorManager.AssignUserColor();
    //     int userIndex = colorManager.GetUserIndex();
    //     playerData.player_index = userIndex;

    //     Player targetPlayer;

    //     if (players.ContainsKey(playerData.color_id)) // 이미 컬러값을 가지고 있다면
    //     {
    //         targetPlayer = players[playerData.color_id];
    //         players.Remove(playerData.color_id);

    //         targetPlayer.isActive = false;
    //         targetPlayer.playerID = "0";
    //         targetPlayer.Test();

    //         Debug.Log("이미 존재하고 있는 컬러 : " + playerData.color_id + " : " + targetPlayer.GetInstanceID());
    //         Debug.Log("이미 존재하는 플레이어 아이디 : " + targetPlayer.playerID);
    //     }
    //     targetPlayer = Instantiate(playerPrefab, GetRandomPosition(), Quaternion.identity).GetComponent<Player>();

    //     // 유저 생성 시 효과
    //     targetPlayer.transform.DOScale(0, 1).SetEase(ease);
    //     targetPlayer.transform.DOShakeScale(1, 1).SetEase(ease);


    //     Debug.Log("유저번호: " + playerData.player_index + " 이 가지고 있는 컬러: " + playerData.color_id);
    //     Debug.Log("추가된 유저의 컬러값 : " + playerData.color_id + " : " + targetPlayer.GetInstanceID());

    //     players.Add(playerData.color_id, targetPlayer);
    //     targetPlayer.playerID = playerData.color_id;
    //     targetPlayer.SetPlayerColor(playerData.color_id);
    //     targetPlayer.SetUserIndex(playerData.player_index);

    //     // 색상 서서히 변경
    //     Renderer renderer = targetPlayer.GetComponent<Renderer>();
    //     if (renderer != null)
    //     {
    //         Material material = renderer.material;
    //         Color color;
    //         if (ColorUtility.TryParseHtmlString("#" + playerData.color_id, out color))
    //         {
    //             float transitionDuration = 2f; // 색이 채워지는데 걸리는 시간
    //             StartCoroutine(ChangeColorGradually(renderer, color, transitionDuration));
    //         }
    //         else
    //         {
    //             Debug.LogError("컬러 생성 안댐: " + playerData.color_id);
    //         }
    //     }
    // }


    private void OnAddUser(PlayerData playerData)
    {
        ColorManager colorManager = ColorManager.instance;
        playerData.color_id = colorManager.AssignUserColor();
        int userIndex = colorManager.GetUserIndex();
        playerData.player_index = userIndex;

        Player targetPlayer;

        GameObject assignedObject = GetAvailableGameObject();
        if (assignedObject == null)
        {
            Debug.LogError("씬에 할당 가능한 오브젝트가 없습니다.");
            return;
        }

        // 이미 존재하는 오브젝트 중 하나를 할당
        Renderer renderer = assignedObject.GetComponent<Renderer>();
        if (renderer != null)
        {

            Material material = renderer.material;
            Color color;
            if (UnityEngine.ColorUtility.TryParseHtmlString("#" + playerData.color_id, out color))
            {
                if (IsBrightColor(color))
                {
                    material.color = color;
                    float transitionDuration = 3f; // 색이 채워지는데 걸리는 시간
                    StartCoroutine(ChangeColorGradually(renderer, color, transitionDuration));
                }
                else if (!IsBrightColor(color))
                {
                    Debug.Log("색이 어둡다");
                    
                }
            }
            else
            {
                Debug.LogError("컬러 생성 안댐: " + playerData.color_id);
            }
        }


        // 오브젝트의 머티리얼 색상을 변경


        // 플레이어 정보 설정

        targetPlayer = assignedObject.GetComponent<Player>();
        targetPlayer.playerID = playerData.color_id;
        targetPlayer.SetPlayerColor(playerData.color_id);
        targetPlayer.SetUserIndex(playerData.player_index);

        targetPlayer.transform.DOScale(1, 1).SetEase(ease);
        targetPlayer.transform.DOShakeScale(1, 1).SetEase(ease);

        players.Add(playerData.color_id, targetPlayer);

        Debug.Log("추가된 유저의 컬러값 : " + playerData.color_id + " : " + targetPlayer.GetInstanceID());
        Debug.Log("추가된 유저 인덱스 :" + playerData.player_index);
    }

    private bool IsBrightColor(Color color)
    {
        float averageBrightness = (color.r + color.g + color.b) / 3f;
        return averageBrightness > 0.5f;
    }

    public GameObject GetAvailableGameObject()
    {
        List<GameObject> availableObjects = new List<GameObject>();

        foreach (GameObject obj in gameObjects)
        {

            Player playerComponent = obj.GetComponent<Player>();
            if (playerComponent != null && playerComponent.isActive) // !playerComponent.isActive 수정 필
            {
                availableObjects.Add(obj);
            }
            else
                return null;
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
    }

    Vector3 GetRandomPosition()
    {
        if (gameObjects.Length == 0)
        {
            Debug.LogError("더 이상 씬 안에 할당받을 오브젝트가 없다");
            return Vector3.zero;
        }

        int randomIndex = UnityEngine.Random.Range(0, gameObjects.Length); // 배열에서 랜덤한 인덱스 설정

        Vector3 randomPosition = gameObjects[randomIndex].transform.position; // 선택한 인덱스에 해당하는 위치 설정

        return randomPosition;
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
                // 딕셔너리에서 첫 번째 플레이어를 가져옴
                KeyValuePair<string, Player> firstPlayer = players.First();

                // 해당 플레이어를 삭제
                Destroy(firstPlayer.Value.gameObject);

                // 딕셔너리에서도 해당 플레이어를 삭제
                players.Remove(firstPlayer.Key);

                // ColorManager에서도 해당 플레이어를 삭제
                ColorManager.instance.RemoveUserAtIndex(0);
            }
            else
            {

                Debug.Log("삭제할 플레이어가 없습니다.");
            }
        }
    }

    // 4.17 ~ 랜덤한 자리에 생성하지 말고, 특정 공간안에서 랜덤하게 생성하게 수정필
    // Vector3 GetRandomPosition()
    // {
    //     if (furSeets.Length == 0)
    //     {
    //         Debug.LogError("furSeets 위치가 없슴");
    //         return Vector3.zero;
    //     }

    //     int randomIndex = UnityEngine.Random.Range(0, furSeets.Length); // 배열에서 랜덤한 인덱스 설정

    //     Vector3 randomPosition = furSeets[randomIndex].position; // 선택한 인덱스에 해당하는 위치 설정

    //     return randomPosition;
    // }

    // Vector3 GetRandomPosition() // 4.18 수정
    // {
    //     if (furSeets.Length == 0)
    //     {
    //         Debug.LogError("furSeets 위치가 없슴");
    //         return Vector3.zero;
    //     }

    //     int randomIndex;
    //     Vector3 randomPosition;

    //     do
    //     {
    //         randomIndex = UnityEngine.Random.Range(0, furSeets.Length); // 배열에서 랜덤한 인덱스 설정
    //         randomPosition = furSeets[randomIndex].position; // 선택한 인덱스에 해당하는 위치 설정
    //     }
    //     while (IsPositionDuplicate(randomPosition)); // 같은 위치를 피하기 위해 반복

    //     return randomPosition;
    // }

    bool IsPositionDuplicate(Vector3 newPosition)
    {
        // 이전에 생성된 모든 위치를 확인하여 중복을 찾음
        foreach (var position in generatedPositions)
        {
            if (Vector3.Distance(position, newPosition) < minDistanceBetweenPositions)
            {
                return true; // 새 위치가 이미 생성된 위치와 너무 가까우면 중복된 것으로 간주
            }
        }

        generatedPositions.Add(newPosition); // 새 위치를 생성된 위치 목록에 추가
        return false; // 중복 없음
    }

}