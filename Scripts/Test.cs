using Unity.VisualScripting;
using UnityEngine;

public class Test : MonoBehaviour
{
     public Material material; // 사용할 머티리얼
    public float speed = 0.5f; // 오프셋 변화 속도

    void Update()
    {
        float offset = Time.time * speed; // 시간에 따라 오프셋 계산
        material.SetTextureOffset("_MainTex", new Vector2(offset, 0)); // X축 오프셋 적용
    }
}
