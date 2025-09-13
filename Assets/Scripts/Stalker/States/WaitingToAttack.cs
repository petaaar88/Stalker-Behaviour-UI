using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingToAttack : State<Stalker>
{
    public void Enter(Stalker stalker)
    {
        stalker.currentStalkerState = "WaitingToAttack";

        if (stalker.stateMachine.GetPreviousState() != stalker.stateMachine.attackingState)
            stalker.animator.SetTrigger("NearPlayer");

        stalker.agentMovement.speed = 0.0f;
    }

    public void Update(Stalker stalker)
    {
        if(!MessageBroker.Instance.isEngagementOver)
            MessageBroker.Instance.AddStalkersInQueueForAttack(stalker);

        if (Vector3.Distance(stalker.transform.position, stalker.player.transform.position) >= stalker.agentMovement.stoppingDistance + 2)
        {
            MessageBroker.Instance.stalkersWaitingForAttack.Remove(stalker);
            stalker.stateMachine.ChangeState(stalker.stateMachine.chaseState);
        }
        else if (!MessageBroker.Instance.isEngagementOver)
            if (MessageBroker.Instance.stalkersWaitingForAttack.Contains(stalker) && stalker.canAttack)
            {
                stalker.stateMachine.ChangeState(stalker.stateMachine.attackingState);
            }
    }

    public void Exit(Stalker stalker)
    {
        stalker.previousStalkerState = "WaitingToAttack";
    }
}
