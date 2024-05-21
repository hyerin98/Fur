using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class test : MonoBehaviour
{
    public GameObject[] obj;
    public Vector3[] targetPositions;
    public Ease ease;

    void Start()
    {
        if (targetPositions.Length != obj.Length)
        {
            Debug.LogError("The number of target positions must match the number of objects.");
            return;
        }

        for (int i = 0; i < obj.Length; i++)
        {
            if (obj[i] != null)
            {
                obj[i].transform.DOMove(targetPositions[i], 3.0f).SetEase(ease); 
            }
        }
    }
}
