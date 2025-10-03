using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisablePreviewCamera : MonoBehaviour
{
    [SerializeField]
    private GameObject previewCamera;
    [SerializeField]
    private GameObject visualGroup;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (visualGroup.activeSelf)
        {
            previewCamera.SetActive(true);
        }
        else
        {
            previewCamera.SetActive(false);
        }
    }
}
