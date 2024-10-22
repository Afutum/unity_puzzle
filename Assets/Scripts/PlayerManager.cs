using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // hp
    int hp = 1000;
    public int Hp {  get { return hp; } }
    int divHp;
    public int DivHp { get {  return divHp; } }
    // 攻撃力
    int power = 0;
    public int Power { get { return power; } }

    EnemyManager enemyManager;
    Animator animator;

    Action completeAction;

    UIManager uiManager;

    AudioSource audioSource;

    PlayerHPbar playerHPbar;

    [SerializeField] AudioClip attackSE;

    public Action CompleteAction {  get { return completeAction; } }

    // Start is called before the first frame update
    void Start()
    {
        enemyManager = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
        animator = GetComponent<Animator>();

        playerHPbar = GameObject.Find("PlayerHPbar").GetComponent<PlayerHPbar>();

        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();

        audioSource = GetComponent<AudioSource>();

        playerHPbar.MaxHp(hp);

        divHp = hp / 2;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // プレイヤーのhpを減らす
    public void SubPlayerHp(int enemAttackPower)
    {
        hp -= enemAttackPower;

        

        Debug.Log(hp);
    }

    // 攻撃力のリセット
    public void ResetPower()
    {
        power = 0;
    }

    // 攻撃力加算
    public void AttackPower(int powerNum)
    {
        power += powerNum;
    }

    // 攻撃
    public void Attack()
    {
        uiManager.AttackPower();
        StartCoroutine(enemyManager.SubHp(power));
    }

    // 攻撃のアニメーション
    public void PushButtonAnim(Action complete)
    {
        // 攻撃アニメーションを開始
        animator.SetBool("attack", true);

        // 敵のダメージアニメーションを開始
        enemyManager.GetEnemyAnimator().SetAnimDamage();

        // 引数のアクションを代入
        completeAction = complete;

        // 0.7秒後にアニメーションを終了
        Invoke(nameof(ResetAnim), 0.7f);

        audioSource.PlayOneShot(attackSE);
    }

    // アニメーションを元に戻す
    public void ResetAnim()
    {
        // 攻撃アニメーションを終了
        animator.SetBool("attack", false);

        completeAction();

        // アクションをなしにする
        completeAction = null;
    }

    // ダメージアニメーション
    public void SetDamageAnim()
    {
        animator.SetTrigger("Damage");
        playerHPbar.NowHp(hp);
    }

    // 死ぬアニメーション
    public void SetDieAnim()
    {
        animator.SetTrigger("Die");
    }

    // クラクラアニメーション
    public void SetDizzyAnim()
    {
        animator.SetBool("Dizzy", true);
    }
}
