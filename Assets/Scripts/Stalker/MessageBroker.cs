using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MessageBroker : MonoBehaviour
{
    public static MessageBroker Instance { get; private set; }
    public List<Stalker> engagedStalkers = new List<Stalker>();
    public HashSet<Stalker> stalkersWaitingForAttack = new HashSet<Stalker>();

    public float engagementTime = 4.0f;
    public float engagementTimer = 0f;
    private bool isTimerStarted = false;
    public bool isEngagementOver = true;

    public bool canChooseStalkerForAttacking = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!isTimerStarted) return;

        engagementTimer += Time.deltaTime;

        if (canChooseStalkerForAttacking)
        {
            List<Stalker> sortedNearestStalkersToPlayer = stalkersWaitingForAttack
                                                          .OrderBy(s => Vector3.Distance(s.transform.position, s.player.position))
                                                          .Take(2)
                                                          .ToList();

            // Choose random stalker for attacking from stalkers that are near player
            if (sortedNearestStalkersToPlayer.Count > 0)
            {
                int indexOfChoosenStalker = Random.Range(0, sortedNearestStalkersToPlayer.Count);
                sortedNearestStalkersToPlayer[indexOfChoosenStalker].canAttack = true;

                canChooseStalkerForAttacking = false;

                // This will make loud noise in order to call other stalkers, stalker will come beacuse he heard loud noise, not beacuse other stalker called him
                NoiceListener.Instance.RegisterLoudNoice(sortedNearestStalkersToPlayer[indexOfChoosenStalker].transform.position);
            }
           
        }


        if (engagementTimer >= engagementTime)
        {
            isEngagementOver = true;
            canChooseStalkerForAttacking = true;
            engagementTimer = 0.0f;
            isTimerStarted = false;
            engagementTime = 5.0f;
            engagedStalkers.Clear();
            stalkersWaitingForAttack.Clear();
        }

    }

    // All stalkers that saw player
    public void AddStalkerToEngagement(Stalker stalker)
    {

        engagedStalkers.Add(stalker);
        engagementTime += 2f;

        if (engagedStalkers.Count == 1)
        {
            isTimerStarted = true;
            isEngagementOver = false;
        }
    }

    // Stalker's that are near player
    public void AddStalkersInQueueForAttack(Stalker stalker)
    {
        stalkersWaitingForAttack.Add(stalker);
    }

    private void CaluclateEngagementTime()
    {

    }

    public bool IsEngagement()
    {
        return isTimerStarted;
    }
}
