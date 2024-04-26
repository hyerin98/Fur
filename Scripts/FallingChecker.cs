using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingChecker : MonoBehaviour
{


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            Debug.Log("플레이어와 충돌");
        }
    }
}