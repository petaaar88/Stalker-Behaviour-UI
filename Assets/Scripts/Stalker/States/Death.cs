using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Death : State<Stalker>
{
    public void Enter(Stalker stalker)
    {
        stalker.agentMovement.speed = 0.0f;
        stalker.GetComponent<CapsuleCollider>().enabled = false;
        stalker.animator.SetTrigger("IsDead");
        MessageBroker.Instance.stalkersWaitingForAttack.Remove(stalker);
        MessageBroker.Instance.engagedStalkers.Remove(stalker);
        stalker.audioManager.StopAllSounds();
        stalker.audioManager.PlaySound("Death");
    }

    public void Update(Stalker stalker)
    {

    }

    public void Exit(Stalker stalker)
    {
      
    }
}
