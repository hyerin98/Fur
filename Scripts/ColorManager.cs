using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using IMFINE.Utils;
using IMFINE.Utils.ConfigManager;
using IMFINE.Utils.JoyStream.Communicator; 

 
public class ColorManager : MonoSingleton<ColorManager> 
{
    public List<string> availableColors = new List<string>();
    //private List<string> assignedColors;
    private HashSet<string> assignedColors = new HashSet<string>();
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
        maxUsers = 50;
        availableColors = GenerateRandomColors(maxUsers);
        //assignedColors = new List<string>(); 
        userIndex = players.Count;
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
    if (availableColors.Count > 0)
    {
        string color = availableColors[0]; // 첫 번째 사용 가능한 색상을 가져옵니다.
        availableColors.RemoveAt(0); // 할당되었으니 사용 가능한 색상 리스트에서 삭제합니다.
        assignedColors.Add(color); // 할당된 색상 해시셋에 추가합니다.
        ++userIndex; // 사용자 인덱스 증가
        return color;
    }
    return null; // 사용 가능한 색상이 없을 경우 null 반환
}


    public bool IsColorAvailable()
    {
        return assignedColors.Count < availableColors.Count;
    }
public void ReturnColor(string color)
{
    if (assignedColors.Contains(color))
    {
        assignedColors.Remove(color); // 할당된 색상 해시셋에서 제거
        availableColors.Add(color); // 다시 사용 가능한 색상 리스트에 추가
    }
}


private void RemoveUser(string playerID)
{
    if (players.ContainsKey(playerID))
    {
        Player player = players[playerID];
        ColorManager.instance.ReturnColor(player.playerColor); // 색상 반환
        players.Remove(playerID); // 플레이어 제거
        // 여기에 기타 정리 로직 추가 (예: 객체 파괴, 이벤트 해제 등)
    }
}

// public void RemoveUserAtIndex(int index)
// {
//     if (index < 0 || index >= assignedColors.Count)
//     {
//         Debug.LogWarning("유효하지 않은 인덱스입니다: " + index);
//         return;
//     }

//     string userColor = assignedColors[index];
//     assignedColors.RemoveAt(index);
//     availableColors.Add(userColor); // 사용자가 삭제될 때 해당 컬러를 다시 사용 능한 컬러리스트에 추가
//     Debug.Log("유저 " + index + "가 삭제되었습니다. 할당된 컬러: " + userColor);

//     // 딕셔너리에서 해당 플레이어를 제거
//     foreach (var playerEntry in players) 
//     {
//         if (playerEntry.Value.playerColor == userColor)
//         {
//             players.Remove(playerEntry.Key);
//             break;
//         }
//     }
//     userIndex--;
// }

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