// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using DG.Tweening;
// using IMFINE.Utils;
// using IMFINE.Utils.ConfigManager;
// using IMFINE.Utils.JoyStream.Communicator;

// public class TestColorAssignment : MonoBehaviour
// {
//     private ColorManager colorManager;
//     public GameObject playerPrefab; // 플레이어 프리팹

//     private void Awake()
//     {
//         colorManager = FindObjectOfType<ColorManager>();
//     }


//     void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.Return))
//         {
//             SpawnPlayer();
//         }

//     }



//     private void SpawnPlayer() // 스폰하는거
//     {
//         ColorManager colorManager = ColorManager.instance;
//         string playerColor = colorManager.AssignUserColor();
//         int userIndex = colorManager.GetUserIndex(); // ColorManager에서 사용자의 인덱스 가져오기

//         if (playerColor != null && userIndex >= 0)
//         {
//             GameObject newPlayer = Instantiate(playerPrefab, transform.position, Quaternion.identity); // 플레이어를 생성
//             newPlayer.transform.DOScale(1.2f, 1.2f); // 생성 효과 추가 (수정필)

//             Player player = newPlayer.GetComponent<Player>();
            
//             // 플레이어의 색상을 playerassignedColors 리스트에 추가
//             player.playerassignedColors.Add(playerColor);
//             Debug.Log("유저번호: " + userIndex + " 이 가지고 있는 컬러: " + playerColor);

//             //player.AssignUserColor();
//             player.SetPlayerColor(playerColor); // 플레이어의 색상 설정
//             player.SetUserIndex(userIndex); // 플레이어의 사용자 인덱스 설정

//             // 플레이어의 색상을 할당한 후에 플레이어의 리스트에 추가
//             colorManager.AddPlayer(player);

//             // 생성된 플레이어 색 바꾸기
//             Renderer renderer = newPlayer.GetComponent<Renderer>();
//             if (renderer != null)
//             {
//                 Material material = renderer.material; // 기존 머터리얼 가져오기
//                 Color color;
//                 if (ColorUtility.TryParseHtmlString("#" + playerColor, out color))
//                 {
//                     renderer.material.color = color;
//                 }
//                 else
//                 {
//                     Debug.LogError("컬러 생성 안댐: " + playerColor);
//                 }
//             }
//             else
//             {
//                 Debug.LogError("랜더러 컴포넌트를 찾을 수 없음");
//             }
//         }
//         else
//         {
//             Debug.Log("더 이상 할당할 컬러가 없습니다.");
//         }
//     }
// }
