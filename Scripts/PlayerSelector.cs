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

    //public GameObject[] gameObjects;

    public List<GameObject> gameObjects;

    public Ease ease;

    //private List<Vector3> generatedPositions = new List<Vector3>(); // 이미 생성된 위치를 추적하는 리스트
    public HashSet<Color> changedColors = new HashSet<Color>(); // 변경된 색상을 추적하기 위한 HashSet

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
            Debug.Log("씬에 할당 가능한 오브젝트가 없습니다.");
            return;
        }

        // 이미 색상이 변경되지 않은 오브젝트를 찾아 할당
        GameObject availableObject = GetAvailableGameObjectExcludingChangedColors();
        if (availableObject == null)
        {
            Debug.Log("변경되지 않은 색상의 오브젝트를 찾을 수 없습니다.");
            return;
        }

        // 이미 존재하는 오브젝트 중 하나를 할당
        // Renderer renderer = availableObject.GetComponent<Renderer>();
        // if (renderer != null)
        // {
        //     Material material = renderer.material;
        //     Color color;
        //     if (UnityEngine.ColorUtility.TryParseHtmlString("#" + playerData.color_id, out color))
        //     {
        //         material.color = color;
        //         StartCoroutine(ChangeColorGradually(renderer, color, 10f));
        //         // 변경된 색상을 추적하기 위해 HashSet에 추가
        //         changedColors.Add(color);
        //     }
        //     else
        //     {
        //         Debug.LogError("컬러 생성 안댐: " + playerData.color_id);
        //     }
        // }
        //-------------------------------------------------------------------
        Renderer[] renderers = availableObject.GetComponentsInChildren<Renderer>(true);
        Color color;
        if (UnityEngine.ColorUtility.TryParseHtmlString("#" + playerData.color_id, out color))
        {
            foreach (Renderer renderer in renderers)
            {
                Material material = renderer.material;
                material.color = color;
                StartCoroutine(ChangeColorGradually(renderer, renderer.material.color, 3f));
            }
            changedColors.Add(color); // 변경된 색상을 추적하기 위해 HashSet에 추가
        }
        else
        {
            Debug.LogError("컬러 생성 안됨: " + playerData.color_id);
        }

        //-------------------------------------------------------------------

        // 플레이어 정보 설정
        targetPlayer = availableObject.GetComponent<Player>();
        targetPlayer.playerID = playerData.color_id;
        targetPlayer.SetPlayerColor(playerData.color_id);
        targetPlayer.SetUserIndex(playerData.player_index);


        // 유저 접속 시 효과 수정필
        targetPlayer.transform.DOScale(0.9f, 1f).SetEase(Ease.InElastic); // 4.23수정 : 0.9크기로 1초안에 변한다 -> 적합한 거 찾기
        //targetPlayer.transform.DOShakeScale(1, 1).SetEase(ease);

        players.Add(playerData.color_id, targetPlayer);

        Debug.Log("추가된 유저의 컬러값 : " + playerData.color_id + " , 유저의 GetInstanceID : " + targetPlayer.GetInstanceID());
        Debug.Log("추가된 유저 인덱스 :" + playerData.player_index);
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


    // 이미 변경된 색상이 아닌 오브젝트를 반환하는 메서드
    private GameObject GetAvailableGameObjectExcludingChangedColors()
    {
        List<GameObject> availableObjects = new List<GameObject>();

        foreach (GameObject obj in gameObjects)
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


    public GameObject GetAvailableGameObject()
    {
        List<GameObject> availableObjects = new List<GameObject>();

        foreach (GameObject obj in gameObjects)
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

    

    // Vector3 GetRandomPosition()
    // {
    //     if (gameObjects.Length == 0)
    //     {
    //         Debug.LogError("더 이상 씬 안에 할당받을 오브젝트가 없다");
    //         return Vector3.zero;
    //     }

    //     int randomIndex = UnityEngine.Random.Range(0, gameObjects.Length); // 배열에서 랜덤한 인덱스 설정

    //     Vector3 randomPosition = gameObjects[randomIndex].transform.position; // 선택한 인덱스에 해당하는 위치 설정

    //     return randomPosition;
    // }

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
}