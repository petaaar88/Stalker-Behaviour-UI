using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    private Animator animator;
    [SerializeField]
    private GameObject weapon;
    private ObjectAudioManager audioManager;
    private bool isAttacking = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioManager = GetComponent<ObjectAudioManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !isAttacking)
        {
            StartAttack();
        }
    }

    private void StartAttack()
    {
        isAttacking = true;
        audioManager.PlaySound("Swing");
        animator.SetTrigger("IsAttacking");

        StartCoroutine(ResetAttackAfterTime());
    }

    private IEnumerator ResetAttackAfterTime()
    {
        yield return new WaitForSeconds(1.0f); 
        isAttacking = false;
    }

    public void EnableCollider()
    {
        weapon.GetComponent<CapsuleCollider>().enabled = true;
    }

    public void DisableCollider()
    {
        weapon.GetComponent<CapsuleCollider>().enabled = false;
    }

    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
    }
}