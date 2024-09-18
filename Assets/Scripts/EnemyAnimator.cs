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

    // �U���A�j���[�V����
    public void SetAnimAttack()
    {
        animator.SetTrigger("EnemyAttack");
    }

    // ���ʃA�j���[�V����
    public void SetAnimDie()
    {
        animator.SetTrigger("Die");
    }

    // �_���[�W�A�j���[�V����
    public void SetAnimDamage()
    {
        animator.SetTrigger("Damage");
    }

    // �N���N���A�j���[�V����
    public void SetAnimDizzy()
    {
        animator.SetBool("Dizzy", true);
    }

    public void ResetDizzyAnim()
    {
        animator.SetBool("Dizzy",false);
    }
}
