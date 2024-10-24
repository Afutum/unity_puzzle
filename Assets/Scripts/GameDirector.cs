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
    [SerializeField] public float gameTimer;
    // フィールドのアイテム総数
    [SerializeField] int fieldItemCountMax;
    // 削除できるアイテム数
    [SerializeField] int deleteCount;
    // コンボカウント
    [SerializeField] int comboCount;

    EnemyManager enemy;
    UIManager uiManager;
    PlayerManager player;

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
    public bool IsPlayerTurn { get { return isPlayerTurn; } }

    bool isEnemyTurn;
    public bool IsEnemyTurn { get { return isEnemyTurn; } }
    // attackボタンを押したかどうか
    bool isAttackBtn;
    // 終了したかどうか
    public bool isEnd;
    public bool IsEnd { get { return isEnd; } }
    // 経過ラウンド
    int roundCnt = 0;
    public int RoundCnt { get { return roundCnt; } }
    // 攻撃力
    int powerNum = 0;
    public int PowerNum { get { return powerNum; } }
    // 攻撃中かどうか
    bool isDelete = false;
    public bool IsDelete { get { return isDelete; } }

    bool isClear;
    public bool IsClear { get { return isClear; } }

    bool isGameOver;
    public bool IsGameOver { get { return isGameOver; } }

    public float GameTimer { get { return gameTimer; } }

    int hideRoundCnt = 0;

    GameObject bubble;

    [SerializeField] AudioClip chainSE;

    // Start is called before the first frame update
    void Start()
    {
        // Enemyスクリプトの取得
        enemy = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();

        // UIManagerスクリプトの取得
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();

        player = GameObject.Find("player").GetComponent<PlayerManager>();

        audioSource = GetComponent<AudioSource>();

        // Audio
        //audioSource = GetComponent<AudioSource>();

        // 全アイテム
        bubbles = new List<GameObject>();

        // ツムツム風
        lineBubbles = new List<GameObject>();
        // 連結したアイテム上のライン
        lineRenderer = GetComponent<LineRenderer>();

        // リザルト画面非表示
        //gameResult.SetActive(false);

        isPlayerTurn = false;
        isEnemyTurn = true;

        isEnd = false;

        isClear = false;
        isGameOver = false;

        // アイテム生成
        SpawnItem(fieldItemCountMax);

        ChangTurn();
    }

    // Update is called once per frame
    public void Update()
    {
        // isEndがtrueのとき
        if (isEnd)
        {
            Debug.Log("くりあ");

            // この時点でUpdateから抜ける
            return;
        }
        else if (isPlayerTurn == true && 
            enemy.EnemyCurrentStatus == EnemyManager.EnemyStatus.None)
        {// isEndがfalse・敵のターンじゃないとき・敵の状態がNoneのとき

            if (hideRoundCnt == 0)
            {
                uiManager.RoundUpText(roundCnt);
                hideRoundCnt++;
            }

            // 操作時間更新処理
            gameTimer -= Time.deltaTime;

            // 操作時間終了もしくは攻撃ボタンを押された場合
            if (gameTimer < 0 || isAttackBtn == true)
            {
                if (player.Power > 0)
                {
                    if (player.CompleteAction == null)
                    {
                        player.Attack();

                        player.PushButtonAnim(() =>
                        {
                            // 攻撃を終わらせる
                            isDelete = false;

                            // ラウンド数が敵より少なかったら
                            if (roundCnt < enemy.Enemy.Length)
                            {
                                // タイマーを戻す
                                gameTimer = 30;

                                // 攻撃力の数値を元に戻す
                                player.ResetPower();

                                if (enemy.Hp[roundCnt] > 0)
                                {
                                    ChangTurn();
                                }
                                else
                                {
                                    isAttackBtn = true;
                                }
                            }
                            else
                            {
                                // リザルト画面表示
                                //gameResult.SetActive(true);

                                NetworkManager.Instance.RegistClearStage(StageSelectManager.stageId,
                                result => {
                                    GameEnd();
                                });

                                // Updateに入らないようにする
                                enabled = false;

                                // この時点でUpdateから抜ける
                                return;
                            }
                        });
                    }
                }
                else
                {
                    // 攻撃を終わらせる
                    isDelete = false;

                    // ラウンド数が2以下だったら
                    if (roundCnt <= enemy.Enemy.Length)
                    {
                        if (enemy.Hp[0] > 0)
                        {
                            ChangTurn();
                        }

                        // タイマーを戻す
                        gameTimer = 30;

                        // 攻撃力の数値を元に戻す
                        player.ResetPower();
                    }
                    else
                    {
                        // リザルト画面表示
                        //gameResult.SetActive(true);

                        isClear = true;

                        NetworkManager.Instance.RegistClearStage(StageSelectManager.stageId,
                            result => {
                                GameEnd();
                            });

                        // Updateに入らないようにする
                        enabled = false;

                        // この時点でUpdateから抜ける
                        return;
                    }
                }
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
                        audioSource.PlayOneShot(chainSE);

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
        else
        {
            // 敵の状態が攻撃完了なら
            if (enemy.EnemyCurrentStatus == EnemyManager.EnemyStatus.AttackComplete)
            {
                // ターンを変更
                ChangTurn();

                // 敵の状態をなしにする
                enemy.EnemyCurrentStatus = EnemyManager.EnemyStatus.None;
            }
            // 敵の状態がなにもないなら
            else if (enemy.EnemyCurrentStatus == EnemyManager.EnemyStatus.None)
            {
                Debug.Log(isEnemyTurn);
                // 敵の攻撃を開始
                StartCoroutine(enemy.Attack(isEnemyTurn));
            }
            /*else if(enemy.IsEnemyChange == true)
            {
                enemy.ResetEnemyChange();

                // 敵の状態をなしにする
                enemy.EnemyCurrentStatus = EnemyManager.EnemyStatus.None;

                isPlayerTurn = true;
                isEnemyTurn = false;
            }*/
        }
    }

    // シーンのロードを遅らせる
    private IEnumerator waitAndLoadScene()
    {
        yield return new WaitForSeconds(1.5f);

        if (isClear)
        {
            // リザルト画面
            SceneManager.LoadScene("Clear");
        }
        else if (isGameOver)
        {
            // リザルト画面
            SceneManager.LoadScene("GameOver");
        }
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
            bubble =
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

        // 攻撃力に繋げた長さ * 30を加算
        //powerNum += checkItems.Count * 40;

        player.AttackPower(checkItems.Count * 40);

        // 攻撃中に変更
        isDelete = true;

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

    // ゲーム終了
    public void GameEnd()
    {
        isEnd = true;

        StartCoroutine(waitAndLoadScene());
    }

    // 攻撃終了
    public void EndAttack()
    {
        // 攻撃を終わらせる
        isDelete = false;
        // Attackボタン表示されているか
        isAttackBtn = true;
    }

    // ラウンドカウント
    public void CountRound()
    {
        roundCnt++;

        if (isEnd == false)
        {
            uiManager.RoundUpText(roundCnt);
        }
    }

    // 攻守ターンの変更
    public void ChangTurn()
    {
        gameTimer = 30;

        if (isPlayerTurn)
        {
            isPlayerTurn = false;
            isEnemyTurn = true;
        }
        else
        {
            isPlayerTurn = true;
            isEnemyTurn = false;
        }

        StartCoroutine(uiManager.ChangeText());
    }

    public IEnumerator WaitChangeTurn()
    {
        yield return new WaitForSeconds(3);

        ChangTurn();
    }

    // ボタンの表示・非表示
    public void DisplayAttackBtn(bool isAttack)
    {
        isAttackBtn = isAttack;
    }

    // ボタン押した判定をリセット
    public void ResetAttackButton()
    {
        isAttackBtn = false;
    }

    public void WinPlayer()
    {
        isClear = true;
    }

    public void LosePlayer()
    {
        isGameOver = true;
    }
}
