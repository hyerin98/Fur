using System.IO;
using UnityEngine;

[System.Serializable]
public class Config
{
    public int count;
    public int rows;
    public int columns;
}

public class ObjectSpawner  : MonoBehaviour
{
    public GameObject[] objectsToPlace; // 배치하고자 하는 오브젝트들의 배열
    private Config config;

    void Start()
    {
        LoadConfig();
        PlaceObjects();
    }

    void LoadConfig()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "config.json");
        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            config = JsonUtility.FromJson<Config>(dataAsJson);
        }
        else
        {
            Debug.LogError("Cannot find config file!");
        }
    }

void PlaceObjects()
{
    if (config == null)
    {
        Debug.LogError("Config is not loaded!");
        return;
    }

    int count = config.count;
    int rows = config.rows;
    int columns = config.columns;

    float screenWidth = Screen.width;
    float screenHeight = Screen.height;

    // 카메라의 월드 좌표
    Vector3 cameraWorldPosition = Camera.main.transform.position;

    // 화면 비율 계산
    float aspectRatio = screenWidth / screenHeight;
    float targetAspectRatio = (float)columns / rows;

    float xSpacing;
    float ySpacing;

    if (aspectRatio > targetAspectRatio)
    {
        // 화면이 가로로 더 길 경우
        ySpacing = screenHeight / (rows + 1);
        xSpacing = ySpacing * targetAspectRatio;
    }
    else
    {
        // 화면이 세로로 더 길거나 비율이 같은 경우
        xSpacing = screenWidth / (columns + 1);
        ySpacing = xSpacing / targetAspectRatio;
    }

    // 오브젝트 위치 조정
    int objectIndex = 0;
    for (int row = 0; row < rows; row++)
    {
        for (int col = 0; col < columns; col++)
        {
            if (objectIndex < objectsToPlace.Length)
            {
                GameObject obj = objectsToPlace[objectIndex];
                if (obj != null)
                {
                    // 카메라의 월드 좌표를 고려하여 오브젝트의 로컬 좌표 설정
                    float xPosition = (col + 1) * xSpacing - screenWidth / 2f + cameraWorldPosition.x;
                    float yPosition = (row + 1) * ySpacing - screenHeight / 2f + cameraWorldPosition.y;

                    obj.transform.localPosition = new Vector3(xPosition, yPosition, obj.transform.localPosition.z);
                }
                objectIndex++;
            }
            else
            {
                return; // 모든 오브젝트를 배치했으면 종료
            }
        }
    }
}





}
