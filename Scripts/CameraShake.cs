using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float ShakeAmount = 0.05f; // 흔들림 강도
    float ShakeTime;
    Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.position;
    }

    public void VibrateForTime(float time)
    {
        ShakeTime = time;
    }

    private void Update()
    {
        if (ShakeTime > 0)
        {
            transform.position = Random.insideUnitSphere * ShakeAmount + initialPosition;
            ShakeTime -= Time.deltaTime;
        }
        else
        {
            ShakeTime = 0.0f;
            transform.position = initialPosition;
        }
    }
}
