using UnityEngine;

public class PlayerStates : MonoBehaviour
{
    public enum States
    {
        WALKING,
        SPRINTING,
        CROUCHING,
        IDLE
    }

    public States currentState;
    private States previousState;
    private bool isCrouching;
    public string utrenutnomstanju;
    private ObjectAudioManager audioManager;
    private bool isWalkingSoundPlaying = false;
    private bool isSprintingSoundPlaying = false;

    void Start()
    {
        currentState = States.IDLE;
        previousState = States.IDLE;
        audioManager = GetComponent<ObjectAudioManager>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        previousState = currentState;

        if (Input.GetKeyDown(KeyCode.C))
        {
            isCrouching = !isCrouching;
        }

        bool isMoving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
                       Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);

        if (isCrouching)
        {
            currentState = States.CROUCHING;
        }
        else if (isMoving && Input.GetKey(KeyCode.LeftShift))
        {
            currentState = States.SPRINTING;
            NoiceListener.Instance.RegisterLoudNoice(transform.position);
        }
        else if (isMoving)
        {
            currentState = States.WALKING;
            NoiceListener.Instance.RegisterSubtleNoice(transform.position);
        }
        else
        {
            currentState = States.IDLE;
        }

        // Handle walking sound
        if (currentState == States.WALKING && !isWalkingSoundPlaying)
        {
            isWalkingSoundPlaying = true;
            audioManager.PlaySound("Walking");
        }
        else if (currentState != States.WALKING && isWalkingSoundPlaying)
        {
            audioManager.StopSound("Walking");
            isWalkingSoundPlaying = false;
        }

        // Handle sprinting sound
        if (currentState == States.SPRINTING && !isSprintingSoundPlaying)
        {
            isSprintingSoundPlaying = true;
            audioManager.PlaySound("Sprinting");
        }
        else if (currentState != States.SPRINTING && isSprintingSoundPlaying)
        {
            audioManager.StopSound("Sprinting");
            isSprintingSoundPlaying = false;
        }
    }
}