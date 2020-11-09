using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{   
        [SerializeField]
        //テキスト型のtxtScore変数
        private Text txtScore;

    [SerializeField]
    //Text型txtTimer変数
    private Text txtTimer;


    /// <summary>
    /// 画面表示スコアの更新処理
    /// </summary>
    public void UpdateDisplayScore()//UpdateDisplayScoreメソッド
    {
         // 画面に表示しているスコアの値を更新
      　 //txtScore変数のtextにゲームデータ型のinstance変数.scoreを文字に変換する
         txtScore.text = GameData.instance.score.ToString();
    }
    /// <summary>
    /// ゲームの残り時間の表示更新
    /// </summary>
    /// <param name="time"></param>
    public void UpdateDisplayGameTime(float time)
    {
        txtTimer.text = time.ToString("F0");
    }
}

