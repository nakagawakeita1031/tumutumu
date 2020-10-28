using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Eto : MonoBehaviour
{
    //Headerでinspectorに表示
    [Header("干支の種類")]
    //etoType変数にEtotype型の情報を代入する
    public EtoType etoType;

    //Headerでinspectorに表示
    [Header("干支のイメージ変更用")]
    //imgEto変数にImage型の情報を代入する
    public Image imgEto;

    //Headerでinspectorに表示
    [Header("スワイプされた干支である判定。trueの場合、この干支は削除対象となる")]
    //isSelected変数にbool型の情報を代入する(真偽を表す)
    public bool isSelected;

    //Headerでinspectorに表示
    [Header("スワイプされた通し番号。スワイプされた順番が代入される")]
    //num変数にint型(整数)の情報を代入する
    public int num;

    /// <summary>
    /// 干支の初期設定
    /// </summary>
    
    //SetUpEtoメソッド----------------------←編集中
    public void SetUpEto(EtoType etoType, Sprite sprite)
    {
        // 干支の種類を設定
        //public EtoType etoTypにEtoTypeスクリプトを代入する
        this.etoType = etoType;

        // 干支の名前を、設定した干支の種類の名前に変更
        //変数nameに設定したetoTypeを文字列に変換し代入
        name = this.etoType.ToString(); ;

        // 引数で届いた干支のイメージに合わせてメージを変更
        ChangeEtoImage(sprite);
    }

    /// <summary>
    /// 干支のイメージを変更
    /// </summary>
    /// <param name="changeSprite">干支のイメージ</param>
    public void ChangeEtoImage(Sprite changeSprite)
    {
        imgEto.sprite = changeSprite;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
