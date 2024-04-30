using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using IMFINE.Utils;
using IMFINE.Utils.ConfigManager;
using IMFINE.Utils.JoyStream.Communicator;
using UnityEngine;

public class ProtocolManager : MonoSingleton<ProtocolManager>
{
    [SerializeField] bool _enableDetaledLog = false;
    public delegate void WebControllerEvent(ProtocolType protocolType, string colorId);
    public event WebControllerEvent onWebControllerEvent;

    public delegate void UserConnectEvent(ProtocolType protocolType, PlayerData playerData);
    public event UserConnectEvent onUserConnectEvent;
    public delegate void ModeDelegate(bool isActive);
    public event ModeDelegate idleModeChanged;
    public event ModeDelegate enterModeChanged;
    public delegate void UserActionDelegate(string colorId);
    public event UserActionDelegate onUserReplayEvent;
    private PlayerSelector playerSelector;

    private void Start()
    {
        playerSelector = FindObjectOfType<PlayerSelector>();
        if (ConfigManager.instance.isPrepared) OnConfigDataPrepared();
        else ConfigManager.instance.Prepared += OnConfigDataPrepared;
    }

    private void OnConfigDataPrepared()
    {
        JoyStreamCommunicator.instance.MessageReceived += ReceiveMessage; // 시그널 메세지 수신 
        //JoyStreamCommunicator.instance.UserEnter += OnUserEnter; // 사용자 들어왔을 때
        //JoyStreamCommunicator.instance.UserExit += OnUserExit; // 사용자 나갔을 때
        JoyStreamCommunicator.instance.KeyDown += OnKeyDown; // 사용자가 키를 눌렀을 때
        JoyStreamCommunicator.instance.KeyUp += OnKeyUp; // 사용자가 키를 뗐을 때
        JoyStreamCommunicator.instance.Prepared += OnPrepared; // 준비가 되었을 때, 서버와 연결되었고 사용자 리스트를 서버로부터 받아왔을 때

        JoyStreamCommunicator.instance.MaxPlayerCount = ConfigManager.instance.data.maxPlayerCount;

#if !UNITY_EDITOR
        JoyStreamCommunicator.instance.Connect(ConfigManager.instance.data.serverURL, "fur");
#else
        JoyStreamCommunicator.instance.Connect(ConfigManager.instance.data.serverURL, "furtest");
#endif
        // DOTween라이브러리의 DelayedCall메서드. 지정된 시간이 지난 후에 지정된 작업을 실행
        DOVirtual.DelayedCall(5, () => SendIdleModeEvent(true)).SetId("IdleTimer" + GetInstanceID());
    }

    private void ReceiveMessage(string conn_id, string key_code, string color_id, string value)
    {
        TraceBox.Log("Message Received / connId: " + conn_id + " / key_code: " + key_code +" / colorID: " + color_id + " / value: " + value);
        switch (key_code)
        {
            case "game_replay":
                {
                    onUserReplayEvent?.Invoke(conn_id);
                    JoyStreamCommunicator.instance.SendToMobile(conn_id, "user_color", JoyStreamCommunicator.instance.GetPlayerIndex(color_id).ToString());
                    //JoyStreamCommunicator.instance.SendToMobile(conn_id, "user_connect", JoyStreamCommunicator.instance.ThemeType + "," + JoyStreamCommunicator.instance.GetPlayerIndex(conn_id).ToString());
                    break;
                }
        }
    }

    private void OnPrepared()
    {
        if (_enableDetaledLog) TraceBox.Log("JoyStream Communicator Prepared");
    }

    // private void OnUserEnter(PlayerData playerData) 
    // {
    //     if (_enableDetaledLog) TraceBox.Log("User Enter / connID: " + playerData.conn_id + " / color: " + playerData.color_id + " / index: " + playerData.player_index);
    //     OnReceivedUserConnect(playerData);
    //     if (JoyStreamCommunicator.instance.GetPlayerCount() == 1)
    //     {
    //         SendIdleModeEvent(true);
    //     }
    //     else if (JoyStreamCommunicator.instance.GetPlayerCount() == 51)
    //     {
    //         enterModeChanged?.Invoke(false);
    //     }
    // }

    // private void OnUserExit(PlayerData playerData)
    // {
    //     if (_enableDetaledLog) TraceBox.Log("User Exit / connID: " + playerData.conn_id + " / color " + playerData.color_id + " / index: " + playerData.player_index);
    //     OnReceivedUserDisconnect(playerData);

    //     if (JoyStreamCommunicator.instance.GetPlayerCount() == 0)
    //     {
    //         DOVirtual.DelayedCall(5, () => SendIdleModeEvent(true)).SetId("IdleTimer" + GetInstanceID());
    //     }
    //     else if (JoyStreamCommunicator.instance.GetPlayerCount() == 49)
    //     {
    //         enterModeChanged?.Invoke(true);
    //     }
    // }
    //-----------
    private void OnUserEnter(string connId)
{
    // ProtocolManager는 PlayerSelector에 사용자 추가를 요청
     
    PlayerData newPlayerData = new PlayerData(connId);
    playerSelector.OnAddUser(newPlayerData);

}

private void OnUserExit(string connId)
{
    // ProtocolManager는 PlayerSelector에 사용자 제거를 요청
    playerSelector.RemoveUser(connId);
}


    private void SendIdleModeEvent(bool isActive)
    {
        if (isActive)
        {
            if (_enableDetaledLog) TraceBox.Log("Idle Mode Active!");
            idleModeChanged?.Invoke(true);
        }
        else
        {
            if (_enableDetaledLog) TraceBox.Log("Idle Mode Disactive!");
            DOTween.Kill("IdleTimer" + GetInstanceID());
            idleModeChanged?.Invoke(false);
        }
    }

    private void OnKeyDown(string connID, int keyCode)
    {
        if (_enableDetaledLog) TraceBox.Log("Key Down / connID: " + connID + " / keyCode: " + keyCode);

        switch (keyCode)
        {
            case 37:
                OnReceivedControllerLeft_Press(connID);
                break;
            case 38:
                OnReceivedControllerUp_Press(connID);
                break;
            case 39:
                OnReceivedControllerRight_Press(connID);
                break;
            case 40:
                OnReceivedControllerDown_Press(connID);
                break;
            case 32:
                OnReceivedControllerFall_Press(connID); 
                break;
        }
    }

    private void OnKeyUp(string connID, int keyCode)
    {
        if (_enableDetaledLog) TraceBox.Log("Key Up / connID: " + connID + " / keyCode: " + keyCode);

        switch (keyCode)
        {
            case 37:
                OnReceivedControllerLeft_Release(connID);
                break;
            case 38:
                OnReceivedControllerUp_Release(connID);
                break;
            case 39:
                OnReceivedControllerRight_Release(connID);
                break;
            case 40:
                OnReceivedControllerDown_Release(connID);
                break;
        }
    }

    public void OnReceivedUserConnect(PlayerData userData)
    {
        onUserConnectEvent?.Invoke(ProtocolType.CONTROLLER_CONNECT, userData);
    }

    public void OnReceivedUserDisconnect(PlayerData userData)
    {
        onUserConnectEvent?.Invoke(ProtocolType.CONTROLLER_DISCONNECT, userData);
    }

    public void OnReceivedControllerFall_Press(string conID) 
    {
        onWebControllerEvent?.Invoke(ProtocolType.CONTROLLER_FALL_PRESS, conID);
    }

    public void OnReceivedControllerUp_Press(string connID)
    {
        onWebControllerEvent?.Invoke(ProtocolType.CONTROLLER_UP_PRESS, connID);
    }

    public void OnReceivedControllerUp_Release(string connID)
    {
        onWebControllerEvent?.Invoke(ProtocolType.CONTROLLER_UP_RELEASE, connID);
    }

    public void OnReceivedControllerDown_Press(string connID)
    {
        onWebControllerEvent?.Invoke(ProtocolType.CONTROLLER_DOWN_PRESS, connID);
    }

    public void OnReceivedControllerDown_Release(string connID)
    {
        onWebControllerEvent?.Invoke(ProtocolType.CONTROLLER_DOWN_RELEASE, connID);
    }

    public void OnReceivedControllerLeft_Press(string connID)
    {
        onWebControllerEvent?.Invoke(ProtocolType.CONTROLLER_LEFT_PRESS, connID);
    }

    public void OnReceivedControllerLeft_Release(string connID)
    {
        onWebControllerEvent?.Invoke(ProtocolType.CONTROLLER_LEFT_RELEASE, connID);
    }
    public void OnReceivedControllerRight_Press(string connID)
    {
        onWebControllerEvent?.Invoke(ProtocolType.CONTROLLER_RIGHT_PRESS, connID);
    }

    public void OnReceivedControllerRight_Release(string connID)
    {
        onWebControllerEvent?.Invoke(ProtocolType.CONTROLLER_RIGHT_RELEASE, connID);
    }
}
