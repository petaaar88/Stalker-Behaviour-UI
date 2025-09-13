using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchingWeapons : MonoBehaviour
{
    [SerializeField]
    private GameObject weapon;
    [SerializeField]
    private GameObject throwableObject;

    // Start is called before the first frame update
    void Start()
    {
        weapon.SetActive(true);
        weapon.GetComponent<CapsuleCollider>().enabled = false;
        GetComponent<MeleeAttack>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            weapon.SetActive(true);
            weapon.GetComponent<CapsuleCollider>().enabled = false;

            throwableObject.SetActive(false);
            GetComponentInChildren<ThrowingObject>().enabled = false;
            GetComponent<MeleeAttack>().enabled = true;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            weapon.SetActive(false);
            weapon.GetComponent<CapsuleCollider>().enabled = false;

            throwableObject.SetActive(true);
            GetComponent<MeleeAttack>().enabled = false;

            GetComponentInChildren<ThrowingObject>().enabled = true;
        }

    }
}
