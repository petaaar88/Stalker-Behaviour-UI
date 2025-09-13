
using UnityEngine;

public class EngagingPlayer : State<Stalker>
{

    public void Enter(Stalker stalker)
    {
        stalker.stateMachine.ChangeState(stalker.stateMachine.chaseState);
        MessageBroker.Instance.AddStalkerToEngagement(stalker);
    }

    public void Update(Stalker stalker)
    {

        if (MessageBroker.Instance.isEngagementOver)
        {
            stalker.stateMachine.ChangeState(stalker.stateMachine.recoveringState);
            this.Exit(stalker);
        }
    }

    public void Exit(Stalker stalker)
    {
        stalker.isEngagingToPlayer = false;
    }
}
