using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
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
        Hadouken_l,
        Hadouken_r,
        Glico,
        Kamehameha,
        Stroheim,
    }

    bool stateEnter = true;
    public PoseType currentState;
    public PoseType newState;

    //test用
    [SerializeField] Text checktext;

    AudioSource audiosource;

    /*各ポーズエフェクト*/
    //きのきの
    [SerializeField] GameObject kinokino_img;
    RectTransform kinokino_rect;
    //Lisp
    [SerializeField] GameObject lisp_img;
    RectTransform lisp_rect;
    //波動拳L
    [SerializeField] GameObject hadoukenL_anime;
    RectTransform hadoukenL_rect;
    [SerializeField]AudioClip hadouken_se;
    //波動拳R
    [SerializeField] GameObject hadoukenR_anime;
    RectTransform hadoukenR_rect;
    //グリコ
    [SerializeField] GameObject glico_img;
    RectTransform glico_rect;

    //座標調整用
    float force = 5;

    // Start is called before the first frame update
    void Start()
    {
        Pos = PoseReceiver.landmarkPosition;
        currentState = PoseType.None;  // 初期ステートを設定

        audiosource = GetComponent<AudioSource>();

        //エフェクトのRectTransform取得
        kinokino_rect = kinokino_img.GetComponent<RectTransform>();
        lisp_rect=lisp_img.GetComponent<RectTransform>();
        hadoukenL_rect=hadoukenL_anime.GetComponent<RectTransform>();
        hadoukenR_rect = hadoukenR_anime.GetComponent<RectTransform>();
        glico_rect =glico_img.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        Pos = PoseReceiver.landmarkPosition;  // 毎フレーム位置を更新
        
        // ここでポーズの位置に応じたステート変更を行う

        /*グリコ
        //左ひざが上がっている
        //両手が上がっている*/
        if (Isupleftknee() &&
        Israisearm())
        {
            ChangeState(PoseType.Glico);
        }
        //きのきの
        else if (Pos[0].y > Pos[19].y && Pos[0].y > Pos[20].y)
        {
            ChangeState(PoseType.Kinokino);
        }
        /*波動拳L
        横を向いている
        *両手を前に突き出している
        *手と手がある程度重なっている*/
        else if (Isside() &&
        Ishandfront() &&
        Ishandsoverlap() && Pos[11].x > Pos[15].x)
        {
            ChangeState(PoseType.Hadouken_l);
        }
        /*波動拳R
        横を向いている
        *両手を前に突き出している
        *手と手がある程度重なっている*/
        else if (Isside() &&
        Ishandfront() &&
        Ishandsoverlap() && Pos[12].x < Pos[16].x)
        {
            ChangeState(PoseType.Hadouken_r);
        }
        /*かめはめ波
        正面を向いている
        *手と手がある程度重なっている*/
        else if (Isfront() &&
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
        //Lispポーズ
        else if (Math.Abs(Pos[0].y - Pos[13].y) <= 0.1 && Math.Abs(Pos[0].x - Pos[13].x) <= 0.2
            || Math.Abs(Pos[0].y - Pos[14].y) <= 0.1 && Math.Abs(Pos[0].x - Pos[14].x) <= 0.2)
        {
            ChangeState(PoseType.Lisp);
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

        //ステートによってswitch文で処理
        switch (currentState)
        {
            //None
            #region
            case PoseType.None:
                //一度だけやる処理
                if (stateEnter)
                {
                    stateEnter = false;
                    Debug.Log("pose:None");
                }
                //ポーズが変わったら
                if (currentState != newState)
                {
                    stateEnter = true;
                }
                //ポーズがそのままならポーズの処理
                else
                {

                }
                break;
            #endregion
            //Lisp
            #region
            case PoseType.Lisp:
                //一度だけやる処理
                if (stateEnter)
                {
                    lisp_img.SetActive(true);
                    stateEnter = false;
                    Debug.Log("pose:Lisp");
                }
                //ポーズが変わったら
                if (currentState != newState)
                {
                    stateEnter = true;
                    lisp_img.SetActive(false);
                }
                //ポーズがそのままならポーズの処理
                else
                {
                    ImageTrack(12, lisp_rect);
                }
                break;
            #endregion
            //kinokino
            #region
            case PoseType.Kinokino:
                //一度だけやる処理
                if (stateEnter)
                {
                    stateEnter = false;
                    Debug.Log("pose:kinokino");
                    kinokino_img.SetActive(true);

                }
                //ポーズが変わったら
                if (currentState != newState)
                {
                    kinokino_img.SetActive(false);
                    stateEnter = true;
                }
                //ポーズがそのままならポーズの処理
                else
                {
                    ImageTrack(0, kinokino_rect);
                }
                break;
            #endregion
            //hadoukenL
            #region
            case PoseType.Hadouken_l:
                //一度だけやる処理
                if (stateEnter)
                {
                    stateEnter = false;
                    Debug.Log("pose:hadoukenL");
                    hadoukenL_anime.SetActive(true);
                    ImageTrack(15, hadoukenL_rect);
                    audiosource.PlayOneShot(hadouken_se);

                }
                //ポーズが変わったら
                if (currentState != newState)
                {
                    hadoukenL_anime.SetActive(false);
                    stateEnter = true;
                }
                //ポーズがそのままならポーズの処理
                else
                {
                    hadoukenL_rect.position += new Vector3(-10, 0, 0);
                }
                break;
            #endregion
            //hadoukenR
            #region
            case PoseType.Hadouken_r:
                //一度だけやる処理
                if (stateEnter)
                {
                    stateEnter = false;
                    Debug.Log("pose:hadoukenR");
                    hadoukenR_anime.SetActive(true);
                    ImageTrack(15, hadoukenR_rect);
                    audiosource.PlayOneShot(hadouken_se);

                }
                //ポーズが変わったら
                if (currentState != newState)
                {
                    hadoukenR_anime.SetActive(false);
                    stateEnter = true;
                }
                //ポーズがそのままならポーズの処理
                else
                {
                    hadoukenR_rect.position += new Vector3(10, 0, 0);
                }
                break;
            #endregion
            //glico
            #region
            case PoseType.Glico:
                //一度だけやる処理
                if (stateEnter)
                {
                    stateEnter = false;
                    Debug.Log("pose:glico");
                    glico_img.SetActive(true);

                }
                //ポーズが変わったら
                if (currentState != newState)
                {
                    glico_img.SetActive(false);
                    stateEnter = true;
                }
                //ポーズがそのままならポーズの処理
                else
                {
                    ImageTrack(0, glico_rect);
                }
                break;
                #endregion
        }

        //state更新
        currentState = newState;

    }

    void ChangeState(PoseType NewState)
    {
        if (newState != NewState)
        {
            newState = NewState;
            Debug.Log("ステートが " + NewState + " に変更されました");
            // ステート変更時の処理をここに追加
            checktext.text = NewState.ToString();
        }
    }

    //画像追尾
    void ImageTrack(int posNum,RectTransform rect)//参照する座標番号、画像のレクト
    {
        Vector3 pos = Pos[posNum];
        Debug.Log("元posデータ" + pos);
        //posの値の範囲:0~1
        pos.x *= Screen.width;
        pos.y *= Screen.height;
        rect.position = new Vector2(pos.x, Screen.height - pos.y);
        Debug.Log("元scale" + rect.localScale);
        //kinokino_rect.localScale *= (-pos.z);

        Debug.Log("pos.x" + pos.x);
        Debug.Log("pos.y" + pos.y);
        Debug.Log("pos.z" + pos.z);
        Debug.Log("座標" + pos);
        Debug.Log("scale" + rect.localScale);
    }

    //ポーズ判定用
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
        if (Pos[0].y > Pos[14].y &&
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
        if (Math.Abs(Pos[15].y - Pos[16].y) < 0.1 &&
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
        if (Pos[25].y < Pos[26].y)
        {

            Debug.Log("左ひざが上がっています");
            return true;
        }
        return false;
    }
    bool IsUpRightKnee()
    {
        //右ひざが左ひざより上
        if (Pos[25].y > Pos[26].y)
        {
            Debug.Log("右ひざが上がっています");
            return true;
        }
        return false;
    }
    bool Ishandfront()
    {
        if (Math.Abs(Pos[12].y - Pos[16].y) < 0.2 &&
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

        if (angle < thresholdAngle)
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
        if (angle < thresholdAngle)
        {
            //Debug.Log("みぎひじが曲げまげ");
            return true;
        }
        return angle < thresholdAngle;
    }

}
