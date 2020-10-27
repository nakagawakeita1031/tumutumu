using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    //このGameDataの持つ情報は一つしか存在しません
    public static GameData instance;

    //アタッチされたゲームオブジェクト上のinspector上に「ゲームに登場する干支の最大種類数」を表示する
    [Header("ゲームに登場する干支の最大種類数")]

    //整数型、etoTypeCountに５を代入する
    public int etoTypeCount = 5;

    //アタッチされたゲームオブジェクト上のinspector上に「ゲーム開始時に生成する干支の数」を表示する
    [Header("ゲーム開始時に生成する干支の数")]

    //整数型、createEtoCountに50を代入する
    public int createEtoCount = 50;

    void Awake()//最初に下記が実行される
    {
        if (instance == null)//インスタンス(実物？)が何も存在していない場合
        {
            //ゲームデータクラスのgameObjectを生成？する
            //Sceneをまたいでも破棄されない
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else//違う場合
        {
            //gameObjectを破棄する
            Destroy(gameObject);
        }
    }
}
