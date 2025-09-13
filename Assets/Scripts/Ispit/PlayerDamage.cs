using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    private Health playerHealth;
    private ObjectAudioManager audioManager;

    void Start()
    {
        playerHealth = gameObject.GetComponent<Health>();
        audioManager = GetComponent<ObjectAudioManager>();
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("StalkerAttackLeft")){
            audioManager.PlaySound("Hurt");
            playerHealth.TakeDamage(10);
        }

        if (other.CompareTag("StalkerAttackRight"))
        {
            audioManager.PlaySound("Hurt");

            playerHealth.TakeDamage(10);
        }
    }
}
