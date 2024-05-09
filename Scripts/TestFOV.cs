using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFOV : MonoBehaviour
{
    public Camera myCam = null;

    public bool maintainWidth = true;

    [Range(-1, 1)]

    public int adaptPosition;

    float defaultWidth;

    float defaultHeight;

    Vector3 CameraPos;

    private void Awake()

    {

        myCam = this.gameObject.GetComponent<Camera>();

    }

    private void Start()

    {

        CameraPos = myCam.transform.position;

        defaultHeight = myCam.fieldOfView;

        defaultWidth = myCam.fieldOfView * myCam.aspect;

    }

    // Update is called once per frame
    void Update()
    {
        if (maintainWidth)
        {



            myCam.fieldOfView = defaultWidth / myCam.aspect;

            myCam.transform.position = new Vector3(CameraPos.x, CameraPos.y + adaptPosition * (defaultHeight - myCam.fieldOfView), CameraPos.z);

// 대신, 해당 해상도에서의 FOV값(ex>60) / 아래 공식을 통해 결정된값을 defaultHeight/Width 고정값으로 넣어주시면 끝.



        }
        else

        {

            myCam.transform.position = new Vector3(adaptPosition * adaptPosition * (defaultWidth - myCam.fieldOfView * myCam.aspect), CameraPos.y, CameraPos.z);



        }
    }
}
