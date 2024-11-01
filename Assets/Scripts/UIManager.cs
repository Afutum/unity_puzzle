using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // UI
    //[SerializeField] TextMeshProUGUI textGameScore;
    [SerializeField] TextMeshProUGUI textGameTimer;
    [SerializeField] TextMeshProUGUI textComboCounter;
    [SerializeField] TextMeshProUGUI combo;
    [SerializeField] TextMeshProUGUI power;
    [SerializeField] TextMeshProUGUI powerBack;
    [SerializeField] TextMeshProUGUI enemyPower;
    [SerializeField] TextMeshProUGUI enemyPowerBack;
    [SerializeField] TextMeshProUGUI clear;
    [SerializeField] GameObject attackButton;
    [SerializeField] GameObject playerTurnText;
    [SerializeField] GameObject enemyTurnText;
    [SerializeField] GameObject roundText;
    [SerializeField] Text roundUpText;
    [SerializeField] GameObject clearObj;
    [SerializeField] GameObject gameOver;

    GameDirector gameDirector;
    EnemyManager enemyManager;
    PlayerManager player;

    GameObject lastEnemy;

    public void Awake()
    {
        playerTurnText.SetActive(false);
        enemyTurnText.SetActive(true);

        clearObj.SetActive(false);
        gameOver.SetActive(false);

        // GameDirectorの取得
        GameObject gameObj = GameObject.Find("GameDirector");

        if( gameObj != null ) 
        { 
            gameDirector = gameObj.GetComponent<GameDirector>(); 
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // テキストを表示
        power.enabled = false;
        powerBack.enabled = false;
        clear.enabled = false;
        enemyPower.enabled = false;
        enemyPowerBack.enabled = false;

        // 攻撃ボタンを非表示
        attackButton.SetActive(false);

        roundText.SetActive(false);

        // EnemyManagerの取得
        GameObject obj = GameObject.Find("EnemyManager");

        if( obj != null )
        {
            enemyManager = obj.GetComponent<EnemyManager>();
            // PlayerManagerの取得
            player = GameObject.Find("player").GetComponent<PlayerManager>();
            //lastEnemy = enemyManager.Enemy.Last();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(gameDirector == null)
        {
            return;
        }

        if (gameDirector.IsDelete)
        {// 攻撃中なら
            // アタックボタンを表示
            attackButton.SetActive(true);
        }

        if (gameDirector.IsEnd)
        {// ゲームが終了していたら
            // クリアテキストを表示
            clear.enabled = true;
        }
        else if (gameDirector.IsEnemyTurn == false)
        {
            //StartCoroutine(HiddenText());

            // タイマーを更新
            textGameTimer.text = "" + (int)gameDirector.GameTimer;

            if (gameDirector.GameTimer <= 0)
            {// タイマーが0以下になったら

                // タイマーが0以下にならないようにする
                textGameTimer.text = "" + 0;

                if (gameDirector.RoundCnt <= enemyManager.Enemy.Length)
                {
                    // 現在の時間を設定
                    textGameTimer.text = "" + (int)gameDirector.gameTimer;

                    // 攻撃力の数値を元に戻す
                    power.text = "" + 0;
                    // 攻撃ボタンを非表示
                    power.enabled = false;
                    powerBack.enabled = false;
                    // ダメージ数を非表示
                    attackButton.SetActive(false);
                }
                else
                {
                    // Updateに入らないようにする
                    enabled = false;

                    // この時点でUpdateから抜ける
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }

    // Attackボタン押した時の処理
    public void AttackButton()
    {
        // プレイヤーの攻撃を終了
        gameDirector.EndAttack();

        //StartCoroutine(HiddenText());

        // 攻撃力の数値を元に戻す
        power.text = "" + 0;
        // 攻撃ボタンを非表示
        attackButton.SetActive(false);
    }

    /*public void TurnText()
    {
        if (gameDirector.IsPlayerTurn)
        {
            playerTurnText.enabled = true;
            enemyTurnText.enabled = false;
        }
        else if (gameDirector.IsEnemyTurn)
        {
            playerTurnText.enabled = false;
            enemyTurnText.enabled = true;
        }
    }*/

    public IEnumerator ChangeText()
    {
        if (gameDirector.IsPlayerTurn)
        {
            playerTurnText.SetActive(true);
            enemyTurnText.SetActive(false);
        } 
        else if (gameDirector.IsEnemyTurn)
        {
            playerTurnText.SetActive(false);
            enemyTurnText.SetActive(true);
        }

        yield return new WaitForSeconds(1);

        if (gameDirector.IsPlayerTurn)
        {
            playerTurnText.SetActive(false);
        }
        else if (gameDirector.IsEnemyTurn)
        {
            enemyTurnText.SetActive(false);
        }
    }

    public void RoundUpText(int round)
    {
        if (round == 0)
        {
            roundText.SetActive(true);

            roundUpText.text = "Round " + (round + 1);

            enemyTurnText.SetActive(false);
        }
        else if (enemyManager.Enemy.Last() == enemyManager.Enemy[gameDirector.RoundCnt])
        {
            roundText.SetActive(true);

            roundUpText.text = "Last Round";
        }
        else
        {
            roundText.SetActive(true);

            roundUpText.text = "Round " + (round + 1);
        }

        StartCoroutine(HideRoundText());
    }

    public IEnumerator HideRoundText()
    {
        yield return new WaitForSeconds(1);

        roundText.SetActive(false);
    }

    /*public void Change()
    {
        gameDirector.
    }*/

    public void Clear()
    {
        clearObj.SetActive(true);
    }

    public void GameOver()
    {
        gameOver.SetActive(true);
    }

    public void AllClear()
    {
        roundText.SetActive(false);
        attackButton.SetActive(false);
        textGameTimer.enabled = false;
        power.enabled = false;
        powerBack.enabled = false;
        clear.enabled = false;
        playerTurnText.SetActive(false);
        enemyTurnText.SetActive(false);
        enemyPower.enabled = false;
        enemyPowerBack.enabled = false;
    }

    public void AttackPower()
    {
        // ダメージ数を表示
        power.enabled = true;
        powerBack.enabled = true;
        power.text = "" + (int)player.Power;
        powerBack.text = "" + (int)player.Power;

        StartCoroutine(HideAttackPower());
    }

    public IEnumerator HideAttackPower()
    {
        yield return new WaitForSeconds(0.5f);

        power.enabled = false;
        powerBack.enabled = false;
    }

    public void EnemyPower()
    {
        enemyPower.enabled = true;
        enemyPowerBack.enabled = true;
        enemyPower.text = "" + (int)enemyManager.AttackPower;
        enemyPowerBack.text = "" + (int)enemyManager.AttackPower;

        StartCoroutine(HideAttackEnemyPower());
    }

    public IEnumerator HideAttackEnemyPower()
    {
        yield return new WaitForSeconds(0.5f);

        enemyPower.enabled = false;
        enemyPowerBack.enabled = false;
    }
}
