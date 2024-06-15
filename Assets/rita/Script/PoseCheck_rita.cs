using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    bool stateEnter = true;
    public PoseType currentState;
    public PoseType newState;

    //test用
    [SerializeField] Text checktext;

    //各ポーズエフェクト
    [SerializeField] GameObject kinokino_img;
    RectTransform kinokino_rect;
    [SerializeField] GameObject lisp_img;
    RectTransform lisp_rect;

    //座標調整用
    float force = 5;

    // Start is called before the first frame update
    void Start()
    {
        Pos = PoseReceiver.landmarkPosition;
        currentState = PoseType.None;  // 初期ステートを設定

        kinokino_rect = kinokino_img.GetComponent<RectTransform>();
        lisp_rect=lisp_img.GetComponent<RectTransform>();
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
                    Vector3 pos = Pos[12];
                    Debug.Log("元posデータ" + pos);
                    //posの値の範囲:0~1
                    pos.x *= Screen.width;
                    pos.y *= Screen.height;
                    lisp_rect.position = new Vector2(pos.x, Screen.height - pos.y);
                    Debug.Log("kinokinoの元scale" + lisp_rect.localScale);
                    //kinokino_rect.localScale *= (-pos.z);

                    Debug.Log("pos.x" + pos.x);
                    Debug.Log("pos.y" + pos.y);
                    Debug.Log("pos.z" + pos.z);
                    Debug.Log("lispの座標" + pos);
                    Debug.Log("lispのscale" + kinokino_rect.localScale);
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
                    Debug.Log("きのきのじゃなくなった");
                    stateEnter = true;
                }
                //ポーズがそのままならポーズの処理
                else
                {
                    Vector3 pos = Pos[0];
                    Debug.Log("元posデータ"+pos);
                    //posの値の範囲:0~1
                    pos.x *= Screen.width;
                    pos.y *= Screen.height;
                    kinokino_rect.position = new Vector2(pos.x, Screen.height-pos.y);
                    Debug.Log("kinokinoの元scale" + kinokino_rect.localScale);
                    //kinokino_rect.localScale *= (-pos.z);

                    Debug.Log("pos.x"+pos.x);
                    Debug.Log("pos.y"+pos.y);
                    Debug.Log("pos.z" + pos.z);
                    Debug.Log("kinokinoの座標"+pos);
                    Debug.Log("kinokinoのscale" + kinokino_rect.localScale);
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

}
