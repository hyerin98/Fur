using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IMFINE.Utils;
 
public class ColorManager : MonoSingleton<ColorManager> 
{
    [Header("ColorSettings")]
    public List<string> availableColors = new List<string>(); // 사용가능한 컬러리스트
    private HashSet<string> assignedColors = new HashSet<string>(); // 할당받은 컬러 해시셋
    private Dictionary<string, Player> players = new Dictionary<string, Player>();
    private int maxUsers;
    //private int userIndex; 
    private float minSaturation = 0.2f;
    private float maxSaturation = 0.8f;
    private float minBrightness = 0.6f;
    private float maxBrightness = 1.0f;

    
    void Start()
    {
        maxUsers = 50;
        availableColors = GenerateRandomColors(maxUsers);
        //userIndex = players.Count;
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
        string color = availableColors[0]; // 첫 번째 사용 가능한 컬러 담기
        availableColors.RemoveAt(0); // 할당되었으니까 사용가능한 컬러리스트에서 삭제
        assignedColors.Add(color); // 할당된 색상 해시셋에 추가합니다.
        //++userIndex; // 사용자 인덱스 증가
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
}