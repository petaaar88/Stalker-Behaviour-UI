using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chase : State<Stalker>
{
    private string[] chaseSfx = { "Threating1", "Laughing", "Threating2", "Threating3" };

    public void Enter(Stalker stalker)
    {
        stalker.agentMovement.SetTarget(stalker.player);
        stalker.agentMovement.speed = stalker.chaseSpeed;

        if (stalker.stateMachine.GetPreviousState() == stalker.stateMachine.waitingToAttackState)
            stalker.animator.SetTrigger("PlayerMovedAway");
        else
            stalker.animator.SetTrigger("StartChase");

        stalker.currentStalkerState = "Chase";

        stalker.audioManager.PlaySound("ChasingPlayer");
    }

    public void Update(Stalker stalker)
    {
        // Ako je stalker stigao dovoljno blizu igrača
        if (Vector3.Distance(stalker.transform.position, stalker.player.transform.position)
            <= stalker.agentMovement.stoppingDistance)
        {
            stalker.stateMachine.ChangeState(stalker.stateMachine.waitingToAttackState);
            return;
        }

        // Ako trenutno ne svira ni jedan chase SFX, izaberi nasumičan i pusti ga
        if (!IsAnyChaseSfxPlaying(stalker))
        {
            string randomSfx = chaseSfx[Random.Range(0, chaseSfx.Length)];
            stalker.audioManager.PlaySound(randomSfx);
        }
    }

    public void Exit(Stalker stalker)
    {
        stalker.previousStalkerState = "Chase";
        stalker.audioManager.StopSound("ChasingPlayer");
    }

    private bool IsAnyChaseSfxPlaying(Stalker stalker)
    {
        foreach (string sfx in chaseSfx)
        {
            if (stalker.audioManager.IsSoundPlaying(sfx))
                return true;
        }
        return false;
    }
}
