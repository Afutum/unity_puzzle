using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EnemyHPbar : MonoBehaviour
{
    [SerializeField] Text hp;

    EnemyManager enemyManager;
    GameDirector gameDirector;

    // 最大HP
    public int maxHp = 0;
    // 現在のHP
    int currentHp;
    public Slider slider;

    // 現在の敵の最大Hpを格納する
    public void MaxHp(int Hp)
    {
        maxHp = Hp;
        hp.text = "" + Hp;
        currentHp = Hp;
        slider.value = 1;
    }

    public void NowHp(int Hp)
    {
        currentHp = Hp;

        if (currentHp <= 0)
        {
            currentHp = 0;
        }

        hp.text = "" + currentHp;
        slider.value = (float)currentHp / (float)maxHp; ;
    }
}
