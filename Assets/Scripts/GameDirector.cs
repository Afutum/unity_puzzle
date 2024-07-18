using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameDirector : MonoBehaviour
{
    // アイテムのプレハブ
    [SerializeField] List<GameObject> prefabBubbles;
    // ゲーム時間
    [SerializeField] float gameTimer;
    // フィールドのアイテム総数
    [SerializeField] int fieldItemCountMax;
    // 削除できるアイテム数
    [SerializeField] int deleteCount;
    // コンボカウント
    [SerializeField] int comboCount;

    // UI
    //[SerializeField] TextMeshProUGUI textGameScore;
    [SerializeField] TextMeshProUGUI textGameTimer;
    [SerializeField] TextMeshProUGUI textComboCounter;
    [SerializeField] TextMeshProUGUI combo;
    [SerializeField] TextMeshProUGUI power;
    [SerializeField] TextMeshProUGUI clear;
    //[SerializeField] GameObject gameResult;
    [SerializeField] GameObject attackButton;

    Enemy enemy;
    GameObject obj;

    // フィールド上のアイテム
    List<GameObject> bubbles;
    // スコア
    int gameScore;
    // 再生装置
    AudioSource audioSource;

    // ツムツム風
    List<GameObject> lineBubbles;
    LineRenderer lineRenderer;

    // プレイヤー・敵のターンかどうか
    public bool isPlayerTurn;
    public bool isEnemyTurn;
    // 終了したかどうか
    public bool isEnd;
    // 経過ターン
    public int turn = 0;
    // プレイヤーのHP
    public int playerHp = 500;
    // 攻撃力
    int powerNum = 0;
    // 消した回数
    bool isDelete = false;

    // Start is called before the first frame update
    void Start()
    {
        // Enemyスクリプトの取得
        obj = GameObject.Find("Enemy");
        enemy = obj.GetComponent<Enemy>();

        // Audio
        audioSource = GetComponent<AudioSource>();

        // 全アイテム
        bubbles = new List<GameObject>();

        // ツムツム風
        lineBubbles = new List<GameObject>();
        // 連結したアイテム上のライン
        lineRenderer = GetComponent<LineRenderer>();

        // リザルト画面非表示
        //gameResult.SetActive(false);

        // テキストを表示
        power.enabled = false;
        clear.enabled = false;

        isPlayerTurn = true;
        isEnemyTurn = false;

        isEnd = false;

        // 攻撃ボタンを非表示
        attackButton.SetActive(false);

        // アイテム生成
        SpawnItem(fieldItemCountMax);
    }

    // Update is called once per frame
    public void Update()
    {
        // isEndがtrueのとき
        if (isEnd)
        {
            // リザルト画面表示
            //gameResult.SetActive(true);

            // クリアテキストを表示
            clear.enabled = true;
            Debug.Log("くりあ");

            // Updateに入らないようにする
            enabled = false;

            // この時点でUpdateから抜ける
            return;
        }
        else if (isEnemyTurn == false)
        {// isEndがfalse・敵のターンじゃないとき

            // ゲームタイマー更新処理
            gameTimer -= Time.deltaTime;
            textGameTimer.text = "" + (int)gameTimer;

            // ゲーム終了
            if (0 > gameTimer)
            {
                // 敵の体力を減らす
                enemy.hp -= powerNum;
                Debug.Log("敵のHP：" + enemy.hp);

                textGameTimer.text = "" + 0;

                isDelete = false;

                isEnemyTurn = true;

                enemy.Attack(isEnemyTurn);

                // ターン数を加算
                turn++;

                // ターン数が2以下だったら
                if (turn <= 2)
                {
                    // タイマーを戻す
                    gameTimer = 30;
                    textGameTimer.text = "" + (int)gameTimer;

                    // 攻撃力の数値を元に戻す
                    powerNum = 0;
                    power.text = "" + 0;

                    // ダメージ数を非表示
                    power.enabled = false;
                    // 攻撃ボタンを非表示
                    attackButton.SetActive(false);
                }
                else
                {
                    textGameTimer.text = "" + 0;

                    // リザルト画面表示
                    //gameResult.SetActive(true);

                    // Updateに入らないようにする
                    enabled = false;

                    // この時点でUpdateから抜ける
                    return;
                }
            }
        }
        else
        {
            return;
        }

        // タッチ開始
        if (Input.GetMouseButtonDown(0))
        {
            GameObject hitBubble = GetHitBubble();

            // 下準備
            lineBubbles.Clear();

            // 当たり判定
            if (hitBubble)
            {
                lineBubbles.Add(hitBubble);
            }
        }
        // おしっぱなし
        else if (Input.GetMouseButton(0))
        {
            GameObject hitBubble = GetHitBubble();

            // 当たり判定あり
            if (hitBubble && lineBubbles.Count > 0)
            {
                // 距離
                GameObject pre = lineBubbles[lineBubbles.Count - 1];
                float distans =
                    Vector2.Distance(hitBubble.transform.position, pre.transform.position);

                // カラー
                bool isSameColor =
                    hitBubble.GetComponent<SpriteRenderer>().sprite == pre.GetComponent<SpriteRenderer>().sprite;

                if (isSameColor && distans <= 1.5f && !lineBubbles.Contains(hitBubble))
                {
                    // ライン追加
                    lineBubbles.Add(hitBubble);
                }
            }
        }
        // タッチ終了
        else if (Input.GetMouseButtonUp(0))
        {
            // 削除されたアイテムをクリア
            bubbles.RemoveAll(item => item == null);

            // アイテム削除
            DeleteItems(lineBubbles);

            // ラインをクリア
            lineRenderer.positionCount = 0;
            lineBubbles.Clear();
        }

        // ライン描画処理
        if (lineBubbles.Count > 1)
        {
            // 頂点数
            lineRenderer.positionCount = lineBubbles.Count;
            // ラインのポジション
            for (int i = 0; i < lineBubbles.Count; i++)
            {
                lineRenderer.SetPosition(i, lineBubbles[i].transform.position);
            }
        }

        /*
         // タッチ処理
         if(Input.GetMouseButtonUp(0))
         {
             GameObject hitBubble = GetHitBubble();

             // 削除されたアイテムをクリア
             bubbles.RemoveAll(item => item == null);

             // 何か当たり判定があれば
             if (hitBubble)
             {
                 CheckItems(hitBubble);
             }
         }
        */
    }

    // アイテム生成
    void SpawnItem(int count)
    {
        for (int i = 0; i < count; i++)
        {
            // 色ランダム
            int rnd = Random.Range(0, prefabBubbles.Count);
            // 場所ランダム
            float x = Random.Range(-2.0f, 2.0f);
            float y = Random.Range(-2.0f, 2.0f);

            // アイテム生成
            GameObject bubble =
                Instantiate(prefabBubbles[rnd], new Vector3(x, 3 + y, 0), Quaternion.identity);

            // 内部データ追加
            bubbles.Add(bubble);
        }
    }

    // 引数と同じ色のアイテムを削除する
    void DeleteItems(List<GameObject> checkItems)
    {
        // 削除可能数に達していなかったらなにもしない
        if (checkItems.Count < deleteCount) return;

        Debug.Log(checkItems.Count);

        // 削除してスコア加算
        List<GameObject> destroyItems = new List<GameObject>();
        foreach (var item in checkItems)
        {
            // 被りなしの削除したアイテムをカウント
            if (!destroyItems.Contains(item))
            {
                destroyItems.Add(item);
            }

            // 削除
            Destroy(item);
        }

        // 実際に削除した分生成してスコア加算
        SpawnItem(destroyItems.Count);
        gameScore += destroyItems.Count * 100;

        // ダメージ数を表示
        power.enabled = true;

        // 攻撃力に繋げた長さ * 30を加算
        powerNum += checkItems.Count * 40;
        power.text = "" + (int)powerNum;

        isDelete = true;

        if(isDelete)
        {
            attackButton.SetActive(true);
        }

        // スコア表示更新
        //textGameScore.text = "" + gameScore;

        // SE再生
        //audioSource.PlayOneShot(seBubble);
    }

    // 同じ色のアイテムを返す
    List<GameObject> GetSameItems(GameObject target)
    {
        List<GameObject> ret = new List<GameObject>();

        foreach (var item in bubbles)
        {
            // アイテムがない、同じアイテム、違う色、距離が遠い場合はスキップ
            if (!item || target == item) continue;

            if (item.GetComponent<SpriteRenderer>().sprite
                != target.GetComponent<SpriteRenderer>().sprite)
            {
                continue;
            }

            float distance =
                Vector2.Distance(target.transform.position, item.transform.position);

            if (distance > 1.1f) continue;

            // ここまで来たらアイテム追加
            ret.Add(item);
        }

        return ret;
    }

    // 引数と同じ色のアイテムを探す
    void CheckItems(GameObject target)
    {
        // このアイテムと同じ色を追加する
        List<GameObject> checkItems = new List<GameObject>();
        // 自分を追加
        checkItems.Add(target);

        // チェック済のインデックス
        int checkIndex = 0;

        // checkItemsの最大値までループ
        while (checkIndex < checkItems.Count)
        {
            // 隣接する同じ色を取得
            List<GameObject> sameItems = GetSameItems(checkItems[checkIndex]);
            // チェック済のインデックスを進める
            checkIndex++;

            // まだ追加されていないアイテムを追加
            foreach (var item in sameItems)
            {
                if (checkItems.Contains(item)) continue;
                checkItems.Add(item);
            }
        }

        // 削除
        DeleteItems(checkItems);
    }

    // マウスポジションに当たり判定があったバブルを返す
    GameObject GetHitBubble()
    {
        GameObject ret = null;

        // スクリーン座標からワールド座標に変換
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit2d = Physics2D.Raycast(worldPoint, Vector2.zero);

        // 当たり判定あり
        if (hit2d)
        {
            // 画像が設定されていたらバブルアイテムと判断する
            SpriteRenderer spriteRenderer =
                hit2d.collider.gameObject.GetComponent<SpriteRenderer>();

            if (spriteRenderer)
            {
                ret = hit2d.collider.gameObject;
            }
        }

        return ret;
    }

    // リトライボタン
    /*public void OnClickRetry()
    {
        SceneManager.LoadScene("SamegamePuzzleScene");
    }*/

    public void Click()
    {
        // ターンを終了
        gameTimer = 0;
    }
}
