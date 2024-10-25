using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class EnemyHPbar : MonoBehaviour
{
    [SerializeField] Text hp;
    [SerializeField] Slider hpBar;

    EnemyManager enemyManager;
    GameDirector gameDirector;

    // �ő�HP
    public int maxHp = 0;
    // ���݂�HP
    int currentHp;
    //public Slider slider;

    private void Start()
    {
        enemyManager = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
    }

    // ���݂̓G�̍ő�Hp���i�[����
    public void MaxHp(int Hp)
    {
        maxHp = Hp;
        hp.text = "" + Hp;
        currentHp = Hp;
        hpBar.value = 1;
    }

    public void NowHp(int Hp)
    {
        currentHp = Hp;

        if (currentHp <= 0)
        {
            currentHp = 0;
        }

        hpBar.value = (float)currentHp / (float)maxHp;

        hp.text = "" + currentHp;
    }
}
