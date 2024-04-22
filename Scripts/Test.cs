using Unity.VisualScripting;
using UnityEngine;

public class Test : MonoBehaviour
{
    public float moveStep = 1.0f; // 한 번 키를 누를 때마다 이동할 거리

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            // 현재 위치에서 오른쪽 방향(양의 x축)으로 moveStep 만큼 이동
            Vector3 newPosition = rb.position + new Vector3(moveStep, 0, 0);
            rb.MovePosition(newPosition);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // 현재 위치에서 오른쪽 방향(양의 x축)으로 moveStep 만큼 이동
            Vector3 newPosition = rb.position + new Vector3(-moveStep, 0, 0);
            rb.MovePosition(newPosition);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            // 현재 위치에서 오른쪽 방향(양의 x축)으로 moveStep 만큼 이동
            Vector3 newPosition = rb.position + new Vector3(0, moveStep, 0);
            rb.MovePosition(newPosition);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            // 현재 위치에서 오른쪽 방향(양의 x축)으로 moveStep 만큼 이동
            Vector3 newPosition = rb.position + new Vector3(0, -moveStep, 0);
            rb.MovePosition(newPosition);
        }
    }
}
