using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    GameDirector gameDirector;
    GameObject obj;

    public int hp = 0;

    // Start is called before the first frame update
    void Start()
    {
        obj = GameObject.Find("GameDirector");
        gameDirector = obj.GetComponent<GameDirector>();

        hp = 300;
    }

    // Update is called once per frame
    void Update()
    {
        // HPが0以下になったら
        if(hp <= 0)
        {
            // HPを0に固定
            hp = 0;
            // このスクリプトがついているオブジェクトを非表示
            this.gameObject.SetActive(false);
            // ゲームを終了
            gameDirector.isEnd = true;
        }
    }

    public void Attack(bool isAttack)
    {
        // 引数の値がtrueだったら
        if (isAttack == true)
        {
            // 敵のターンに変更
            gameDirector.isPlayerTurn = false;
            gameDirector.isEnemyTurn = true;

            // プレイヤーの体力を減らす
            gameDirector.playerHp -= 300;
            Debug.Log(gameDirector.playerHp);
        }

        // プレイヤーのターンに戻す
        gameDirector.isPlayerTurn = true;
        gameDirector.isEnemyTurn = false;
    }
}
