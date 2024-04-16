using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using IMFINE.Utils;
using IMFINE.Utils.ConfigManager;
using IMFINE.Utils.JoyStream.Communicator;

public class PlayerMovement : MonoBehaviour
{
    private Animator animator;

    public float fallSpeed = 10f;
    public float fallDistance = 10f;
    private bool isFalling = false;
    //private bool isFalled = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // float hAxis = Input.GetAxisRaw("Horizontal");
        // float vAxis = Input.GetAxisRaw("Vertical");

        // Vector3 moveVec = new Vector3(hAxis, 0, vAxis).normalized;
        // transform.position += moveVec * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            animator.SetBool("Right", true);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isFalling = true;
        }

        if (isFalling)
            transform.Translate(Vector3.back * fallSpeed * Time.deltaTime);
        if (transform.position.z <= -fallDistance)
        {
            isFalling = false;
        }
    }


    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground") && isFalling)
        {
            isFalling = false;
            //isFalled = true;
            Debug.Log("땅에 닿았다");
        }
    }

    // void Falled()
    // {
    //     if(isFalled)
    //     {
    //         this.gameObject.SetActive(false);
    //     }
    // }
}