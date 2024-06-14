using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class CubeContoroller : MonoBehaviour
{
    [SerializeField]GameObject Cube;
    public int force = 5;
    // Start is called before the first frame update
    void Start()
    {
        Cube = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {   
        if (PoseReceiver_kou.landmarkPosition.Length > 0){
            Vector3 pos = PoseReceiver_kou.landmarkPosition[0];
            pos.x -= 0.5f;
            pos.y -= 0.5f;
            pos.z -= 0.5f;
            this.transform.position = new Vector3(pos.x * force,pos.y * force, pos.z * force);
        }


    }
}
