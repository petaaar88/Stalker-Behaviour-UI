using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cover : MonoBehaviour
{
    public Vector3 position;
    public float rotation = 0.0f;
    public Collider coverCollider;

    public bool mirrored = false;

    private void Start()
    {
        position = gameObject.transform.position;
    }
}
