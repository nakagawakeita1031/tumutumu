using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [Header("現在のスコア")]
    //int型score変数に0を代入
    public int score = 0;

    [Header("干支を消した際に加算されるスコア")]
    //int型etoPoint変数に100を代入
    public int etoPoint = 100;

    [Header("消した干支の数")]
    //int型eraseEtoCount変数に0を代入
    public int eraseEtoCount = 0;

    [SerializeField, Header("1回辺りのゲーム時間")]
    //int型initTime変数に60を代入
    private int initTime = 60;

    [Header("現在のゲームの残り時間")]
    //float型gameTime変数
    public float gameTime;

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
        // ゲームの初期化
        InitGame();
    }
    /// <summary>
    /// ゲーム初期化
    /// </summary>
    private void InitGame()
    {
        //score変数に0を代入
        //eraseEtoCountに0を代入
        //"Init Game"をログに表示
        score = 0;
        eraseEtoCount = 0;

        // ゲーム時間を設定
        //gameTime変数にinitTimeを代入
        gameTime = initTime;
        Debug.Log("Init Game");
    }

    /// <summary>
    /// 現在のゲームシーンを再読み込み
    /// </summary>
    /// <returns></returns>
    public IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(1.0f);

        // 現在のゲームシーンを取得し、シーンの名前を使ってLoadScene処理を行う(再度、同じゲームシーンを呼び出す)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // 初期化 GameDataゲームオブジェクトはシーン遷移しても破棄されない設定になっていますので、ここで再度、初期化の処理を行う必要があります。
        InitGame();
    }
}

