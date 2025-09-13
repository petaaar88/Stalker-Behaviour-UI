using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GlobalStalkerState : State<Stalker>
{
    private float chaseDelayTimer = 0f;
    private bool sawPlayer = false;
    private bool isDead = false;

    public void Enter(Stalker stalker)
    {

    }

    public void Update(Stalker stalker)
    {
        if (stalker.health.IsDead && !isDead)
        {
            stalker.stateMachine.ChangeState(stalker.stateMachine.deathState);
            isDead = true;
            return;
        }
        if (isDead)
            return;
        // Delaying chasing
        if (sawPlayer)
        {
            if (stalker.stateMachine.GetCurrentState() == stalker.stateMachine.recoveringState)
            {
                sawPlayer = false;
                chaseDelayTimer = 0;
                return;
            }

            chaseDelayTimer += Time.deltaTime;
            
            if (chaseDelayTimer >= stalker.chaseDelay)
            {
                chaseDelayTimer = 0.0f;
                sawPlayer = false;
                
                if(!stalker.isEngagingToPlayer)
                    stalker.StartEngageToPlayer();

                //stalker.stateMachine.ChangeState(stalker.stateMachine.chaseState);

            }
        }

        // mora da bude ispod delaying chase
        if (!stalker.canChasePlayer)
            return;



        Vector3 directionToPlayer = stalker.playerSpotPoint.position - stalker.eyes.position;

        // Visibility 
        if (directionToPlayer.magnitude <= stalker.viewDistance)
        {
            float angle = Vector3.Angle(stalker.eyes.transform.forward, directionToPlayer.normalized);

            if (angle <= stalker.viewAngle / 2)
                if (!Physics.Raycast(stalker.eyes.transform.position, directionToPlayer.normalized, directionToPlayer.magnitude, stalker.obstacleMask))
                    sawPlayer = true;
        }

        // Allowing to show line from stalker to player when stalker see player
        if (stalker.isEngagingToPlayer)
            stalker.canSeePlayer = true;
        else
            stalker.canSeePlayer = false;


        stalker.SubtleNoiceDetection();
        stalker.LoudNoiceDetection();
        

    }

    public void Exit(Stalker entity)
    {
        chaseDelayTimer = 0.0f;
    }
}
