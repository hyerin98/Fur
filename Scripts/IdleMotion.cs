using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class IdleMotion : MonoBehaviour
{
    public List<GameObject> idleFurs = new List<GameObject>(); // idle일때 움직이는 털 리스트 
    public Dictionary<GameObject, bool> furColorAssigned = new Dictionary<GameObject, bool>(); // fur의 색상 할당 상태를 추적
    private PlayerSelector playerSelector;

    void Start()
    {
        playerSelector = GetComponent<PlayerSelector>();
    }

    public void IdleMotionn()
    {
        StartCoroutine(AssignColorsWithDelay());
    }

    private IEnumerator AssignColorsWithDelay()
    {
        List<Color> redColors = new List<Color> { Color.red, new Color(1f, 0.5f, 0.5f), new Color(1f, 0.2f, 0.2f) };
        List<Color> blueColors = new List<Color> { Color.blue, new Color(0.5f, 0.5f, 1f), new Color(0.2f, 0.2f, 1f) };
        List<Color> yellowColors = new List<Color> { Color.yellow, new Color(0.5f, 0.5f, 1f), new Color(0.2f, 0.2f, 1f) };
        List<Color> greenColors = new List<Color> { Color.green, new Color(1f, 0.5f, 0.5f), new Color(1f, 0.2f, 0.2f) };
        

        // 다양한 컬러 리스트를 포함한 리스트
        List<List<Color>> allColorLists = new List<List<Color>> { redColors, blueColors, yellowColors, greenColors};
        
        // 랜덤으로 선택된 컬러 리스트를 사용
        List<Color> selectedColors = allColorLists[Random.Range(0, allColorLists.Count)];

        for (int i = 0; i < idleFurs.Count; i++)
        {
            GameObject idleFur = idleFurs[i];
            Renderer renderer_idle = idleFur.GetComponent<Renderer>();
            Light childLight_idle = idleFur.GetComponentInChildren<Light>();
            Player player = idleFur.GetComponent<Player>(); // idleFur에서 Player 컴포넌트를 가져옴

            if (childLight_idle != null)
            {
                childLight_idle.enabled = true;
            }

            if (renderer_idle != null)
            {
                Material material_idle = renderer_idle.material;
                Color initialColor_idle = material_idle.color; // 초기 색상 저장
                Color targetColor_idle = selectedColors[Random.Range(0, selectedColors.Count)];

                DOVirtual.Color(material_idle.color, targetColor_idle, 2f, value =>
                {
                    material_idle.color = value;
                });
                DOVirtual.Color(childLight_idle.color, targetColor_idle, 2f, value =>
                {
                    childLight_idle.color = value;
                    childLight_idle.intensity = 8f;
                    childLight_idle.range = 5f;
                });

                // 일정 시간이 지난 후 다시 초기 색상으로 되돌리는 코루틴을 시작
                StartCoroutine(RevertColorAfterDelay(material_idle, childLight_idle, initialColor_idle, 1f, 2f));
            }
            else
            {
                Debug.LogWarning("Renderer가 존재하지 않습니다: " + idleFur.name);
            }

            if (player != null)
            {
                if (player.CompareTag("Fur1"))
                {
                    player.ApplyForceToHingeJoints(transform.right);
                }
                else if (player.CompareTag("Fur2"))
                {
                    player.ApplyForceToHingeJoints(-transform.right);
                }
                else if (player.CompareTag("Fur3"))
                {
                    player.ApplyForceToHingeJoints(transform.right);
                }
                //player.PushHingeJoint("fur", "push", 20f); // Player 컴포넌트의 PushHingeJoint 함수 호출
            }
            else
            {
                Debug.LogWarning("Player 컴포넌트가 존재하지 않습니다: " + idleFur.name);
            }

            yield return new WaitForSeconds(0.05f); // 다음 fur로 넘어가기 전에 약간의 대기
        }
    }

    private IEnumerator RevertColorAfterDelay(Material material_idle, Light childLight_idle, Color initialColor_idle, float delay, float duration)
    {
        yield return new WaitForSeconds(delay);

        DOVirtual.Color(material_idle.color, initialColor_idle, duration, value =>
        {
            material_idle.color = value;
        });
        if (childLight_idle != null)
        {
            DOVirtual.Color(childLight_idle.color, initialColor_idle, duration, value =>
            {
                childLight_idle.color = value;
            });
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            IdleMotionn();
        }
    }
}
