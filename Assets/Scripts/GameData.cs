using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    //このGameDataの持つ情報は一つしか存在しません←自分
    //instance変数にGameData型の情報を代入して利用する。(public static GameData instance = this(GameData);)
    //static修飾子がついているので、この変数の情報は、クラス名.変数名と書くことでどのクラスからでも参照できる
    //この場合、GameData.instanceと記述して利用する
    public static GameData instance;

    //アタッチされたゲームオブジェクト上のinspector上に「ゲームに登場する干支の最大種類数」を表示する
    [Header("ゲームに登場する干支の最大種類数")]

    //整数型、etoTypeCountに５を代入する
    public int etoTypeCount = 5;

    //アタッチされたゲームオブジェクト上のinspector上に「ゲーム開始時に生成する干支の数」を表示する
    [Header("ゲーム開始時に生成する干支の数")]

    //整数型、createEtoCountに50を代入する
    public int createEtoCount = 50;

    void Awake()//Startメソッドよりも最初に実行される。
    {
        //instance変数に、GameData型の値が代入されていない(空、null)場合
        if (instance == null)//インスタンス(実物？)が何も存在していない場合←自分
        {
            //ゲームデータクラスのgameObjectを生成？する←自分
            //instance変数に、GameDataクラス自身を代入する
            //これにより、instanceの中身はGameDataとなる
            instance = this;
            ////Sceneをまたいでも破棄されない
            DontDestroyOnLoad(gameObject);
        }
        else//違う場合
        {
            // instance の中身が Null ではない場合とはすなわち、GameData が代入されている状態
            // つまり GameData がゲーム中に存在していることになるので
            // こちらの分岐に入る時は、2つ目のGameData が存在した場合になる
            // シングルトンパターンは ゲーム中に１つしか存在しないことを実装するパターンなので
            // それを実現するために、2つ目以降の GameData は破棄し、
            // 常にゲーム内にはGameData は１つだけになるようにする

            //gameObjectを破棄する => GameData クラスを持つゲームオブジェクトを破棄
            Destroy(gameObject);
        }
    }
}
