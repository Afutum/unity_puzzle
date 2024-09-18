using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // hp
    int hp = 200;
    public int Hp {  get { return hp; } }
    int divHp;
    public int DivHp { get {  return divHp; } }
    // �U����
    int power = 0;
    public int Power { get { return power; } }

    EnemyManager enemyManager;
    Animator animator;

    Action completeAction;

    public Action CompleteAction {  get { return completeAction; } }

    // Start is called before the first frame update
    void Start()
    {
        enemyManager = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
        animator = GetComponent<Animator>();

        divHp = hp / 2;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // �v���C���[��hp�����炷
    public void SubPlayerHp(int enemAttackPower)
    {
        hp -= enemAttackPower;

        Debug.Log(hp);
    }

    // �U���͂̃��Z�b�g
    public void ResetPower()
    {
        power = 0;
    }

    // �U���͉��Z
    public void AttackPower(int powerNum)
    {
        power += powerNum;
    }

    // �U��
    public void Attack()
    {
        StartCoroutine(enemyManager.SubHp(power));
    }

    // �U���̃A�j���[�V����
    public void PushButtonAnim(Action complete)
    {
        // �U���A�j���[�V�������J�n
        animator.SetBool("attack", true);

        // �G�̃_���[�W�A�j���[�V�������J�n
        enemyManager.GetEnemyAnimator().SetAnimDamage();

        // �����̃A�N�V��������
        completeAction = complete;

        // 0.7�b��ɃA�j���[�V�������I��
        Invoke(nameof(ResetAnim), 0.7f);
    }

    // �A�j���[�V���������ɖ߂�
    public void ResetAnim()
    {
        // �U���A�j���[�V�������I��
        animator.SetBool("attack", false);

        completeAction();

        // �A�N�V�������Ȃ��ɂ���
        completeAction = null;
    }

    // �_���[�W�A�j���[�V����
    public void SetDamageAnim()
    {
        animator.SetTrigger("Damage");
    }

    // ���ʃA�j���[�V����
    public void SetDieAnim()
    {
        animator.SetTrigger("Die");
    }

    // �N���N���A�j���[�V����
    public void SetDizzyAnim()
    {
        animator.SetBool("Dizzy", true);
    }
}