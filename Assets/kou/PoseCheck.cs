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
        Kamehameha, //6
        Stroheim,   //7
        

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
        
        //デバッグ用
        Isside();
        Isfront();
        Israisearm();
        Ishandsoverlap();
        Ishandfront();
        Isupleftknee();
        IsUpRightKnee();
        // ここでポーズの位置に応じたステート変更を行う
        /*現場猫
        //右ひざが上がっている
        //正面を向いている
        //右左肘を曲げている*/
        if(Isfront()&&
        IsLeftElbowBent()&&
        IsRightElbowBent()&&
        IsUpRightKnee()
        )
        {
            ChangeState(PoseType.Genbaneko);
        }
        /*グリコ
        //左ひざが上がっている
        //両手が上がっている*/
        else if (Isupleftknee() &&
        Israisearm())
        {
            ChangeState(PoseType.Glico);
        }
        //きのきの
        else if (Pos[0].y > Pos[21].y && Pos[0].y > Pos[22].y)
        {
            ChangeState(PoseType.Kinokino);
        }
        /*波動拳
        横を向いている
        *両手を前に突き出している
        *手と手がある程度重なっている*/
        else if (Isside()&&
        Ishandfront()&&
        Ishandsoverlap())
        {
            ChangeState(PoseType.Hadouken);
        }
        /*かめはめ波
        正面を向いている
        *手と手がある程度重なっている*/
        else if (Isfront()&&
        Ishandsoverlap())
        {
            ChangeState(PoseType.Kamehameha);
        }
        /*シュトロハイム
        *両肘が鼻より上
        *手先が鼻より下
        *左ひざを突き出している
        */
        else if (Israisearm())
        {
            ChangeState(PoseType.Stroheim);
        }
        /*空条承太郎＆スタープラチナ
        *横向いている
        *手を前に突き出している
        */
        else if(Isside()){
            //ChangeState(PoseType.);
        }
        /*ゴールドエクスペリエンス
        *肩の中心と右手が近い
        *左手は腰
        */
        else if (false)
        {

        }
        /*ハイウェイスター
        *肩とひざの判定が近い
        *手がくっついている
        */
        else if(false)
        {

        }
        //Lispポーズ
        else if ((Math.Abs(Pos[0].y - Pos[13].y) <= 0.1 && Math.Abs(Pos[0].x - Pos[13].x) <= 0.1) || (Math.Abs(Pos[0].y - Pos[14].y) <= 0.1 && Math.Abs(Pos[0].x - Pos[14].x) <= 0.1))
        {
            ChangeState(PoseType.Lisp);
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
        if (Math.Abs(Pos[11].x - Pos[12].x) < 0.15)
        {
            Debug.Log("横を向いています");
            return true;
        }
        return false;
    }
    bool Isfront()
    {
        if (Math.Abs(Pos[11].x - Pos[12].x) > 0.15)
        {
            Debug.Log("正面を向いています");
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
            Debug.Log("両手が上がっています");
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
            Debug.Log("両手が重なっています");
            return true;
        }
        return false;
    }
    bool Isupleftknee()
    {
        //左ひざが右ひざより上
        if(Pos[25].y < Pos[26].y)
        {
            
            Debug.Log("左ひざが上がっています");
            return true;
        }
        return false;
    }
    bool IsUpRightKnee()
    {
        //右ひざが左ひざより上
        if(Pos[25].y > Pos[26].y)
        {
            Debug.Log("右ひざが上がっています");
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
    bool IsLeftElbowBent()
    {
        float thresholdAngle = 160f;
        
        Vector3 upperArm = Pos[13] - Pos[11];
        Vector3 forearm = Pos[15] - Pos[13];
        
        float angle = Vector3.Angle(upperArm, forearm);

        if(angle < thresholdAngle)
        {
            //Debug.Log("左ひじが曲がっています");
            return true;
        }
        return angle < thresholdAngle;
    }
    bool IsRightElbowBent()
    {
        float thresholdAngle = 160f;
        
        Vector3 upperArm = Pos[14] - Pos[12];
        Vector3 forearm = Pos[16] - Pos[14];
        Debug.Log("upperArm:" + upperArm);
        Debug.Log("forear:" + forearm);
        
        float angle = Vector3.Angle(upperArm, forearm);
        if(angle < thresholdAngle)
        {
            //Debug.Log("みぎひじが曲げまげ");
            return true;
        }
        return angle < thresholdAngle;
    }
}
