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
    [SerializeField] TextMeshProUGUI clear;
    [SerializeField] GameObject attackButton;
    [SerializeField] GameObject playerTurnText;
    [SerializeField] GameObject enemyTurnText;
    [SerializeField] GameObject roundText;
    [SerializeField] Text roundUpText;

    GameDirector gameDirector;
    EnemyManager enemyManager;
    PlayerManager player;

    GameObject lastEnemy;

    public void Awake()
    {
        playerTurnText.SetActive(false);
        enemyTurnText.SetActive(true);

        // GameDirector�̎擾
        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // �e�L�X�g��\��
        power.enabled = false;
        clear.enabled = false;

        // �U���{�^�����\��
        attackButton.SetActive(false);

        roundText.SetActive(false);

        // EnemyManager�̎擾
        enemyManager = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
        // PlayerManager�̎擾
        player = GameObject.Find("player").GetComponent<PlayerManager>();

        lastEnemy = enemyManager.Enemy.Last();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameDirector.IsDelete)
        {// �U�����Ȃ�
            // �_���[�W����\��
            power.enabled = true;
            power.text = "" + (int)player.Power;
            // �A�^�b�N�{�^����\��
            attackButton.SetActive(true);
        }

        if (gameDirector.IsEnd)
        {// �Q�[�����I�����Ă�����
            // �N���A�e�L�X�g��\��
            clear.enabled = true;
        }
        else if (gameDirector.IsEnemyTurn == false)
        {
            //StartCoroutine(HiddenText());

            // �^�C�}�[���X�V
            textGameTimer.text = "" + (int)gameDirector.GameTimer;

            if (gameDirector.GameTimer <= 0)
            {// �^�C�}�[��0�ȉ��ɂȂ�����

                // �^�C�}�[��0�ȉ��ɂȂ�Ȃ��悤�ɂ���
                textGameTimer.text = "" + 0;

                if (gameDirector.RoundCnt <= enemyManager.Enemy.Length)
                {
                    // ���݂̎��Ԃ�ݒ�
                    textGameTimer.text = "" + (int)gameDirector.gameTimer;

                    // �U���͂̐��l�����ɖ߂�
                    power.text = "" + 0;
                    // �U���{�^�����\��
                    power.enabled = false;
                    // �_���[�W�����\��
                    attackButton.SetActive(false);
                }
                else
                {
                    // Update�ɓ���Ȃ��悤�ɂ���
                    enabled = false;

                    // ���̎��_��Update���甲����
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }

    // Attack�{�^�����������̏���
    public void AttackButton()
    {
        // �v���C���[�̍U�����I��
        gameDirector.EndAttack();

        //StartCoroutine(HiddenText());

        // �U���͂̐��l�����ɖ߂�
        power.text = "" + 0;
        // �_���[�W�����\��
        power.enabled = false;
        // �U���{�^�����\��
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
        else if (lastEnemy == enemyManager.Enemy[gameDirector.RoundCnt])
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
}
