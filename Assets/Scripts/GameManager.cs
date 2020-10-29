using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Headerでinspectorに表示
    [Header("干支のプレファブ")]
    //(古いコメント)etoPrefab変数にGameObject型の情報を代入する(GameObject型のetoPrefabオブジェクト)
    // 宣言する型を GameObject型 から Eto型に変更。EtoPrefabは前と同じようにProject内からアサインできる
    public Eto etoPrefab;　

    //Headerでinspectorに表示
    [Header("干支の生成位置")]
    //etoSetTran変数に、ここにアサインされたゲームオブジェクトの持つTransform型の情報を代入する
    public Transform etoSetTran;

    //Headerでinspectorに表示
    [Header("干支生成時の最大回転角度")]
    //浮動小数点型、maxRotateAngle変数に40.0を代入する
    public float maxRotateAngle = 40.0f;

    //Headerでinspectorに表示
    [SerializeField, Header("生成された干支のリスト")]
    //このクラスのみ参照可
    //リスト宣言、すでに作成したEto型のetoListに新しくリストEto型で初期化する
    private List<Eto> etoList = new List<Eto>();

    //データ構造やオブジェクトの状態を、保存・再構築できるようなフォーマットに変換する
    //Headerでinspectorに表示
    [SerializeField, Header("干支の画像データ")]
    //Sprite型のetoSprites変数を生成
    private Sprite[] etoSprites;

    IEnumerator Start()
    {   // <=  戻り値を void から IEnumerator型に変更して、コルーチンメソッドにする
        // 干支の画像を読みこむ。この処理が終了するまで、次の処理へはいかないようにする
        yield return StartCoroutine(LoadEtoSprites());

        //コルーチンとは実行を停止して Unity へ制御を戻し、ただし続行するときは
        //停止したところから次のフレームで実行を継続することができる関数
        //フレームを跨いで処理を中断・再開させることが出来る仕組み
        //コルーチンを実行、GameData型のinstance変数のcreateEtoCountを参照
        StartCoroutine(CreateEtos(GameData.instance.createEtoCount));
    }

    /// <summary>
    /// 干支の画像を読み込んで配列から使用できるようにする
    /// </summary>
    //コルーチンメソッド。干支画像読み込むまで次の処理にはいかない
    private IEnumerator LoadEtoSprites()
    {
        Debug.Log("生成");
        // etoSprites変数に、配列(整数型のEtoTypeスクリプト内の12個の画像が入るようにSprite型の配列を12個用意する)を初期化する
        etoSprites = new Sprite[(int)EtoType.Count];

        //後から生成して登場させたい
        // ResourcesフォルダのSpritesの"eto"と名前の付いた画像をすべて取得しSprite型の配列に代入する
        // Resources.LoadAllを行い、分割されている干支の画像を順番にすべて読み込んで配列に代入
        etoSprites = Resources.LoadAll<Sprite>("Sprites/eto");

        // ※　１つのファイルを１２分割していない場合には、以下の処理を使います。12分割している場合には使用しません。
        //for(int i = 0; i < etoSprites.Length; i++){
        //     etoSprites[i] = Resources.Load<Sprite>("Sprites/eto_" + i);
        //}

        //コルーチンを途中で終了する。中断ではないので再開されない。
        yield break;
    }

    //private修飾子、このクラスからしかアクセス不可
    //Startメソッドのコルーチンから呼び出し
    // CreateEtosメソッドをコルーチン型で実行する
    // コルーチン型の遅延処理を使う理由は、for 文で繰り返し処理をして干支のクローンを生成する際に
    // まとめて全部生成ではなく、１つ生成するごとに、ほんの少しずつだけ生成間隔を空けることで
    // バラバラに順番に生成されるようにしているため
    // メソッドの引数には int 型の count 変数を受け取って、この値の数だけ干支を生成する
    // そのため、命令する処理で 50 と指定すれば 50個生成し、 80 と指定すれば 80個生成のように
    // 生成する量を調整できる
    // 今回の命令はStartCoroutine(CreateEtos(GameData.instance.createEtoCount)) で、引数として利用されているのは
    // GameData.instance.createEtoCountの部分になり、createEtoCount の値を変更することで生成数を変更できる
    private IEnumerator CreateEtos(int count)
    {
        /// <summary>
        /// 干支を生成
        /// </summary>
        /// <param name="count">生成する数</param>
        /// <returns></returns>

        //for文、繰り返し処理
        //整数型iを0で初期化
        //整数型iが50より小さい場合、iに１を加算し続ける
        for (int i = 0; i < count; i++)
        {
            //処理が満たされていない場合、次の処理を実行する
            // ゲームオブジェクト型のetoに、
            //(第1) GameObject型の eto 変数に、etoPrefabを元に生成された GameObject型の干支(クローン)を代入する
            // 昨日のサイトをみて頂きたいですが Instantiateメソッドは戻り値を持ちます
            // 生成という処理を行うとともに、左辺に変数を用意しておくことで、生成された情報(ここでは干支のクローン)を
            // この戻り値を利用して取得し、eto 変数に代入しています
            // そのため、次以降の処理で eto.～ の書式で干支クローンに対して命令が出せています 


            //(古いコメント)干支プレファブのクローンを干支の生成位置に生成
            //生成された干支を代入する型をGameObject型からEto型に変更する
            Eto eto = Instantiate(etoPrefab, etoSetTran, false);

            //干支クローンの向きを指定(回転させずに-40～40までの位置で角度を調整する)
            // 生成された干支の回転情報を設定(色々な角度になるように)
            eto.transform.rotation = Quaternion.AngleAxis(Random.Range(-maxRotateAngle, maxRotateAngle), Vector3.forward);

            //etoSetTranの 位置に生成されている 干支クローンの localPosition 情報を
            //new Vector2(Random.Range(-400.0f, 400.0f), 1400f) に変更する
            // 生成位置をランダムにして落下位置を変化させる
            eto.transform.localPosition = new Vector2(Random.Range(-400.0f, 400.0f), 1400f);


            // int型のrandomValue変数にランダムな干支を0～12種類の中から１つ選択
            int randomValue = Random.Range(0, (int)EtoType.Count);

            // etoオブジェクト(生成された干支)の初期設定(干支の種類と干支の画像を引数を使ってEtoへ渡す)
            eto.SetUpEto((EtoType)randomValue, etoSprites[randomValue]);

            // etoListにetoを追加
            etoList.Add(eto);

            // コルーチンを一時中断、0.03秒待って次の干支(クローン)を生成
            yield return new WaitForSeconds(0.03f);
        }
    }
}
