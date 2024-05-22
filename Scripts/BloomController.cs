// using UnityEngine;
// using UnityEngine.Rendering.PostProcessing;

// public class BloomController : MonoBehaviour
// {
//     public PostProcessVolume postProcessVolume; // 포스트 프로세싱 볼륨
//     private Bloom bloomLayer; // Bloom 레이어

//     public float scatterChangeRate = 1f; // Scatter 값 변경 속도
//     public KeyCode increaseKey = KeyCode.I; // 증가 키
//     public KeyCode decreaseKey = KeyCode.D; // 감소 키

//     void Start()
//     {
//         // PostProcessVolume 컴포넌트로부터 Bloom 레이어 가져오기
//         postProcessVolume.profile.TryGetSettings(out bloomLayer);
//     }

//     void Update()
//     {
//         // 증가 키를 누르면 scatter 값을 증가시킴
//         if (Input.GetKeyDown(increaseKey))
//         {
//             AdjustScatter(scatterChangeRate);
//         }

//         // 감소 키를 누르면 scatter 값을 감소시킴
//         if (Input.GetKeyDown(decreaseKey))
//         {
//             AdjustScatter(-scatterChangeRate);
//         }
//     }

//     // Scatter 값을 조절하는 함수
//     void AdjustScatter(float amount)
//     {
//         // 현재 scatter 값을 가져옴
//         Vector2 scatter = bloomLayer.scatter.value;
        
//         // scatter 값을 변경
//         scatter += new Vector2(amount, amount);
        
//         // scatter 값을 설정 (0 이상으로 보정)
//         scatter = new Vector2(Mathf.Max(scatter.x, 0f), Mathf.Max(scatter.y, 0f));
//         bloomLayer.scatter.value = scatter;
//     }
// }
