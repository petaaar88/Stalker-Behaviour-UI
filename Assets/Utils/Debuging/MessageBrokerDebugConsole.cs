using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MessageBrokerDebugConsole : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Text engagedStalkersText;
    [SerializeField] private Text waitingStalkersText;
    [SerializeField] private Text isEngagementText;

    private void Update()
    {
        if (MessageBroker.Instance == null) return;
        var broker = MessageBroker.Instance;

            engagedStalkersText.text =
                "Engaged Stalkers:  " +
                string.Join(", ", broker.engagedStalkers
                    .Select(s => $"{s.gameObject.name}"));
            waitingStalkersText.text =
                "Near Stalkers:  " +
                string.Join(", ", broker.stalkersWaitingForAttack
                    .Select(s => $"{s.gameObject.name}"));


        isEngagementText.text = "Is Engagement: " + (MessageBroker.Instance.isEngagementOver ? "No" : "Yes");

    }
}
