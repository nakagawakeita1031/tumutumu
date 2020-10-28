using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Headerでinspectorに表示
    [Header("干支のプレファブ")]
    //etoPrefab変数にGameObject型の情報を代入する(GameObject型のetoPrefabオブジェクト)
    public GameObject etoPrefab;

    //Headerでinspectorに表示
    [Header("干支の生成位置")]
    //etoSetTran変数にGameObject型の情報を代入する(Transform型のetoSetTranオブジェクト)
    public Transform etoSetTran;

    //Headerでinspectorに表示
    [Header("干支生成時の最大回転角度")]
    //浮動小数点型、maxRotateAngle変数に40.0を代入する
    public float maxRotateAngle = 40.0f;

    // Start is called before the first frame update
    void Start()
    {
        //コルーチンとは実行を停止して Unity へ制御を戻し、ただし続行するときは
        //停止したところから次のフレームで実行を継続することができる関数
        //フレームを跨いで処理を中断・再開させることが出来る仕組み
        //コルーチンを実行、GameData型のinstance変数のcreateEtoCountを参照
        StartCoroutine(CreateEtos(GameData.instance.createEtoCount));
    }

    //private修飾子、このクラスからしかアクセス不可
    //Startメソッドのコルーチンから呼び出し
    //CreateEtos整数型は整数型の50(count=50? なぜ50と表示しないのか？)
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
            //(第1)etoPrefabをを生成
           
            
            //干支プレファブのクローンを干支の生成位置に生成
            GameObject eto = Instantiate(etoPrefab, etoSetTran, false);

            //(第2)クローンの向きを指定(回転させずに-40～40までの位置で角度を調整する)
            // 生成された干支の回転情報を設定(色々な角度になるように)
            eto.transform.rotation = Quaternion.AngleAxis(Random.Range(-maxRotateAngle, maxRotateAngle), Vector3.forward);

            //(第3)falseの場合、etoSetTranに生成したクローンを配置(x軸は-400～400、y軸は1400位置に生成する)
            // 生成位置をランダムにして落下位置を変化させる
            eto.transform.localPosition = new Vector2(Random.Range(-400.0f, 400.0f), 1400f);

            // コルーチンを一時中断、0.03秒待って次の干支(クローン)を生成
            yield return new WaitForSeconds(0.03f);
        }
    }
}
