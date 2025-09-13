using UnityEngine;

public class NoiceListener : MonoBehaviour
{
    public static NoiceListener Instance;

    public Vector3 loudNoicePosition { get; private set; }
    public Vector3 subtleNoicePosition { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        loudNoicePosition = Vector3.positiveInfinity;
        subtleNoicePosition = Vector3.positiveInfinity;
    }

    public void RegisterLoudNoice(Vector3 noicePosition)
    {
        loudNoicePosition = noicePosition;
    }

    public void RegisterSubtleNoice(Vector3 noicePosition)
    {
        subtleNoicePosition = noicePosition;
    }

   
    
}
