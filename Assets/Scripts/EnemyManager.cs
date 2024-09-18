using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] public Scrollbar enemyHp;

    [SerializeField] GameObject[] enemy;

    public GameObject[] Enemy {  get { return enemy; } }

    GameDirector gameDirector;
    UIManager uiManager;
    PlayerManager playerManager;

    Animator animator;
    Action completeAtk;

    EnemyAnimator[] enemyAnim;

    public EnemyAnimator GetEnemyAnimator()
    {
        return enemyAnim[gameDirector.RoundCnt];
    }

    public Action CompleteAtk { get { return completeAtk; } }

    // �̗�
    int[] hp;

    public int[] Hp { get { return hp; } }

    // �U����
    int[] baseDamege;

    // �v���C���[�ɗ^����_���[�W
    int attackPower = 0;
    // �_���[�W�ϓ����̏���l
    const float volatilityAtcMax = 1.2f;
    // �_���[�W�ϓ����̉����l
    const float volatilityAtcMin = 0.7f;

    int divHp = 0;

    bool isEnemyChange;

    public bool IsEnemyChange {  get { return isEnemyChange; } }

    public enum EnemyStatus
    {
        None,           // �Ȃɂ��Ȃ�
        WaitAttack,     // �U���ҋ@
        AttackComplete, // �U������
        Die             // ���S
    }

    public EnemyStatus EnemyCurrentStatus { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        // GameDirector�X�N���v�g�̎擾
        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
        //uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        playerManager = GameObject.Find("player").GetComponent<PlayerManager>();

        enemyAnim = new EnemyAnimator[enemy.Length];

        for (int i = 0; i < enemy.Length; i++)
        {
            enemyAnim[i] = enemy[i].GetComponent<EnemyAnimator>();
        }

        // �z���G�����쐬
        hp = new int[enemy.Length];
        baseDamege = new int[enemy.Length];

        for (int i = 0; i < enemy.Length; i++)
        {
            enemy[i].SetActive(false);
        }

        enemy[gameDirector.RoundCnt].SetActive(true);

        for (int i = 0; i < hp.Length; i++)
        {
            // hp�E�U���͂̐���
            hp[i] = 300;
            // 70�`200�̊Ԃ��烉���_���Œ��o
            baseDamege[i] = UnityEngine.Random.Range(70,201);
        }

        // hp�̔������i�[
        divHp = hp[gameDirector.RoundCnt] / 2;

        EnemyCurrentStatus = EnemyStatus.None;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // �G�̍U��
    public IEnumerator Attack(bool isAttack)
    {
        // Attack�{�^�����\��
        gameDirector.DisplayAttackBtn(false);

        // �����̒l��true��������
        if (isAttack == true)
        {
            // �U���� * �ϓ�������E�����l�̍��v����int�ɕϊ����ă����_���Œ��o
            attackPower = (int)UnityEngine.Random.Range(
                baseDamege[gameDirector.RoundCnt] * volatilityAtcMin, // �U���� * �����l
                baseDamege[gameDirector.RoundCnt] * volatilityAtcMax  // �U���� * ����l
            );

            Debug.Log(attackPower);

            // �v���C���[�̗̑͂����炷
            playerManager.SubPlayerHp(attackPower);
        }

        // �U���A�j���[�V�����J�n
        enemyAnim[gameDirector.RoundCnt].SetAnimAttack();

        // �U���ҋ@
        EnemyCurrentStatus = EnemyStatus.WaitAttack;

        // 1�b������x�点��
        yield return new WaitForSeconds(1);

        if (EnemyCurrentStatus != EnemyStatus.Die)
        {
            // �v���C���[�̃_���[�W�A�j���[�V�����J�n
            playerManager.SetDamageAnim();
        }

        // �v���C���[��HP��0�ȉ��Ȃ�
        if (playerManager.Hp < 0)
        {
            // �v���C���[�̎��ʃA�j���[�V������ǉ�
            playerManager.SetDieAnim();

            // �Q�[���I��
            gameDirector.GameEnd();
        }
        // �v���C���[��HP�������ȉ��Ȃ�
        else if (playerManager.Hp < playerManager.DivHp)
        {
            // �v���C���[�̃N���N���A�j���[�V�������J�n
            playerManager.SetDizzyAnim();
        }

        // �X�e�[�^�X���U�������ɕύX
        EnemyCurrentStatus = EnemyStatus.AttackComplete;
    }

    // �G��hp�����炷
    public IEnumerator SubHp(int attackPower)
    {
        if (enemy.Length <= enemy.Length)
        {
            // hp�����������炷
            hp[gameDirector.RoundCnt] -= attackPower;

            // HP��0�ȉ��ɂȂ�����
            if (hp[gameDirector.RoundCnt] <= 0)
            {
                EnemyCurrentStatus = EnemyStatus.Die;

                enemyAnim[gameDirector.RoundCnt].ResetDizzyAnim();

                // ���񂾃A�j���[�V�������J�n
                enemyAnim[gameDirector.RoundCnt].SetAnimDie();

                // 2.5�b������x�点��
                yield return new WaitForSeconds(2.5f);

                // ���̃X�N���v�g�����Ă���I�u�W�F�N�g���\��
                enemy[gameDirector.RoundCnt].SetActive(false);

                if (enemy.Last() && hp.Last() <= 0)
                {
                    gameDirector.GameEnd();
                }
                else if (enemy.Last().activeSelf == false)
                {
                    // ���E���h��i�߂�
                    gameDirector.CountRound();

                    hp[gameDirector.RoundCnt] = 300;

                    // roundCnt�Ԗڂ̓G��\��
                    enemy[gameDirector.RoundCnt].SetActive(true);

                    EnemyCurrentStatus = EnemyStatus.None;

                    gameDirector.ResetAttackButton();
                }
                Debug.Log(gameDirector.RoundCnt);
            }
            // ���݂̓G��hp�������ȉ�����0�ȏ�Ȃ�
            else if (hp[gameDirector.RoundCnt] < divHp
                && hp[gameDirector.RoundCnt] > 0)
            {
                // �N���N���A�j���[�V�������J�n
                enemyAnim[gameDirector.RoundCnt].SetAnimDizzy();
            }
        }
    }

    public void ResetEnemyChange()
    {
        isEnemyChange = false;
    }
}
