using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoseCheck : MonoBehaviour
{
    Vector3[] Pos;
    public enum PoseType
    {
        None,
        Lisp,
        Kinokino,
        Hadouken,
        Genbaneko,
        Glico,
        Jojo,
        Kamehameha
    }

    public PoseType currentState;
    [SerializeField] Image poseImage; // イラストを表示するImageコンポーネント
    [SerializeField] List<Sprite> poseSprites; // スプライトを格納するリスト
    [SerializeField] Text checktext;

    // Start is called before the first frame update
    void Start()
    {
        Pos = PoseReceiver_kou.landmarkPosition;
        currentState = PoseType.None;  // 初期ステートを設定
        UpdatePoseImage();
    }

    // Update is called once per frame
    void Update()
    {
        Pos = PoseReceiver_kou.landmarkPosition;  // 毎フレーム位置を更新
        
        // ここでポーズの位置に応じたステート変更を行う
        //0:Lisp
        if (Math.Abs(Pos[0].y - Pos[13].y) <= 0.1 || Math.Abs(Pos[0].x - Pos[13].x) <= 0.1)
        {
            ChangeState(PoseType.Lisp);
        }
        //1:きのきの
        else if (Pos[0].y < Pos[21].y && Pos[0].y < Pos[22].y)
        {
            ChangeState(PoseType.Kinokino);
        }
        //2:波動拳
        else if (Pos[12].x > Pos[11].x)
        {
            ChangeState(PoseType.Hadouken);
        }//この下にあたらしいぽーずを追加
        //4:現場猫
        //右膝を挙げている・右手を横に向けている・左腕は横
        else if (Pos[26].y > Pos[25].y && Math.Abs(Pos[12].z - Pos[16].z) <= 0.2 && Math.Abs(Pos[11].z - Pos[15].z) <= 0.2)
        {
            ChangeState(PoseType.Genbaneko);
        }//5:グリコ
        //左膝が上がっている・右手左手が上がっている
        else if (Pos[26].y > Pos[25].y && Pos[16].y > Pos[12].y && Pos[15].y > Pos[11].y)
        {
            ChangeState(PoseType.Glico);
        }
        else
        {
            ChangeState(PoseType.None);
        }
        // 他のポーズ条件を追加
        // 例：
        // else if (別の条件)
        // {
        //     ChangeState(PoseType.別のポーズ);
        // }
    }

    void ChangeState(PoseType newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
            Debug.Log("ステートが " + newState + " に変更されました");
            //画像更新
            UpdatePoseImage();
            // ステート変更時の処理をここに追加
            checktext.text = newState.ToString();

        }
    }

    void UpdatePoseImage()
    {
        poseImage.sprite = poseSprites[(int)currentState];
    }
}
