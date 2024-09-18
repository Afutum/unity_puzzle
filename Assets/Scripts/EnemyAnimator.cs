using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 攻撃アニメーション
    public void SetAnimAttack()
    {
        animator.SetTrigger("EnemyAttack");
    }

    // 死ぬアニメーション
    public void SetAnimDie()
    {
        animator.SetTrigger("Die");
    }

    // ダメージアニメーション
    public void SetAnimDamage()
    {
        animator.SetTrigger("Damage");
    }

    // クラクラアニメーション
    public void SetAnimDizzy()
    {
        animator.SetBool("Dizzy", true);
    }

    public void ResetDizzyAnim()
    {
        animator.SetBool("Dizzy",false);
    }
}
