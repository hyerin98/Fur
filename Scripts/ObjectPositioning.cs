using UnityEngine;

public class ObjectPositioning : MonoBehaviour
{
    public Transform[] objects; // 배치할 오브젝트들
    public Camera mainCamera; // 메인 카메라
    public float baseDistance = 10f; // 기본 오브젝트 간의 거리
    public float distanceMultiplier = 1.5f; // 오브젝트 간 거리에 대한 배율

    void Start()
    {
        UpdateObjectPositions();
    }

    void Update()
    {
        // 화면 크기 또는 카메라 시야각이 변경되었을 때도 오브젝트들을 업데이트
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UpdateObjectPositions();
        }
    }

    void UpdateObjectPositions()
    {
        int objectCount = objects.Length;
        float distance = baseDistance + objectCount * distanceMultiplier; // 오브젝트 간의 적절한 거리 계산

        // 카메라의 시야각을 계산하여 화면 크기를 조정
        float aspectRatio = Screen.width / (float)Screen.height;
        float fov = Mathf.Atan(aspectRatio * Mathf.Tan(mainCamera.fieldOfView * Mathf.Deg2Rad)) * Mathf.Rad2Deg;

        // 오브젝트들을 카메라 주위에 원형으로 배치
        for (int i = 0; i < objectCount; i++)
        {
            float angle = i * Mathf.PI * 2f / objectCount;
            float x = Mathf.Sin(angle) * distance;
            float z = Mathf.Cos(angle) * distance;
            Vector3 objectPosition = new Vector3(x, 0f, z);
            objects[i].position = objectPosition;
        }
    }
}
