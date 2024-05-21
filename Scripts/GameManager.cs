using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Camera MainCamera;

    public GameObject furObject;



    private void Awake()
    {
        Instance = this;

    }

    public void StartGame()
    {
        MakeFur();
    }

    private void MakeFur()
    {
        Vector3 makePos =MainCamera.ScreenToWorldPoint(new Vector3((Screen.width/2)+(Random.Range(-3,1)), (Screen.height/2)+ Random.Range(-2, 2), 17.5f));
        GameObject tempBall = Instantiate(furObject, makePos,Quaternion.Euler(180,180,180));    
    }
}
