using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{

    [Header("干支のプレファブ")]
    //(古いコメント)etoPrefab変数にGameObject型の情報を代入する(GameObject型のetoPrefabオブジェクト)
    // 宣言する型を GameObject型 から Eto型に変更。EtoPrefabは前と同じようにProject内からアサインできる
    public Eto etoPrefab;　

    [Header("干支の生成位置")]
    //etoSetTran変数に、ここにアサインされたゲームオブジェクトの持つTransform型の情報を代入する
    public Transform etoSetTran;

    [Header("干支生成時の最大回転角度")]
    //浮動小数点型、maxRotateAngle変数に40.0を代入する
    public float maxRotateAngle = 40.0f;

    //SerializeField←データ構造やオブジェクトの状態を、保存・再構築できるようなフォーマットに変換する
    [SerializeField, Header("生成された干支のリスト")]
    //このクラスのみ参照可
    //リスト宣言、使用するためにEto型のetoListを初期化
    private List<Eto> etoList = new List<Eto>();

    [SerializeField, Header("干支の画像データ")]
    //Sprite型のetoSprites変数を生成
    private Sprite[] etoSprites;

    // 最初にドラッグした干支の情報(firstSelectEto変数にEto型の情報を代入)
    private Eto firstSelectEto;

    // 最後にドラッグした干支の情報(lastSelectEto変数にEto型の情報を代入)
    private Eto lastSelectEto;

    // 最初にドラッグした干支の種類(currentEtoType変数にEtoType型の情報を代入)
    private EtoType? currentEtoType;

    [SerializeField, Header("削除対象となる干支を登録するリスト")]
    //リスト宣言、使用するためにEto型のeraseEtoList変数を初期化
    private List<Eto> eraseEtoList = new List<Eto>();

    [SerializeField, Header("つながっている干支の数")]
    //int型linkCountに0を代入
    private int linkCount = 0;

    [Header("スワイプでつながる干支の範囲")]
    //float型etoDistance変数に1.0を代入
    public float etoDistance = 1.0f;

    [SerializeField]
    //UIManager型のuiManager変数
    private UIManager uiManager;

    private float timer;        // 残り時間計測用

    /// <summary>
    /// ゲームの進行状況
    /// </summary>
    public enum GameState
    {
        Select,     // 干支の選択中
        Ready,      // ゲームの準備中
        Play,       // ゲームのプレイ中
        Result      // リザルト中
    }

    [Header("現在のゲームの進行状況")]
    //GameState型のgameState変数にGameStateクラスのSelect変数を代入
    public GameState gameState = GameState.Select;

    [SerializeField]
    //ResultPopUp型のresultPopUp変数
    private ResultPopUp resultPopUp;


    IEnumerator Start()
    {   // <=  戻り値を void から IEnumerator型に変更して、コルーチンメソッドにする
        // 干支の画像を読みこむ。この処理が終了するまで、次の処理へはいかないようにする
        yield return StartCoroutine(LoadEtoSprites());

        // 残り時間の表示
        uiManager.UpdateDisplayGameTime(GameData.instance.gameTime);

        //コルーチンとは実行を停止して Unity へ制御を戻し、ただし続行するときは
        //停止したところから次のフレームで実行を継続することができる関数
        //フレームを跨いで処理を中断・再開させることが出来る仕組み
        //コルーチンを実行、GameData型のinstance変数のcreateEtoCountを参照
        StartCoroutine(CreateEtos(GameData.instance.createEtoCount));

        // gameStateを準備中に変更
        gameState = GameState.Ready;
    }

    /// <summary>
    /// 干支の画像を読み込んで配列から使用できるようにする
    /// </summary>
    //コルーチンメソッド。干支画像読み込むまで次の処理にはいかない
    private IEnumerator LoadEtoSprites()
    {
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

        // gameStateが準備中のときだけゲームプレイ中に変更
        if (gameState == GameState.Ready)
        {
            gameState = GameState.Play;
        }
    }

    void Update()
    {
        // ゲームのプレイ中以外のgameStateでは処理を行わない
        if (gameState != GameState.Play)
        {
            return;
        }

        // 干支をつなげる処理(もし左クリックが押され、かつfirstSelectEtoがnullだった場合)
        if (Input.GetMouseButtonDown(0) && firstSelectEto == null)
        {
            // 干支を最初にドラッグした際の処理(OnStartDrag処理が行われる)
            //OnStartDragメソッドが処理される
            OnStartDrag();
        }
        //nullではなく、もし左クリックを離した場合
        else if (Input.GetMouseButtonUp(0))
        {
            // 干支のドラッグをやめた（指を離した）際の処理
            //OnEndDragめっそっどが処理させる
            OnEndDrag();
        }
        //nullではなかった場合、もしfirstSelectEtoがnullではない場合
        else if (firstSelectEto != null)
        {
            // 干支のドラッグ（スワイプ）中の処理	    
            //OnDraggingメソッドが処理される
            OnDragging();
        }

        // ゲームの残り時間のカウント処理
        timer += Time.deltaTime;

        // timerが 1 以上になったら
        if (timer >= 1)
        {
            // リセットして再度加算できるように
            timer = 0;

            // 残り時間をマイナス 
            GameData.instance.gameTime--;

            // 残り時間がマイナスになったら
            if (GameData.instance.gameTime <= 0)
            {

                //0で止める
                GameData.instance.gameTime = 0;

                // TODO ゲーム終了を追加する
                StartCoroutine(GameUp());
            }

            // 残り時間の表示更新
            uiManager.UpdateDisplayGameTime(GameData.instance.gameTime);
        }
    }

    /// <summary>
    /// 干支を最初にドラッグした際の処理
    /// </summary>
    private void OnStartDrag()
    {
        // 画面をタップした際の位置情報を、CameraクラスのScreenToWorldPointメソッドを利用してCanvas上の座標に変換
        //Camera.main(メインカメラオブジェクト)に属するScreenToWorldPointメソッドをを使いオブジェクトの位置情報をCanvasScene上の座標に変換し結果をhitに返す。
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        // 干支がつながっている数を初期化
        //linkCount変数に0を代入
        linkCount = 0;

        // 変換した座標のコライダーを持つゲームオブジェクトがあるか確認
        //もしCanvas上にcolliderを持つオブジェクトがnullではないとき
        if (hit.collider != null)
        {
            // ゲームオブジェクトがあった場合、そのゲームオブジェクトがEtoクラスを持っているかどうか確認
            //もし当たり判定のあるゲームオブジェクトがEtoクラスを持っている場合dragEto変数に代入された場合
            //やりたいことが３つある
            //Etoクラスを持っているか確認
            //Etoクラスを持っていたらEtoクラスに代入
            //Etoクラスを持っていたらtrueを返す
            if (hit.collider.gameObject.TryGetComponent(out Eto dragEto))
            {
                // Etoクラスを持っていた場合には、以下の処理を行う

                // 最初にドラッグした干支の情報を変数に代入
                //firstSelectEto変数にdragEto(オブジェクトの情報)変数を代入
                firstSelectEto = dragEto;

                // 最後にドラッグした干支の情報を変数に代入(最初のドラッグなので、最後のドラッグも同じ干支)
                //lastSelectEto変数にdragEto(オブジェクトの情報)変数を代入
                lastSelectEto = dragEto;

                // 最初にドラッグしている干支の種類を代入 = 後ほど、この情報を使ってつながる干支かどうかを判別する
                //currentEtoType変数にdragEtoのetoType(オブジェクトの種類)を代入
                currentEtoType = dragEto.etoType;

                // 干支の状態が「選択中」であると更新   
                dragEto.isSelected = true;

                // 干支に何番目に選択されているのか、通し番号を登録　
                dragEto.num = linkCount;

                // 削除する対象の干支を登録するリストを初期化　
                //eraseEtoList変数を初期化
                eraseEtoList = new List<Eto>();

                // ドラッグ中の干支を削除の対象としてリストに登録
                AddEraseEtoList(dragEto);
            }
        }
    }

    /// <summary>
    /// 干支のドラッグ（スワイプ）中処理
    /// </summary>
    private void OnDragging()
    {
        // OnStartDragメソッドと同じ処理で、指の位置をワールド座標に変換しRayを発射し、その位置にあるコライダーを持つオブジェクトを取得してhit変数へ代入
        ////Camera.main(メインカメラオブジェクト)に属するScreenToWorldPointメソッドをを使いオブジェクトの位置情報をCanvasScene上の座標に変換し結果をhitに返す。
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        // Rayの戻り値があり(hit変数がnullではない)、hit変数のゲームオブジェクトがEtoクラスを持っていたら
        //もし当たり判定のあるゲームオブジェクトが空ではなく、かつEtoクラスのdragEto変数を持っているか確認できた場合
        if (hit.collider != null && hit.collider.gameObject.TryGetComponent(out Eto dragEto))
        {

            // 現在選択中の干支の種類がnullなら処理は行わない
            //もしcurrentEtoTypeがnullの場合何も処理しない
            if (currentEtoType == null)
            {
                return;
            }

            // dragEto変数の干支の種類が最初に選択した干支の種類と同じであり、最後にタップしている干支と現在の干支が違うオブジェクトであり、かつ、現在の干支がすでに「選択中」でなければ
            //dragEto変数のetoTypeがcurrentEtoType同じでlastSelectEtoが
            if (dragEto.etoType == currentEtoType && lastSelectEto != dragEto && !dragEto.isSelected)
            {

                // 現在タップしている干支の位置情報と最後にタップした干支の位置情報と比べて、差分の値（干支通しの距離）を取る
                float distance = Vector2.Distance(dragEto.transform.position, lastSelectEto.transform.position);

                // 干支同士の距離が設定値よりも小さければ(２つの干支が離れていなければ)、干支をつなげる
                if (distance < etoDistance)
                {

                    // 現在の干支を選択中にする
                    dragEto.isSelected = true;

                    // 最後に選択している干支を現在の干支に更新
                    lastSelectEto = dragEto;

                    // 干支のつながった数のカウントを１つ増やす
                    linkCount++;

                    // 干支に通し番号を設定
                    dragEto.num = linkCount;

                    // 削除リストに現在の干支を追加
                    AddEraseEtoList(dragEto);
                }
            }

            // 現在の干支の種類を確認(現在の干支(dragEtoの情報であれば、他の情報でもよい。ちゃんと選択されているかの確認用))		
            Debug.Log(dragEto.etoType);

            // 削除リストに２つ以上の干支が追加されている場合
            if (eraseEtoList.Count > 1)
            {

                // 現在の干支の通し番号を確認
                Debug.Log(dragEto.num);

                // 条件に合致する場合、削除リストから干支を除外する(ドラッグしたまま１つ前の干支の戻る場合、現在の干支を削除リストから除外する)
                if (eraseEtoList[linkCount - 1] != lastSelectEto && eraseEtoList[linkCount - 1].num == dragEto.num && dragEto.isSelected)
                {

                    // 選択中のボールを取り除く 
                    RemoveEraseEtoList(lastSelectEto);

                    lastSelectEto.GetComponent<Eto>().isSelected = false;

                    // 最後のボールの情報を、前のボールに戻す
                    lastSelectEto = dragEto;

                    // つながっている干支の数を減らす	
                    linkCount--;
                }
            }
        }
    }

    /// <summary>
    /// 干支のドラッグをやめた（指を画面から離した）際の処理
    /// </summary>
    private void OnEndDrag()
    {

        // つながっている干支が３つ以上あったら削除する処理にうつる
        //もし干支が3以上ある場合
        if (eraseEtoList.Count >= 3)
        {

            // 選択されている干支を消す
            //iが干支の数より少なければ足し続ける
            for (int i = 0; i < eraseEtoList.Count; i++)
            {
                // 干支リストから取り除く
                //干支リストからその数を取り除く
                etoList.Remove(eraseEtoList[i]);

                // 干支を削除
                //干支リストの数のゲームオブジェクトを破棄する
                Destroy(eraseEtoList[i].gameObject);
            }

            // スコアと消した干支の数の加算
            //AddScoresメソッド
            AddScores(currentEtoType, eraseEtoList.Count);


            // 消した干支の数だけ新しい干支をランダムに生成
            StartCoroutine(CreateEtos(eraseEtoList.Count));

            // 削除リストをクリアする
            eraseEtoList.Clear();
        }
        //干支の数が２つ以下なら
        else
        {
            // つながっている干支が２つ以下なら、削除はしない

            // 削除リストから、削除候補であった干支を取り除く		
            for (int i = 0; i < eraseEtoList.Count; i++)
            {

                // 各干支の選択中の状態を解除する
                eraseEtoList[i].isSelected = false;

                // 干支の色の透明度を元の透明度に戻す
                ChangeEtoAlpha(eraseEtoList[i], 1.0f);
            }
        }

        // 次回の干支を消す処理のために、各変数の値をnullにする	
        firstSelectEto = null;
        lastSelectEto = null;
        currentEtoType = null;
    }

    /// <summary>
    /// 選択された干支を削除リストに追加
    /// </summary>
    /// <param name="dragEto"></param>
    private void AddEraseEtoList(Eto dragEto)
    {
        // 削除リストにドラッグ中の干支を追加
        eraseEtoList.Add(dragEto);

        // ドラッグ中の干支のアルファ値を0.5fにする(半透明にすることで、選択中であることをユーザーに伝える)
        ChangeEtoAlpha(dragEto, 0.5f);
    }

    /// <summary>
    /// 前の干支に戻った際に削除リストから削除
    /// </summary>
    /// <param name="dragEto"></param>
    private void RemoveEraseEtoList(Eto dragEto)
    {
        // 削除リストから削除
        eraseEtoList.Remove(dragEto);

        // 干支の透明度を元の値(1.0f)に戻す
        ChangeEtoAlpha(dragEto, 1.0f);

        // 干支の「選択中」の情報がtrueの場合
        if (dragEto.isSelected)
        {
            // falseにして選択中ではない状態に戻す
            dragEto.isSelected = false;
        }
    }

    /// <summary>
    /// 干支のアルファ値を変更
    /// </summary>
    /// <param name="dragEto"></param>
    /// <param name="alphaValue"></param>
    private void ChangeEtoAlpha(Eto dragEto, float alphaValue)
    {
        // 現在ドラッグしている干支のアルファ値を変更
        dragEto.imgEto.color = new Color(dragEto.imgEto.color.r, dragEto.imgEto.color.g, dragEto.imgEto.color.b, alphaValue);
    }

    /// <summary>
    /// スコアと消した干支の数を加算
    /// </summary>
    /// <param name="etoType">消した干支の種類</param>
    /// <param name="count">消した干支の数</param>
    //
    private void AddScores(EtoType? etoType, int count)
    {
        // スコアを加算(EtoPoint * 消した数)
        //GameDataクラスのinstance.score変数にetoPoint * countの合計を代入
        GameData.instance.score += GameData.instance.etoPoint * count;

        // 消した干支の数を加算
        //eraceEtoCountにcountを加算する
        GameData.instance.eraseEtoCount += count;

        // スコア加算と画面の更新処理
        uiManager.UpdateDisplayScore();
    }

    /// ゲーム終了処理
    /// </summary>
    private IEnumerator GameUp()
    {

        // gameStateをリザルトに変更する = Updateメソッドが動かなくなる
        gameState = GameState.Result;

        yield return new WaitForSeconds(1.5f);

        // TODO リザルトの処理を実装する
        yield return StartCoroutine(MoveResultPopUp());
    }

    /// <summary>
    /// リザルドポップアップを画面内に移動
    /// </summary>
    /// <returns></returns> 
    private IEnumerator MoveResultPopUp()
    {

        // DoTweenの機能を使って、ResultPopUpゲームオブジェクトを画面外から画面内に移動させる
        //resultPopUpの位置情報を0に1.0秒かけてEase.Linearアニメーション形式で移動させる
        resultPopUp.transform.DOMoveY(0, 1.0f).SetEase(Ease.Linear)
                .OnComplete(() => {
                    // TODO 移動完了後に、リザルト内容を表示
                    // リザルト表示(スコアと消した干支の数を渡す)　TODOを実装します
                    resultPopUp.DisplayResult(GameData.instance.score, GameData.instance.eraseEtoCount);

                }
    );

        yield return new WaitForSeconds(1.0f);
    }


}
