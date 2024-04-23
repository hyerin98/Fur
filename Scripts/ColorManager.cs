using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using IMFINE.Utils;
using IMFINE.Utils.ConfigManager;
using IMFINE.Utils.JoyStream.Communicator; 

 
public class ColorManager : MonoSingleton<ColorManager> 
{
    public List<string> availableColors;
    private List<string> assignedColors;
    public Dictionary<string, Player> players = new Dictionary<string, Player>();
    public int maxUsers;
    public int userIndex; 

    // 색 할당
    private float minSaturation = 0.2f;
    private float maxSaturation = 0.8f;
    private float minBrightness = 0.6f;
    private float maxBrightness = 1.0f;

    
    void Start()
    {
        maxUsers = 10;
        availableColors = GenerateRandomColors(maxUsers);
        assignedColors = new List<string>(); 
    }

     public void AddPlayer(Player player)
    {
         players.Add(player.playerID, player); 
    }


    List<string> GenerateRandomColors(int count)
    {
        List<string> randomColors = new List<string>();
        for (int i = 0; i < count; i++)
        {
                string color = GenerateRandomColor();
                randomColors.Add(color);
        }
        return randomColors;
    }

    string GenerateRandomColor()
    {
        // 임의의 색상을 생성하여 반환
        //Color color = Random.ColorHSV();
        //return ColorUtility.ToHtmlStringRGB(color);
        Color color;
        do
        {
            // 랜덤한 HSV 범위에서 색상을 생성합니다.
            color = Random.ColorHSV(0f, 1f, minSaturation, maxSaturation, minBrightness, maxBrightness);
        } while (IsColorTooDark(color)); // 생성된 색상이 너무 어두운지 확인

        return ColorUtility.ToHtmlStringRGB(color);
    
    }

     bool IsColorTooDark(Color color)
    {
        // 색상의 밝기를 계산
        float brightness = color.r * 0.299f + color.g * 0.587f + color.b * 0.114f;
        // 만약 밝기가 너무 낮다면 어두운 색상으로 판단
        return brightness < 0.5f;
    }

    
    public string AssignUserColor() 
{
    // 현재 할당된 플레이어 수가 최대 플레이어 수보다 작은 경우에만 새로운 컬러를 할당
    if (userIndex <= maxUsers)
    {
        string userColor = null;

        if (userIndex < assignedColors.Count)
        {
            // 이미 할당된 컬러가 있으면 재사용
            userColor = assignedColors[userIndex];
        }
        else
        {
            if (availableColors.Count > 0)
            {
                userColor = availableColors[0];
                assignedColors.Add(userColor);
                availableColors.RemoveAt(0); // 0번째 인덱스 삭제
            }
            else
            {
                Debug.Log("더 이상 할당받을 컬러가 없당!!!!");
                return null;
            }
        }

        userIndex++;
        return userColor;
    }
    else
    {
        Debug.Log("현재 최대 " + maxUsers + "명 이므로 새로운 컬러 할당할 수 없음");
        return null;
    }
}


public void RemoveUserAtIndex(int index)
{
    if (index < 0 || index >= assignedColors.Count)
    {
        Debug.LogWarning("유효하지 않은 인덱스입니다: " + index);
        return;
    }

    string userColor = assignedColors[index];
    assignedColors.RemoveAt(index);
    availableColors.Add(userColor); // 사용자가 삭제될 때 해당 컬러를 다시 사용 능한 컬러리스트에 추가
    Debug.Log("유저 " + index + "가 삭제되었습니다. 할당된 컬러: " + userColor);

    // 딕셔너리에서 해당 플레이어를 제거
    foreach (var playerEntry in players) 
    {
        if (playerEntry.Value.playerColor == userColor)
        {
            players.Remove(playerEntry.Key);
            break;
        }
    }
    userIndex--;
}

    // public string GetAssignedColorAtIndex(int index)
    // {
    //     if (index < assignedColors.Count)
    //     {
    //         return assignedColors[index];
    //     }
    //     else
    //     {
    //         return null;
    //     }
    // }

    public int GetUserIndex()
    {
        return userIndex;
    }
}