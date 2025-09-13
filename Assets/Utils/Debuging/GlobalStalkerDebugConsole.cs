using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GlobalStalkerDebugConsole : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Text headerText;
    [SerializeField] private Text currentStateText;
    [SerializeField] private Text previousStateText;
    [SerializeField] private Text lastStatesText;
    [SerializeField] private Text animatorStatesText;
    [SerializeField] private Text isEngagingText;
    [SerializeField] private Text speedText;

    [Header("Settings")]
    [SerializeField] private int maxLastStates = 5;

    [HideInInspector] public int selectedStalkerIndex = 0;

    private List<Stalker> allStalkers = new List<Stalker>();
    [SerializeField, HideInInspector] private string[] stalkerNames;

    // Svaki stalker ima svoj history
    private Dictionary<Stalker, StalkerHistory> stalkerHistories = new Dictionary<Stalker, StalkerHistory>();

    private Dictionary<int, string> animatorStateNames = new Dictionary<int, string>();

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

    private class StalkerHistory
    {
        public Queue<CustomState> LastStates = new Queue<CustomState>();
        public Queue<int> LastAnimatorStates = new Queue<int>();
    }

    void Awake()
    {
        allStalkers = FindObjectsOfType<Stalker>().ToList();
        stalkerNames = allStalkers.Select(s => s.gameObject.name).ToArray();

        stalkerHistories.Clear();
        foreach (var stalker in allStalkers)
        {
            stalkerHistories[stalker] = new StalkerHistory();
        }

        // Animator state hash mapa
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
        if (allStalkers.Count == 0) return;
        if (selectedStalkerIndex < 0 || selectedStalkerIndex >= allStalkers.Count) return;

        var stalker = allStalkers[selectedStalkerIndex];
        var history = stalkerHistories[stalker];

        isEngagingText.text = "Is Engaging: " + (stalker.isEngagingToPlayer ? "True" : "False");
        speedText.text = "Speed: " + stalker.agentMovement.speed;

        UpdateCustomStates(stalker, history);
        UpdateAnimatorStates(stalker, history);
        UpdateUI(stalker, history);
    }

    private void UpdateCustomStates(Stalker stalker, StalkerHistory history)
    {
        var currentState = stalker.stateMachine.GetCurrentState();
        bool isEngaging = stalker.isEngagingToPlayer;

        if (history.LastStates.Count == 0 || history.LastStates.Reverse().First().State != currentState)
        {
            if (history.LastStates.Count >= maxLastStates)
                history.LastStates.Dequeue();

            history.LastStates.Enqueue(new CustomState(currentState, isEngaging));
        }
    }

    private void UpdateAnimatorStates(Stalker stalker, StalkerHistory history)
    {
        AnimatorStateInfo stateInfo = stalker.animator.GetCurrentAnimatorStateInfo(0);
        int currentHash = stateInfo.fullPathHash;

        if (history.LastAnimatorStates.Count == 0 || history.LastAnimatorStates.Reverse().First() != currentHash)
        {
            if (history.LastAnimatorStates.Count >= maxLastStates)
                history.LastAnimatorStates.Dequeue();

            history.LastAnimatorStates.Enqueue(currentHash);
        }
    }

    private void UpdateUI(Stalker stalker, StalkerHistory history)
    {
        headerText.text = $"{stalker.gameObject.name} State:";
        currentStateText.text = $"CurrentState: {stalker.stateMachine.GetCurrentState()}";
        previousStateText.text = $"PreviousState: {stalker.stateMachine.GetPreviousState()}";

        lastStatesText.text = $"Last {maxLastStates} states:\n\n" +
            string.Join("\n", history.LastStates.Reverse().Select((s, i) => $"{i + 1}. {s}"));

        animatorStatesText.text = $"Last {maxLastStates} Animator States:\n\n" +
            string.Join("\n", history.LastAnimatorStates.Reverse().Select((h, i) =>
            {
                string name = animatorStateNames.ContainsKey(h) ? animatorStateNames[h] : "Unknown";
                return $"{i + 1}. {name}";
            }));
    }

    // Metoda za Editor skriptu da dobije imena stalkera
    public string[] GetStalkerNames()
    {
        return stalkerNames ?? new string[0];
    }
}
