using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] public Scrollbar enemyHp;

    [SerializeField] protected GameObject[] enemy;

    public GameObject[] Enemy {  get { return enemy; } }

    protected GameDirector gameDirector;
    UIManager uiManager;
    PlayerManager playerManager;

    Animator animator;
    Action completeAtk;

    EnemyAnimator[] enemyAnim;

    protected EnemyHPbar enemyHPbar;

    AudioSource audioSource;

    [SerializeField] AudioClip attackSE;

    public EnemyAnimator GetEnemyAnimator()
    {
        return enemyAnim[gameDirector.RoundCnt];
    }

    public Action CompleteAtk { get { return completeAtk; } }

    // 体力
    protected int[] hp;

    public int[] Hp { get { return hp; } }

    // 攻撃力
    protected int[] baseDamege;

    // プレイヤーに与えるダメージ
    int attackPower = 0;
    public int AttackPower { get { return attackPower; } }

    // ダメージ変動率の上限値
    const float volatilityAtcMax = 1.2f;
    // ダメージ変動率の下限値
    const float volatilityAtcMin = 0.7f;

    int divHp = 0;

    bool isEnemyChange;

    public bool IsEnemyChange {  get { return isEnemyChange; } }

    public enum EnemyStatus
    {
        None,           // なにもない
        WaitAttack,     // 攻撃待機
        AttackComplete, // 攻撃完了
        Change,         // 敵の切り替え
        Die             // 死亡
    }

    public EnemyStatus EnemyCurrentStatus { get; set; }

    private void Start()
    {
        SetStart();
    }

    // Start is called before the first frame update
    protected void SetStart()
    {
        // GameDirectorスクリプトの取得
        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        playerManager = GameObject.Find("player").GetComponent<PlayerManager>();

        enemyHPbar = GameObject.Find("HPbar").GetComponent<EnemyHPbar>();

        enemyAnim = new EnemyAnimator[enemy.Length];

        for (int i = 0; i < enemy.Length; i++)
        {
            enemyAnim[i] = enemy[i].GetComponent<EnemyAnimator>();
        }

        // 配列を敵数分作成
        hp = new int[enemy.Length];
        baseDamege = new int[enemy.Length];

        for (int i = 0; i < enemy.Length; i++)
        {
            enemy[i].SetActive(false);
        }

        enemy[gameDirector.RoundCnt].SetActive(true);

        SetEnemyDamage();

        enemyHPbar.MaxHp(hp[0]);

        // hpの半分を格納
        divHp = hp[gameDirector.RoundCnt] / 2;

        EnemyCurrentStatus = EnemyStatus.None;

        audioSource = GetComponent<AudioSource>();
    }

    virtual protected void SetEnemyDamage()
    {
        for (int i = 0; i < hp.Length; i++)
        {
            // hp・攻撃力の生成
            hp[i] = 300 + (100 * i);
            // 70〜200の間からランダムで抽出
            baseDamege[i] = UnityEngine.Random.Range(70, 201);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    virtual protected void DiePlayer()
    {
        gameDirector.LosePlayer();

        // ゲーム終了
        gameDirector.GameEnd();
    }

    // 敵の攻撃
    public IEnumerator Attack(bool isAttack)
    {
        // Attackボタンを非表示
        gameDirector.DisplayAttackBtn(false);

        // 引数の値がtrueだったら
        if (isAttack == true)
        {
            // 攻撃力 * 変動率上限・下限値の合計からintに変換してランダムで抽出
            attackPower = (int)UnityEngine.Random.Range(
                baseDamege[gameDirector.RoundCnt] * volatilityAtcMin, // 攻撃力 * 下限値
                baseDamege[gameDirector.RoundCnt] * volatilityAtcMax  // 攻撃力 * 上限値
            );

            Debug.Log(attackPower);

            // プレイヤーの体力を減らす
            playerManager.SubPlayerHp(attackPower);
        }

        // 攻撃アニメーション開始
        enemyAnim[gameDirector.RoundCnt].SetAnimAttack();

        // 攻撃待機
        EnemyCurrentStatus = EnemyStatus.WaitAttack;

        // 1秒処理を遅らせる
        yield return new WaitForSeconds(0.5f);

        if (EnemyCurrentStatus != EnemyStatus.Die)
        {
            audioSource.PlayOneShot(attackSE);
            uiManager.EnemyPower();

            // プレイヤーのダメージアニメーション開始
            playerManager.SetDamageAnim();
        }

        // プレイヤーのHPが0以下なら
        if (playerManager.Hp < 0)
        {
            // プレイヤーの死ぬアニメーションを追加
            playerManager.SetDieAnim();

            DiePlayer();
        }
        // プレイヤーのHPが半分以下なら
        else if (playerManager.Hp < playerManager.DivHp)
        {
            // プレイヤーのクラクラアニメーションを開始
            playerManager.SetDizzyAnim();
        }

        // ステータスを攻撃完了に変更
        EnemyCurrentStatus = EnemyStatus.AttackComplete;
    }

    // 敵のhpを減らす
    public IEnumerator SubHp(int attackPower)
    {
        if (enemy.Length <= enemy.Length)
        {
            // hpを引数分減らす
            hp[gameDirector.RoundCnt] -= attackPower;

            enemyHPbar = GameObject.Find("HPbar").GetComponent<EnemyHPbar>();
            // HPバーを現在のHP分に減らす
            enemyHPbar.NowHp(hp[gameDirector.RoundCnt]);

            // HPが0以下になったら
            if (hp[gameDirector.RoundCnt] <= 0)
            {
                EnemyCurrentStatus = EnemyStatus.Die;

                enemyAnim[gameDirector.RoundCnt].ResetDizzyAnim();

                // 死んだアニメーションを開始
                enemyAnim[gameDirector.RoundCnt].SetAnimDie();

                // 2.5秒処理を遅らせる
                yield return new WaitForSeconds(2.5f);

                // このスクリプトがついているオブジェクトを非表示
                enemy[gameDirector.RoundCnt].SetActive(false);

                // 最後の敵のhpが0以下になった場合
                if (enemy.Last() && hp.Last() <= 0)
                {
                    gameDirector.WinPlayer();
 
                    TerminateBattle();
                }
                // 最後の敵以外のとき
                else if (enemy.Last().activeSelf == false)
                {
                    // ラウンドを進める
                    gameDirector.CountRound();

                    // roundCnt番目の敵を表示
                    enemy[gameDirector.RoundCnt].SetActive(true);

                    EnemyCurrentStatus = EnemyStatus.None;

                    gameDirector.ResetAttackButton();

                    enemyHPbar = GameObject.Find("HPbar").GetComponent<EnemyHPbar>();

                    // HPバーの最大値を設定
                    enemyHPbar.MaxHp(hp[gameDirector.RoundCnt]);
                }
                Debug.Log(gameDirector.RoundCnt);
            }
            // 現在の敵のhpが半分以下かつ0以上なら
            else if (hp[gameDirector.RoundCnt] < divHp
                && hp[gameDirector.RoundCnt] > 0)
            {
                // クラクラアニメーションを開始
                enemyAnim[gameDirector.RoundCnt].SetAnimDizzy();
            }
        }
    }

    virtual public void TerminateBattle()
    {
        StartCoroutine(NetworkManager.Instance.RegistClearStage(StageSelectManager.stageId,
                            result => {
                                gameDirector.GameEnd();
                            }));
    }



    public void ResetEnemyChange()
    {
        isEnemyChange = false;
    }
}
