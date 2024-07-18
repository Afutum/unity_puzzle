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
        // HP��0�ȉ��ɂȂ�����
        if(hp <= 0)
        {
            // HP��0�ɌŒ�
            hp = 0;
            // ���̃X�N���v�g�����Ă���I�u�W�F�N�g���\��
            this.gameObject.SetActive(false);
            // �Q�[�����I��
            gameDirector.isEnd = true;
        }
    }

    public void Attack(bool isAttack)
    {
        // �����̒l��true��������
        if (isAttack == true)
        {
            // �G�̃^�[���ɕύX
            gameDirector.isPlayerTurn = false;
            gameDirector.isEnemyTurn = true;

            // �v���C���[�̗̑͂����炷
            gameDirector.playerHp -= 300;
            Debug.Log(gameDirector.playerHp);
        }

        // �v���C���[�̃^�[���ɖ߂�
        gameDirector.isPlayerTurn = true;
        gameDirector.isEnemyTurn = false;
    }
}
