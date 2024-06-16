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
        KujoJotaro,
        Stroheim,
        HighwayStar,
        Kamehameha,
        Hadouken_l,
        Hadouken_r,
        Glico,
        Lisp,
        None,



        Kinokino,
        Giornogiovana,
       
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
    [SerializeField] AudioClip asobanai_se;
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
    //ハイウェイスター
    [SerializeField] GameObject highwaystar_img;
    RectTransform highwaystar_rect;
    //スタープラチナ
    [SerializeField] GameObject starplatinum_img;
    RectTransform starplatinum_rect;
    //かめはめ派
    [SerializeField] GameObject kamehame_fx;
    RectTransform kamehame_rect;
    ParticleSystem kamehame_par;

    //ポーズサンプル
    PoseSample posesample;
    Vector3[] currentpos;
    Vector3[] ps_starplatinum;//スタープラチナム
    Vector3[] ps_stroheim;//シュトロハイム
    Vector3[] ps_highwaystar;
    Vector3[] ps_glico;
    Vector3[] ps_hadoukenR;
    Vector3[] ps_hadoukenL;
    Vector3[] ps_kamehameha;
    Vector3[] ps_lisp;

    //類似度（デバッグ用）
    float[] pos_cos;

    //座標調整用
    float force = 5;

    // 絶対座標のVector3配列から相対座標のVector3配列に変換
    public static Vector3[] ConvertToRelativeCoordinates(Vector3[] absoluteCoordinates, int referenceIndex)
    {
        if (referenceIndex < 0 || referenceIndex >= absoluteCoordinates.Length)
        {
            Debug.LogError("参照インデックスが配列の範囲外です。");
            return null;
        }

        Vector3 referencePoint = absoluteCoordinates[referenceIndex]; // 参照点
        Vector3[] relativeCoordinates = new Vector3[absoluteCoordinates.Length];

        for (int i = 0; i < absoluteCoordinates.Length; i++)
        {
            // 相対座標の計算
            relativeCoordinates[i] = absoluteCoordinates[i] - referencePoint;
        }

        return relativeCoordinates;
    }

    //cos類似度
    public static float ManualCos(Vector3[] A, Vector3[] B)
    {
        if (A.Length != B.Length)
        {
            Debug.LogError("配列の長さが異なります。");
            return 0f;
        }

        float dotSum = 0f;
        float aNormSum = 0f;
        float bNormSum = 0f;

        for (int i = 0; i < A.Length; i++)
        {
            dotSum += Vector3.Dot(A[i], B[i]);
            aNormSum += A[i].sqrMagnitude;
            bNormSum += B[i].sqrMagnitude;
        }

        float cos = dotSum / (Mathf.Sqrt(aNormSum) * Mathf.Sqrt(bNormSum) + 1e-10f);

        // 検出できない場合の処理
        cos = (cos == 0) ? float.NaN : cos;

        return cos;
    }

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
        highwaystar_rect=highwaystar_img.GetComponent<RectTransform>();
        starplatinum_rect=starplatinum_img.GetComponent<RectTransform>();
        kamehame_rect=kamehame_fx.GetComponent<RectTransform>();
        //Particle取得
        kamehame_par = kamehame_fx.GetComponent<ParticleSystem>();

        //ポーズサンプル
        posesample=GetComponent<PoseSample>();

        ps_starplatinum = posesample.starplatinum_sample;//スタープラチナ
        ps_starplatinum = ConvertToRelativeCoordinates(ps_starplatinum, 12);
        ps_stroheim = posesample.stroheim_sample;//シュトロハイム
        ps_stroheim=ConvertToRelativeCoordinates(ps_stroheim, 12);
        ps_highwaystar=posesample.highwaystar_sample;//ハイウェイスター
        ps_highwaystar=ConvertToRelativeCoordinates(ps_highwaystar, 12);
        ps_kamehameha = posesample.kamehameha_sample;//かめはめ波
        ps_kamehameha=ConvertToRelativeCoordinates(ps_kamehameha, 12);
        ps_hadoukenL = posesample.hadoukenL_sample;//波動拳左
        ps_hadoukenL = ConvertToRelativeCoordinates(ps_hadoukenL, 12);
        ps_hadoukenR =posesample.hadoukenR_sample;//波動拳右
        ps_hadoukenR=ConvertToRelativeCoordinates(ps_hadoukenR, 12);
        ps_glico=posesample.glico_sample;//グリコ
        ps_glico=ConvertToRelativeCoordinates(ps_glico, 12);
        ps_lisp=posesample.lisp_sample;//リスプ
        ps_lisp=ConvertToRelativeCoordinates(ps_lisp, 12);
        
    }

    // Update is called once per frame
    void Update()
    {
        Pos = PoseReceiver.landmarkPosition;  // 毎フレーム位置を更新
        currentpos = ConvertToRelativeCoordinates(Pos, 12);
;

        // ここでポーズの位置に応じたステート変更を行う

        /*        Debug.Log("currentpos.Length:"+currentpos.Length);
                for (int i = 0; i < currentpos.Length; i++)
                {
                    Debug.Log("currentpos at index " + i + ": " + currentpos[i]);
                }

                Debug.Log("starplatinum.length:" + ps_starplatinum.Length);
                Debug.Log(ps_starplatinum);
                for (int i = 0; i < ps_starplatinum.Length; i++)
                {
                    Debug.Log("ps_starplatinum at index " + i + ": " + ps_starplatinum[i]);
                }*/
        //類似度を格納
        /* try
         {
             Debug.Log("類似度を格納");
             pos_cos[0] = ManualCos(currentpos, ps_starplatinum);
             pos_cos[1] = ManualCos(currentpos, ps_stroheim);
             pos_cos[2] = ManualCos(currentpos, ps_highwaystar);
             pos_cos[3] = ManualCos(currentpos, ps_kamehameha);
             pos_cos[4] = ManualCos(currentpos, ps_hadoukenL);
             pos_cos[5] = ManualCos(currentpos, ps_hadoukenR);
             pos_cos[6] = ManualCos(currentpos, ps_glico);
             pos_cos[7] = ManualCos(currentpos, ps_lisp);

             int Maxpose = -1;
             float Maxcos = 0;
             //一番類似度の高いものを探す
             for (int i = 0; i < pos_cos.Length; i++)
             {
                 if (pos_cos[i] > Maxcos)
                 {
                     Maxcos = pos_cos[i];
                     Maxpose = i;
                 }
             }
             //基準値を超えていれば判定
             if (pos_cos[Maxpose] > 0.93)
             {
                 Debug.Log("cos類似度" + pos_cos[Maxpose]);
                 ChangeState((PoseType)Maxpose);

             }
             else
             {
                 ChangeState(PoseType.None);
             }
         }
         catch
         {
             Debug.Log("類似度比較失敗:リトライします");
         }
         */

        #region
        //シュトロハイム
        // 両肘が鼻より上
        //手先が鼻より下
        // 左ひざを突き出している


        if (ManualCos(currentpos, ps_stroheim) > 0.85)
        {
            Debug.Log("シュトロハイムcos類似度：" + ManualCos(currentpos, ps_stroheim));
            ChangeState(PoseType.Stroheim);
        }
        /*else if (Israisearm() &&
        IsHandBelowNose() &&
        Isupleftknee())
        {
            ChangeState(PoseType.Stroheim);
        }*/
        //グリコ
        //左ひざが上がっている
        //両手が上がっている
        else if (ManualCos(currentpos, ps_glico) > 0.9&& Isupleftknee() &&
        Israisearm())
        {
            ChangeState(PoseType.Glico);
        }
        /*else if (Isupleftknee() &&
        Israisearm())
        {
            ChangeState(PoseType.Glico);
        }*/
        //波動拳L
        //横を向いている
        // 両手を前に突き出している
        //手と手がある程度重なっている
       /* else if (ManualCos(currentpos, ps_hadoukenL) > 0.9&& Isside() &&
        Ishandfront() &&
        Ishandsoverlap() && Pos[11].x > Pos[15].x)
        {
            ChangeState(PoseType.Hadouken_l);
        }*/
        else if (Isside() &&
        Ishandfront() &&
        Ishandsoverlap() && Pos[11].x > Pos[15].x&&ManualCos(currentpos, ps_hadoukenL) > 0.85)
        {
            ChangeState(PoseType.Hadouken_l);
        }
        //波動拳R
        //横を向いている
        //両手を前に突き出している
        //手と手がある程度重なっている
       /* else if (ManualCos(currentpos, ps_hadoukenR) > 0.9&& Isside() &&
        Ishandfront() &&
        Ishandsoverlap() && Pos[12].x < Pos[16].x)
        {
            ChangeState(PoseType.Hadouken_r);
        }*/
        else if (Isside() &&
        Ishandfront() &&
        Ishandsoverlap() && Pos[12].x < Pos[16].x&& ManualCos(currentpos, ps_hadoukenR) > 0.85)
        {
            ChangeState(PoseType.Hadouken_r);
        }
        //ハイウェイスター
        //肩とひざの判定が近い
        //手がくっついている

        //ManualCos(currentpos, ps_highwaystar) > 0.93)
        else if (Ishandsoverlap() &&
            Ismaekagami())
        {
            ChangeState(PoseType.HighwayStar);
        }
        /*else if (
            Ishandsoverlap() &&
            Ismaekagami()
        )
        {
            ChangeState(PoseType.HighwayStar);
        }*/
        //かめはめ波
        //正面を向いている
        //手と手がある程度重なっている
       /* else if (ManualCos(currentpos, ps_kamehameha) > 0.9&&Isfront())
        {
            ChangeState(PoseType.Kamehameha);
        }*/
        else if (Isfront() &&
        Ishandsoverlap())
        {
            ChangeState(PoseType.Kamehameha);
        }
        //空条承太郎＆スタープラチナ
        //横向いている
        //手を前に突き出している
        //どちらかの手を腰に当てている


        else if (IsUpSomeHand() && 
            ManualCos(currentpos, ps_starplatinum) > 0.95 &&
        (Isnear(20, 24, 0.3) ||
        Isnear(21, 23, 0.3)))
        {
            ChangeState(PoseType.KujoJotaro);
        }
        /*else if (Isside() &&
        IsUpSomeHand() &&
        (Isnear(20, 24, 0.3) ||
        Isnear(21, 23, 0.3)))
        {
            ChangeState(PoseType.KujoJotaro);
        }*/
        //ゴールドエクスペリエンス
        //肩の中心と右手が近い
        //左手は腰


        else if (IsHandCenter() &&
        Isnear(15, 23, 0.3)
        )
        {
            ChangeState(PoseType.Giornogiovana);
        }
        //Lispポーズ
        else if (ManualCos(currentpos, ps_lisp) > 0.9)
        {
            ChangeState(PoseType.Lisp);
        }
        /*else if (Math.Abs(Pos[0].y - Pos[13].y) <= 0.1 && Math.Abs(Pos[0].x - Pos[13].x) <= 0.2
            || Math.Abs(Pos[0].y - Pos[14].y) <= 0.1 && Math.Abs(Pos[0].x - Pos[14].x) <= 0.2)
        {
            ChangeState(PoseType.Lisp);
        }*/
        //きのきの
        else if (Pos[0].y > Pos[21].y && Pos[0].y > Pos[22].y&&IsElbowBelowNose())
        {
            ChangeState(PoseType.Kinokino);
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

        #endregion

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
                    audiosource.PlayOneShot(asobanai_se);

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
            //HighwayStar
            #region
            case PoseType.HighwayStar:
                //一度だけやる処理
                if (stateEnter)
                {
                    highwaystar_img.SetActive(true);
                    stateEnter = false;
                }
                //ポーズが変わったら
                if (currentState != newState)
                {
                    stateEnter = true;
                    highwaystar_img.SetActive(false);
                }
                //ポーズがそのままならポーズの処理
                else
                {
                    ImageTrack(24, highwaystar_rect);
                }
                break;
            #endregion
            //KujoJotaro
            #region
            case PoseType.KujoJotaro:
                //一度だけやる処理
                if (stateEnter)
                {
                    starplatinum_img.SetActive(true);
                    stateEnter = false;
                }
                //ポーズが変わったら
                if (currentState != newState)
                {
                    stateEnter = true;
                    starplatinum_img.SetActive(false);
                }
                //ポーズがそのままならポーズの処理
                else
                {
                    ImageTrack(0, starplatinum_rect);
                }
                break;
            #endregion
            //Kamehameha
            #region
            case PoseType.Kamehameha:
                //一度だけやる処理
                if (stateEnter)
                {
                    kamehame_fx.SetActive(true);
                    kamehame_par.Play();
                    stateEnter = false;
                    GameObject.Find("Canvas").GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
                }
                //ポーズが変わったら
                if (currentState != newState)
                {
                    stateEnter = true;
                    kamehame_fx.SetActive(false);
                    GameObject.Find("Canvas").GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                }
                //ポーズがそのままならポーズの処理
                else
                {
                    Vector3 pos = Pos[15];
                    Debug.Log("元posデータ" + pos);
                    //posの値の範囲:0~1
                    pos.x *= Screen.width;
                    pos.y *= Screen.height;
                    pos.x -= 960;
                    pos.y += 520;
                    kamehame_rect.transform.localPosition = new Vector3(pos.x, Screen.height - pos.y, 0);
                    Debug.Log("元scale" + kamehame_rect.transform.localPosition);
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
        rect.position = new Vector3(pos.x, Screen.height - pos.y,0);
        Debug.Log("元scale" + rect.localScale);
        //kinokino_rect.localScale *= (-pos.z);
    }

    //ポーズ判定用
    bool Isside()
    {
        if (Math.Abs(Pos[11].x - Pos[12].x) < 0.1)
        {
            Debug.Log("横を向いています");
            return true;
        }
        return false;
    }
    bool Isfront()
    {
        if (Math.Abs(Pos[11].x - Pos[12].x) > 0.1)
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
        if (Math.Abs(Pos[15].y - Pos[16].y) < 0.08 &&
        Math.Abs(Pos[15].x - Pos[16].x) < 0.08)
        {
            Debug.Log("両手が重なっています");
            return true;
        }
        return false;
    }
    bool Isupleftknee()
    {
        //左ひざが右ひざより上
        //要修正（ある程度離れたら）
        if ((Pos[26].y - Pos[25].y) > 0.05)
        {

            Debug.Log("左ひざが上がっています");
            return true;
        }
        return false;
    }
    bool IsUpRightKnee()
    {
        //右ひざが左ひざより上
        if ((Pos[25].y - Pos[26].y) > 0.05)
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

    bool IsUpSomeHand()
    {
        if (Math.Abs(Pos[0].y - Pos[16].y) < 0.2 ||
        Math.Abs(Pos[0].y - Pos[15].y) < 0.2)
        {
            Debug.Log("どちらかの手が上がっています");
            return true;
        }
        return false;
    }
    bool IsHandCenter()
    {
        float thresholdDistance = 0.15f;
        // 左肩と右肩の中間点を計算
        float shoulderCenter = (Pos[11].x + Pos[12].x) / 2;
        //Debug.Log("中心点：" + shoulderCenter);

        // 距離がしきい値以下かどうかを判定
        if (Math.Abs(shoulderCenter - Pos[20].x) < thresholdDistance)
        {

            Debug.Log("手が胸の中心にあります:" + Pos[20].x);
            return true;
        }
        return false;
    }
    bool IsHandBelowNose()
    {
        //判定悪
        if (Pos[0].y < Pos[19].y &&
        Pos[0].y < Pos[20].y)
        {
            Debug.Log("手が鼻より下です");
            return true;
        }
        return false;
    }
    bool IsElbowBelowNose()
    {
        Debug.Log("Pos[0]:" + Pos[0].y + "Pos[14]:" + Pos[14].y + "Pos[13]:" + Pos[13].y);
        if (Pos[0].y < Pos[14].y)
        {
            Debug.Log("右ひじが鼻より下です");
        }
        if (Pos[0].y < Pos[13].y)
        {
            Debug.Log("左ひじが鼻より下です");
        }
        //判定悪
        if (Pos[0].y < Pos[14].y &&
        Pos[0].y < Pos[13].y)
        {
            Debug.Log("肘が鼻より下です");
            return true;
        }
        return false;
    }
    bool Ismaekagami()
    {
        //肩と腰が近い
        if (Math.Abs(Pos[11].y - Pos[23].y) < 0.25 ||
        Math.Abs(Pos[12].y - Pos[24].y) < 0.25)
        {
            Debug.Log("前かがみです");
            return true;
        }
        return false;
    }
    bool Isudekumi()
    {
        //右手と左ひじ
        //左手と右ひじ
        if (Isnear(14, 15, 0.1) && Isnear(13, 16, 0.1))
        {
            Debug.Log("腕を組んでいます");
            return true;
        }
        return false;
    }
    bool Isnear(int a, int b, double sikii)
    {
        if (Math.Abs(Pos[a].y - Pos[b].y) <= sikii && Math.Abs(Pos[a].x - Pos[b].x) <= sikii)
        {
            Debug.Log(a + "と" + b + "が近づきました");
            return true;
        }
        return false;
    }
    bool Isfar(int a, int b, double sikii)
    {
        if (Math.Abs(Pos[a].y - Pos[b].y) >= sikii && Math.Abs(Pos[a].x - Pos[b].x) >= sikii)
        {
            Debug.Log(a + "と" + b + "は遠いです");
            return true;
        }
        return false;
    }

}
