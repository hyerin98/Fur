using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurManager : MonoBehaviour
{
    public static FurManager instance;


    public GameObject playerPrefab;


    private List<Color> usedColors = new List<Color>();

    void Start()
    {
        // 싱글톤 인스턴스 초기화
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 새로운 플레이어를 생성하는 메서드
    public void CreatePlayer()
    {
        // 플레이어에 무작위 색상 생성
        Color randomColor = GetRandomColor();

        // 플레이어 프리팹을 인스턴스화
        GameObject newPlayer = Instantiate(playerPrefab, RandomPosition(), Quaternion.identity);

        // 플레이어 재료 색상 설정
        Renderer renderer = newPlayer.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = randomColor;
        }


        // Player playerScript = newPlayer.GetComponent<Player>();
        // if (playerScript != null)
        // {
        //     // 플레이어 데이터 생성 및 설정
        //     PlayerData playerData = new PlayerData(System.Guid.NewGuid().ToString(), usedColors.Count + 1);
        //     playerScript.playerData = playerData;
        // }

        // 사용된 색상을 목록에 추가
        usedColors.Add(randomColor);
    }

    // 아직 사용되지 않은 무작위 색상을 가져오는 메서드
    private Color GetRandomColor()
    {
        // 무작위 색상 생성
        Color randomColor;
        do
        {
            randomColor = new Color(Random.value, Random.value, Random.value);
        } while (usedColors.Contains(randomColor));
        return randomColor;
    }

    // 플레이어 스폰을 위한 무작위 위치를 가져오는 메서드
    private Vector3 RandomPosition()
    {
        return new Vector3(Random.Range(-10f, 10f), Random.Range(-5f, 5f), Random.Range(-10f, 10f));
    }
}