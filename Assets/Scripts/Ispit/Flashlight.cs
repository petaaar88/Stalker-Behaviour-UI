using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    private bool isOn = false;
    [SerializeField]
    private Light flashlightLight;
    [SerializeField]
    private GameObject bulb;

    private ObjectAudioManager audioManager;


    void Start()
    {
        audioManager = GetComponent<ObjectAudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)){
            if(isOn)
                TurnOff();
            else
                TurnOn();
        }
    }

    void TurnOn()
    {
        flashlightLight.enabled = true;
        bulb.SetActive(true);
        isOn = true;
        audioManager.PlaySound("Click");
    }

    void TurnOff()
    {
        flashlightLight.enabled = false;
        bulb.SetActive(false);
        isOn = false;
        audioManager.PlaySound("Click");
    }
}
