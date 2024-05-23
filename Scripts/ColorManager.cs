using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IMFINE.Utils;

public class ColorManager : MonoSingleton<ColorManager>
{
    [Header("ColorSettings")]
    public List<string> availableColors = new List<string>(); // 사용 가능한 컬러 리스트
    private HashSet<string> assignedColors = new HashSet<string>(); // 할당 받은 컬러 해시셋
    private Dictionary<string, Player> players = new Dictionary<string, Player>();
    private int maxUsers;
    public int idleColorCount;
    private float minSaturation = 0.5f; 
    private float maxSaturation = 1.0f;
    private float minBrightness = 0.5f; 
    private float maxBrightness = 0.9f; 

    [Header("idleColorSettings")]
    public List<string> idleColors = new List<string> ();

    void Start()
    {
        maxUsers = 50;
        availableColors = GenerateRandomColors(maxUsers);

        // idle모션일 떄 할당할 컬러수
        idleColorCount = 118;
        idleColors = GenerateRandomColors(idleColorCount);
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
        Color color = UnityEngine.Random.ColorHSV(0f, 1f, minSaturation, maxSaturation, minBrightness, maxBrightness);
        return ColorUtility.ToHtmlStringRGB(color);
    }

    public string AssignUserColor()
    {
        if (availableColors.Count > 0)
        {
            string color = availableColors[0]; // 첫 번째 사용 가능한 컬러 담기
            availableColors.RemoveAt(0); // 할당되었으니까 사용 가능한 컬러 리스트에서 삭제
            assignedColors.Add(color); // 할당된 색상 해시셋에 추가
            return color;
        }
        return null; // 사용 가능한 색상이 없을 경우 null 반환
    }

    public string IdleColor()
    {
        if(idleColors.Count > 0)
        {
            string color = idleColors[0];
            idleColors.RemoveAt(0);
            idleColors.Add(color);
            return color;
        }
        return null;
    }

    public void IdleReturnColor(string color)
    {
        if(idleColors.Contains(color))
        {
            idleColors.Remove(color);
            idleColors.Add(color);
        }
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
