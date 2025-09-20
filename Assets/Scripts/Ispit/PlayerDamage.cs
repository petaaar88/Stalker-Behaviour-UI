using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDamage : MonoBehaviour
{
    private Health playerHealth;
    private ObjectAudioManager audioManager;
    private Animator animator;

    private bool isDead = false;

    void Start()
    {
        playerHealth = gameObject.GetComponent<Health>();
        audioManager = GetComponent<ObjectAudioManager>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (playerHealth.GetHealth() <= 0 && !isDead)
        {
            Component[] components = GetComponents<Component>();

            foreach (Component comp in components)
            {
                if (comp is Transform || comp is Animator || comp is PlayerDamage)
                    continue;

                Destroy(comp);
            }

            animator.SetTrigger("IsDead");
            isDead = true;
           

        }
    }

    private void LoadYouDiedScene()
    {
        Debug.Log("Player died. Loading YouDied scene...");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("YouDied");
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
