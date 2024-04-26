using IMFINE.Utils.JoyStream.Communicator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugKeyboardControllManager : MonoBehaviour
{
    private string selectedID = "0";

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (selectedID == "1")
            {
                PlayerData playerData = new PlayerData(); // 모든 값이 기본값
                playerData.conn_id = "0";
                playerData.color_id = "0";
                ProtocolManager.instance.OnReceivedUserDisconnect(playerData);
                selectedID = "0";
                Debug.Log("1번눌렀고, 플레이어 연결x selectedID는? " + selectedID);
            }
            else
            {
                PlayerData playerData = new PlayerData();
                playerData.conn_id = "1";
                playerData.color_id = "1";
                selectedID = "1";
                ProtocolManager.instance.OnReceivedUserConnect(playerData);
                Debug.Log("1번 눌렀고, 플레이어 연결O selectedID는? " + selectedID);
            }

        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (selectedID == "2")
            {
                PlayerData playerData = new PlayerData();
                playerData.conn_id = "2";
                ProtocolManager.instance.OnReceivedUserDisconnect(playerData);
                selectedID = "0";
            }
            else
            {
                PlayerData playerData = new PlayerData();
                playerData.conn_id = "2";
                selectedID = "2";
                ProtocolManager.instance.OnReceivedUserConnect(playerData);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (selectedID == "3")
            {
                PlayerData playerData = new PlayerData();
                playerData.conn_id = "3";
                ProtocolManager.instance.OnReceivedUserDisconnect(playerData);
                selectedID = "0";
            }
            else
            {
                PlayerData playerData = new PlayerData();
                playerData.conn_id = "3";
                selectedID = "3";
                ProtocolManager.instance.OnReceivedUserConnect(playerData);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (selectedID == "4")
            {
                PlayerData playerData = new PlayerData();
                playerData.conn_id = "4";
                ProtocolManager.instance.OnReceivedUserDisconnect(playerData);
                selectedID = "0";
            }
            else
            {
                PlayerData playerData = new PlayerData();
                playerData.conn_id = "4";
                selectedID = "4";
                ProtocolManager.instance.OnReceivedUserConnect(playerData);
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ProtocolManager.instance.OnReceivedControllerLeft_Press(selectedID);
        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            ProtocolManager.instance.OnReceivedControllerLeft_Release(selectedID);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ProtocolManager.instance.OnReceivedControllerRight_Press(selectedID);
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            ProtocolManager.instance.OnReceivedControllerRight_Release(selectedID);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ProtocolManager.instance.OnReceivedControllerUp_Press(selectedID);
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            ProtocolManager.instance.OnReceivedControllerUp_Release(selectedID);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ProtocolManager.instance.OnReceivedControllerDown_Press(selectedID);
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            ProtocolManager.instance.OnReceivedControllerDown_Release(selectedID);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            if (selectedID != "0")
            {
                ProtocolManager.instance.OnReceivedControllerFall_Press(selectedID);
                selectedID = "0";
            }
        }

        if (selectedID == "0") return;
    }
}