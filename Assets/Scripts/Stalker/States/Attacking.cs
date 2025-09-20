using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacking : State<Stalker>
{

    public void Enter(Stalker stalker)
    {
        stalker.animator.SetTrigger("Attack");
        stalker.isRightHandedAttack = Random.Range(0, 2) == 1;
        stalker.animator.SetBool("isAttackMirrored", stalker.isRightHandedAttack);

        stalker.currentStalkerState = "Attacking";

        stalker.audioManager.PlaySound("Attack");
    }

    public void Update(Stalker stalker)
    {

    }

    public void Exit(Stalker stalker)
    {
        stalker.previousStalkerState = "Attacking";
        stalker.canAttack = false;
        MessageBroker.Instance.canChooseStalkerForAttacking = true;

            stalker.rightHandCollider.enabled = false;
            stalker.leftHandCollider.enabled = false;

        stalker.audioManager.StopSound("Attack");
    }
}
