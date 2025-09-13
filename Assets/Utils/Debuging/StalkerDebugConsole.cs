using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StalkerDebugConsole : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Stalker stalker;
    [SerializeField] private Text headerText;
    [SerializeField] private Text currentStateText;
    [SerializeField] private Text previousStateText;
    [SerializeField] private Text lastStatesText;
    [SerializeField] private Text animatorStatesText;
    [SerializeField] private Text isEngagingText;
    [SerializeField] private Text speedText;

    [Header("Settings")]
    [SerializeField] private int maxLastStates = 5;

    // Queue za custom state
    private Queue<CustomState> lastStates = new Queue<CustomState>();

    // Queue za Animator state
    private Queue<int> lastAnimatorStateHashes = new Queue<int>();
    private Dictionary<int, string> animatorStateNames = new Dictionary<int, string>();

    // Klasa koja čuva custom stanje + isEngaging flag
    private class CustomState
    {
        public State<Stalker> State { get; private set; }
        public bool IsEngaging { get; private set; }

        public CustomState(State<Stalker> state, bool isEngaging)
        {
            State = state;
            IsEngaging = isEngaging;
        }

        public override string ToString()
        {
            return $"{State} ({IsEngaging})";
        }
    }

    void Start()
    {
        headerText.text = $"{stalker.gameObject.name} State:";

        string layer = "Base Layer.";
        animatorStateNames.Add(Animator.StringToHash(layer + "RunToCover"), "RunToCover");
        animatorStateNames.Add(Animator.StringToHash(layer + "Death"), "Death");
        animatorStateNames.Add(Animator.StringToHash(layer + "LookAround"), "LookAround");
        animatorStateNames.Add(Animator.StringToHash(layer + "RunningTowardsPlayer"), "RunningTowardsPlayer");
        animatorStateNames.Add(Animator.StringToHash(layer + "SneakingForward"), "SneakingForward");
        animatorStateNames.Add(Animator.StringToHash(layer + "IdleOne"), "IdleOne");
        animatorStateNames.Add(Animator.StringToHash(layer + "Attack"), "Attack");
        animatorStateNames.Add(Animator.StringToHash(layer + "CrouchingIdle"), "CrouchingIdle");
    }

    void Update()
    {
        isEngagingText.text = "Is Engaging: " + (stalker.isEngagingToPlayer ? "True": "False");
        speedText.text = "Speed: " + stalker.agentMovement.speed;
        UpdateCustomStates();
        UpdateAnimatorStates();
        UpdateUI();
    }

    private void UpdateCustomStates()
    {
        var currentState = stalker.stateMachine.GetCurrentState();
        bool isEngaging = stalker.isEngagingToPlayer;

        // Dodaj novo stanje samo ako je različito od poslednjeg
        if (lastStates.Count == 0 || lastStates.Reverse().First().State != currentState)
        {
            if (lastStates.Count >= maxLastStates)
                lastStates.Dequeue();

            lastStates.Enqueue(new CustomState(currentState, isEngaging));
        }
    }

    private void UpdateAnimatorStates()
    {
        AnimatorStateInfo stateInfo = stalker.animator.GetCurrentAnimatorStateInfo(0);
        int currentHash = stateInfo.fullPathHash;

        if (lastAnimatorStateHashes.Count == 0 || lastAnimatorStateHashes.Reverse().First() != currentHash)
        {
            if (lastAnimatorStateHashes.Count >= maxLastStates)
                lastAnimatorStateHashes.Dequeue();

            lastAnimatorStateHashes.Enqueue(currentHash);
        }
    }

    private void UpdateUI()
    {
        currentStateText.text = $"CurrentState: {stalker.stateMachine.GetCurrentState()}";
        previousStateText.text = $"PreviousState: {stalker.stateMachine.GetPreviousState()}";
        var lines = lastStates.Reverse()
            .Select((s, i) => $"{i + 1}. {s}");
        lastStatesText.text = $"Last {maxLastStates} states:\n\n{string.Join("\n", lines)}";


        // Leaderboard za animator state
        animatorStatesText.text = $"Last {maxLastStates} Animator States:\n\n" +
            string.Join("\n", lastAnimatorStateHashes.Reverse().Select((h, i) =>
            {
                string name = animatorStateNames.ContainsKey(h) ? animatorStateNames[h] : "Unknown";
                return $"{i + 1}. {name}";
            }));
    }
}
