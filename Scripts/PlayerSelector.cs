using DG.Tweening;
using IMFINE.Utils;
using IMFINE.Utils.ConfigManager;
using IMFINE.Utils.JoyStream.Communicator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class PlayerSelector : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab = null;

    private Dictionary<string, Player> players = new Dictionary<string, Player>();

    public Transform[] furSeets;

    Tweener floatTweener;
    Sequence showSequence, hideSequence;
    
    public Ease ease;


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

        if (players.ContainsKey(playerData.color_id)) // 이미 컬러값을 가지고 있다면
        {
            targetPlayer = players[playerData.color_id];
            players.Remove(playerData.color_id);

            targetPlayer.isActive = false;
            targetPlayer.playerID = "0";
            targetPlayer.Test();

            Debug.Log("이미 존재하고 있는 컬러 : " + playerData.color_id + " : " + targetPlayer.GetInstanceID());
            Debug.Log("이미 존재하는 플레이어 아이디 : " + targetPlayer.playerID);
        }
        targetPlayer = Instantiate(playerPrefab, GetNextPosition(), Quaternion.identity).GetComponent<Player>();

        targetPlayer.transform.DOScale(0,1).SetEase(ease);
        targetPlayer.transform.DOShakeScale(1,1).SetEase(ease);

        // targetPlayer.transform.DOScale(1.2f, 1.2f); // 생성 효과 추가 (수정필)
        // targetPlayer.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 1.0f)
        // .SetEase(Ease.InOutSine)
        // .SetLoops(-1, LoopType.Yoyo);

        Debug.Log("유저번호: " + playerData.player_index + " 이 가지고 있는 컬러: " + playerData.color_id);
        Debug.Log("추가된 유저의 컬러값 : " + playerData.color_id + " : " + targetPlayer.GetInstanceID());

        players.Add(playerData.color_id, targetPlayer);
        targetPlayer.playerID = playerData.color_id;
        targetPlayer.SetPlayerColor(playerData.color_id);
        targetPlayer.SetUserIndex(playerData.player_index);

        Renderer renderer = targetPlayer.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material material = renderer.material;
            Color color;
            if (ColorUtility.TryParseHtmlString("#" + playerData.color_id, out color))
            {
                renderer.material.color = color;
            }
            else
            {
                Debug.LogError("컬러 생성 안댐: " + playerData.color_id);
            }
        }
    }


    private void OnRemoveUser(PlayerData playerData) // 4.15 수정필
    {
        if (players.ContainsKey(playerData.color_id))
        {
            players[playerData.color_id].isActive = false;
            //players[playerData.color_id].RemoveUserAtIndex();
            players[playerData.color_id].RemovePlayer();
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
        // if (Input.GetKeyDown(KeyCode.Return))
        // {
        //     SpawnPlayer();
        // }

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
    Vector3 RandomPosition() // 플레이어 랜덤하게 생성
    {

        Vector3 randomPosition;
        do
        {
            randomPosition = new Vector3(UnityEngine.Random.Range(-8f, 8f), UnityEngine.Random.Range(-5f, 5f)); // 4.15 random오류나서 UnityEngine써서 수정함
        } while (Physics.CheckSphere(randomPosition, 1f)); // 반지름의 구를 기준으로 겹치는지 확인
        return randomPosition;
    }
    Vector3 GetNextPosition()
    {
        if (furSeets.Length == 0)
        {
            Debug.LogError("정해진 자리가 없슴");
            return Vector3.zero;
        }


        Vector3 nextPosition = furSeets[0].position;
        furSeets = furSeets.Skip(1).ToArray();

        return nextPosition;
    }

}