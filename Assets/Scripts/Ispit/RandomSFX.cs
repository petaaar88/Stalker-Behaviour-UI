using System.Collections.Generic;
using UnityEngine;

public class RandomSFX : MonoBehaviour
{
    private ObjectAudioManager audioManager;

    [SerializeField]
    private List<string> sfxNames;

    [SerializeField]
    private float minInterval = 5f; 
    [SerializeField]
    private float maxInterval = 12f;

    private float timer;
    private int lastIndex = -1; 

    void Start()
    {
        audioManager = GetComponent<ObjectAudioManager>();
        timer = Random.Range(minInterval, maxInterval);
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            int index;

            do
            {
                index = UnityEngine.Random.Range(0, sfxNames.Count);
            } while (index == lastIndex && sfxNames.Count > 1);

            string randomElement = sfxNames[index];
            audioManager.PlaySound(randomElement);

            lastIndex = index;

            timer = Random.Range(minInterval, maxInterval);
        }
    }
}
