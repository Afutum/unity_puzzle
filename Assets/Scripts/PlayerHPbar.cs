using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPbar : MonoBehaviour
{
    [SerializeField] Text hp;

    int maxHp = 0;
    // åªç›ÇÃHP
    int currentHp;
    public Slider slider;

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
