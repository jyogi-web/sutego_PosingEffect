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
        None,       //0
        Lisp,       //1
        Kinokino,   //2
        Hadouken,   //3
        Genbaneko,  //4
        Glico,      //5
        Kamehameha  //6
    }

    public PoseType currentState;
    [SerializeField] Image poseImage; // イラストを表示するImageコンポーネント
    [SerializeField] List<Sprite> poseSprites; // スプライトを格納するリスト
    [SerializeField] Text checktext;

    // Start is called before the first frame update
    void Start()
    {
        Pos = PoseReceiver_kou.landmarkPosition;
        currentState = PoseType.Lisp;  // 初期ステートを設定
        UpdatePoseImage();
    }

    // Update is called once per frame
    void Update()
    {
        Pos = PoseReceiver_kou.landmarkPosition;  // 毎フレーム位置を更新
        
        //デバッグ用
        //Isside();
        //Isfront();
        //Israisearm();
        Ishandsoverlap();
        Ishandfront();
        // ここでポーズの位置に応じたステート変更を行う
        //Listポーズ
        if (Math.Abs(Pos[0].y - Pos[13].y) <= 0.2 || Math.Abs(Pos[0].x - Pos[13].x) <= 0.2)
        {
            //ChangeState(PoseType.Lisp);
        }
        //きのきの
        else if (Pos[0].y > Pos[21].y && Pos[0].y > Pos[22].y)
        {
            ChangeState(PoseType.Kinokino);
        }
        //波動拳
        //横を向いている
        //両手を前に突き出している
        //手と手がある程度重なっている
        else if (Isside()&&
        Ishandfront()&&
        Ishandsoverlap())
        {
            ChangeState(PoseType.Hadouken);
        }
        //グリコ
        //左ひざが上がっている
        //両手が上がっている
        else if (Isupleftknee() &&
        Israisearm())
        {
            ChangeState(PoseType.Glico);
        }
        //かめはめ波
        //正面を向いている
        //両手を前に突き出している
        //手と手がある程度重なっている
        else if (Isfront()&&
        Ishandfront() && 
        Ishandsoverlap())
        {
            ChangeState(PoseType.Kamehameha);
        }
        //スタープラチナ
        /*else if (Pos[12].x > Pos[11].x)
        {
            ChangeState(PoseType.Jojo);
        }*/
        //
        else if(){

        }
        else
        {
            ChangeState(PoseType.None);
        }
        // 他のポーズ条件を追加
        // 例：
        // if (別の条件)
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

    bool Isside()
    {
        if (Math.Abs(Pos[11].x - Pos[12].x) <= 0.2)
        {
            //Debug.Log("横を向いています");
            return true;
        }
        return false;
    }
    bool Isfront()
    {
        if (Math.Abs(Pos[11].x - Pos[12].x) >= 0.2)
        {
            //Debug.Log("正面を向いています");
            return true;
        }
        return false;
    }
    bool Israisearm()
    {
        //肘が鼻より高い(y座標は下が正)
        if(Pos[0].y > Pos[14].y &&
        Pos[0].y > Pos[13].y)
        {
            //Debug.Log("両手が上がっています");
            return true;
        }
        return false;
    }
    bool Ishandsoverlap()
    {
        //両手のxy座標が近い
        if(Math.Abs(Pos[15].y - Pos[16].y) < 0.1 &&
        Math.Abs(Pos[15].x - Pos[16].x) < 0.1)
        {
            //Debug.Log("両手が重なっています");
            return true;
        }
        return false;
    }
    bool Isupleftknee()
    {
        if((Pos[25].y - Pos[26].y) > 0.2)
        {
            Debug.Log("左ひざが上がっています");
            return true;
        }
        return false;
    }
    
    bool Ishandfront()
    {
        if(Math.Abs(Pos[12].y - Pos[16].y) < 0.2 &&
        Math.Abs(Pos[11].y - Pos[15].y) < 0.2)
        {
            Debug.Log("両手をまっすぐ突き出しています");
            return true;
        }
        return false;
    }
}
