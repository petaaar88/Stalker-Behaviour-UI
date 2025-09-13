using cakeslice;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class XRayVision : MonoBehaviour
{

    private bool isDisabled = true;
    
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.H))
            isDisabled = !isDisabled;

        Outline[] outlines = FindObjectsByType<Outline>(FindObjectsSortMode.None);
        foreach (var item in outlines)
        {
            item.eraseRenderer = isDisabled;
        }
    }
}
