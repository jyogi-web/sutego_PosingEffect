using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class CubeContoroller : MonoBehaviour
{
    [SerializeField]GameObject Cube;
    // Start is called before the first frame update
    void Start()
    {
        Cube = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {   
        Vector3 pos = transform.position;   
        if(Input.GetButtonDown("Jump"))
        {
            pos.y ++;
            Cube.transform.position = new Vector3(pos.x, pos.y, pos.z);   
            Debug.Log("Jumpキーが押されました。");
        } 
        
        if(Math.Abs(Input.GetAxis("Horizontal")) > 0)
        {
            pos.x += Input.GetAxis("Horizontal");
            Cube.transform.position = new Vector3(pos.x, pos.y, pos.z);   
            Debug.Log("水平キーが押されました。");
        } 
        
        if(Math.Abs(Input.GetAxis("Vertical")) > 0)
        {
            pos.z += Input.GetAxis("Vertical");
            Cube.transform.position = new Vector3(pos.x, pos.y, pos.z);   
            Debug.Log("垂直キーが押されました。");
        } 


    }
}
