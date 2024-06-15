using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoseCheck_rita : MonoBehaviour
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
        Pos = PoseReceiver.landmarkPosition;
        currentState = PoseType.Lisp;  // 初期ステートを設定
        UpdatePoseImage();
    }

    // Update is called once per frame
    void Update()
    {
        Pos = PoseReceiver.landmarkPosition;  // 毎フレーム位置を更新
        
        // ここでポーズの位置に応じたステート変更を行う
        //Lispポーズ
        if (Math.Abs(Pos[0].y - Pos[13].y) <= 0.1 && Math.Abs(Pos[0].x - Pos[13].x) <= 0.2
            || Math.Abs(Pos[0].y - Pos[14].y) <= 0.1&& Math.Abs(Pos[0].x - Pos[14].x) <= 0.2)
        {
            ChangeState(PoseType.Lisp);
        }
        //きのきの
        else if (Pos[0].y > Pos[19].y && Pos[0].y > Pos[20].y)
        {
            ChangeState(PoseType.Kinokino);
        }
        //ジョジョ
        else if (Pos[12].x > Pos[11].x)
        {
            ChangeState(PoseType.Jojo);
        }//この下にあたらしいぽーずを追加
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
