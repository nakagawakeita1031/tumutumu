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
        //this.etoType に、引数で届いた etoType を代入する
        // EtoType は enum 型なので、enum列挙子という情報を持ちます。それが、子から始まる干支の種類です
        // この enum列挙子の中の１つの干支の種類が、etoType 変数に代入された状態でこのメソッドに届いています
        // それを代入していますので、このthis.etoType には１つの干支の種類が代入されている状態になります
        this.etoType = etoType;

        // 干支の名前を、設定した干支の種類の名前に変更
        //変数nameに設定したetoTypeを文字列に変換し代入
        //< 補足 >
        // 変数の name は、小文字の transform 変数と同じで、Unityが自動的に用意している変数です。
        // このスクリプトがアタッチされているゲームオブジェクトの名前の情報が入っている string 型の変数になります。
        // etoType 変数の型は EtoType 型です。そのため、代入処理を成立させるには、 string = string である必要があります。
        // そこで、数字などを文字列に変換する際にも利用するToStringメソッドを利用して、 EtoType型を string 型にしています
        // そして (string) name = (string)etoType の式が成立します。型の変換をキャストといいます。
        // 例えば this.etoType の値が 辰 であるなら、これを文字列にキャストし、name 変数に代入すると、
        // ゲームオブジェクトの名前が 辰 になり、ヒエラルキー上でも 辰 と表示されるようになります
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
