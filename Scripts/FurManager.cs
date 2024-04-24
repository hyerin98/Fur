using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using IMFINE.Utils;
using IMFINE.Utils.ConfigManager;
using IMFINE.Utils.JoyStream.Communicator;
using UnityEngine.UIElements;

public class FurManager : MonoSingleton<FurManager>
{
    public List<GameObject> furs = new List<GameObject>();
    public List<Vector3> furPositions = new List<Vector3>(); // 오브젝트 위치를 저장하는 리스트
    PlayerSelector playerSelector;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            DestroyOneFur();
        }

        playerSelector = GetComponent<PlayerSelector>();
    }

    void DestroyOneFur()
    {
        if (furs.Count > 0)
        {
            // 리스트에서 첫 번째 오브젝트를 찾아서 삭제
            GameObject furToDestroy = furs[0];
            furs.RemoveAt(0);
            Vector3 positionOfDestroyed = furToDestroy.transform.position; // 삭제될 오브젝트의 위치를 저장
            Destroy(furToDestroy);

            // 일정 시간 후에 오브젝트를 다시 생성
            StartCoroutine(RespawnFur(positionOfDestroyed));
        }
    }

    void FurCAssignedColor()
    {   
        
    }

    IEnumerator RespawnFur(Vector3 position)
    {
        // 여기서 3초 기다립니다. 필요에 따라 시간을 조절하세요.
        yield return new WaitForSeconds(3.0f);

        // 다시 오브젝트를 생성합니다. `furPrefab`은 인스펙터에서 할당받은 프리팹을 가정합니다.
        GameObject newFur = Instantiate(furPrefab, position, Quaternion.identity);
        furs.Add(newFur); // 생성된 오브젝트를 리스트에 추가
    }

    // 프리팹을 위한 참조입니다.
    public GameObject furPrefab;
    
}