using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalkerStateMachine : StateMachine<Stalker>
{
    public Relocating relocatingState;
    public InCover inCoverState;
    public Chase chaseState;
    public Recovering recoveringState;
    public Investigating investigatingState;
    public LookingAround lookingAroundState;
    public AlertInvestigating alertInvestigatingState;
    public WaitingToAttack waitingToAttackState;
    public Attacking attackingState;
    public Death deathState;

    public EngagingPlayer engagingPlayerState;

    public StalkerStateMachine(Stalker stalker) : base(stalker) {

        relocatingState = new Relocating();
        inCoverState = new InCover();
        chaseState = new Chase();
        recoveringState = new Recovering();
        investigatingState = new Investigating();
        lookingAroundState = new LookingAround();
        alertInvestigatingState = new AlertInvestigating();
        engagingPlayerState = new EngagingPlayer();
        waitingToAttackState = new WaitingToAttack();
        deathState = new Death();
        attackingState = new Attacking();
        globalState = new GlobalStalkerState();
    }

    public override void Update()
    {
        base.Update();

        if (entity.isEngagingToPlayer)
            engagingPlayerState.Update(entity);
    }

}
