using MB.PhysicsPrediction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector.vCharacterController;

public class ThrowingObject : MonoBehaviour
{
    private int numberOfProjectiles = 0;
    [SerializeField]
    private int maxNumberOfProjectiles = 3;
    [SerializeField]
    private bool hasInfiniteProjectiles = false;
    [SerializeField]
    private MeshRenderer bottleMesh;

    [SerializeField]
    private GameObject prefab = default;
    private bool isAiming;
    private vThirdPersonController playerController;
    private vThirdPersonCamera playerCamera;
    private float targetRightOffset = 0.2f;
    public float interpolationSpeed = 5f;
    private PlayerStates playerState;

    [SerializeField]
    ForceData force = new ForceData(Vector3.forward * 2 + Vector3.up * 3, ForceMode.Impulse);

    // Camera rotation control settings
    public float minCameraAngle = -30f; // minimum camera X angle for min force
    public float maxCameraAngle = 30f;  // maximum camera X angle for max force
    public float maxForceMultiplier = 2f;


    [Serializable]
    public struct ForceData
    {
        [SerializeField]
        public Vector3 vector;

        [HideInInspector]
        ForceMode mode;
        public ForceMode Mode => mode;

        public ForceData(Vector2 vector, ForceMode mode)
        {
            this.vector = vector;
            this.mode = mode;
        }
    }

    [SerializeField]
    PredictionProperty prediction = default;

    [Serializable]
    public class PredictionProperty
    {
        [SerializeField]
        int iterations = 40;
        public int Iterations => iterations;

        [SerializeField]
        int rate = 30;
        public int Rate => rate;

        [SerializeField]
        LineRenderer line = default;
        public LineRenderer Line => line;
    }

    PredictionTimeline timeline;

    public const KeyCode Key = KeyCode.Mouse0;

    Transform InstanceContainer;

    void Start()
    {
        if (!hasInfiniteProjectiles && numberOfProjectiles == 0)
            bottleMesh.enabled = false;

        playerController = gameObject.transform.root.gameObject.GetComponent<vThirdPersonController>();
        playerState = gameObject.transform.root.gameObject.GetComponent<PlayerStates>();
        playerCamera = FindObjectOfType<vThirdPersonCamera>();
        InstanceContainer = new GameObject("Projectiles Container").transform;

        StartCoroutine(Procedure());
    }

    void Update()
    {

    

        if (playerState.currentState == PlayerStates.States.SPRINTING || playerState.currentState == PlayerStates.States.WALKING)
        {
            isAiming = false;
            playerController.isStrafing = false;

            prediction.Line.positionCount = 0;
            if(timeline != null)
                PredictionSystem.Record.Prefabs.Remove(timeline);

            TrajectoryPredictionDrawer.HideAll();

            targetRightOffset = 0.2f;
            return;
        }


        if (Input.GetKeyDown(KeyCode.Mouse1) && (numberOfProjectiles != 0 || hasInfiniteProjectiles))
        {
            timeline = PredictionSystem.Record.Prefabs.Add(prefab, Launch);
            playerController.isStrafing = true;
            isAiming = true;
            targetRightOffset = 0.78f;
        }
        else if(numberOfProjectiles == 0 && !hasInfiniteProjectiles)
        {
            prediction.Line.positionCount = 0;
            playerController.isStrafing = false;

            if (timeline != null)
                PredictionSystem.Record.Prefabs.Remove(timeline);

            isAiming = false;


            foreach (Transform child in InstanceContainer)
            {
                if (child.GetComponent<ThrowableObject>().isCollided)
                    Destroy(child.gameObject);
            }

            TrajectoryPredictionDrawer.HideAll();
            targetRightOffset = 0.2f;
        }

        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            prediction.Line.positionCount = 0;
            playerController.isStrafing = false;

            if(timeline != null)
                PredictionSystem.Record.Prefabs.Remove(timeline);

            isAiming = false;


            foreach (Transform child in InstanceContainer)
            {
                if(child.GetComponent<ThrowableObject>().isCollided)
                    Destroy(child.gameObject);
            }

            TrajectoryPredictionDrawer.HideAll();
            targetRightOffset = 0.2f;
        }
        Shoot();

        playerCamera.rightOffset = Mathf.Lerp(playerCamera.rightOffset, targetRightOffset, Time.deltaTime * interpolationSpeed);
    }

    void Shoot()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && isAiming && (numberOfProjectiles != 0 || hasInfiniteProjectiles))
        {
            var instance = Instantiate(prefab);
            instance.transform.SetParent(InstanceContainer);
            Launch(instance);
            numberOfProjectiles--;

            if (numberOfProjectiles == 0 && !hasInfiniteProjectiles)
                bottleMesh.enabled = false;
        }
    }

    public bool AddProjectile()
    {
        if (numberOfProjectiles == maxNumberOfProjectiles)
            return false;

        if(numberOfProjectiles == 0)
            bottleMesh.enabled = true;

        numberOfProjectiles++;

        return true;
    }

    public int GetNumberOfProjectiles()
    {
        return numberOfProjectiles;
    }

    public bool HasInfiniteProjectiles()
    {
        return hasInfiniteProjectiles;
    }

    float GetForceMultiplierFromCameraRotation()
    {
        // Uzmi X rotaciju kamere
        float cameraXRotation = playerCamera.transform.eulerAngles.x;

        // Konvertuj u -180 do 180 range
        if (cameraXRotation > 180f)
            cameraXRotation -= 360f;

        // Mapiraj camera angle na force multiplier
        float normalizedAngle = Mathf.InverseLerp(-minCameraAngle, -maxCameraAngle, cameraXRotation);
        return Mathf.Lerp(1f, maxForceMultiplier, normalizedAngle);
    }

    void Launch(GameObject gameObject)
    {
        var rigidbody = gameObject.GetComponent<Rigidbody>();

        // Skaliraj silu prema X rotaciji kamere
        float forceMultiplier = GetForceMultiplierFromCameraRotation();
        var scaledForce = force.vector * forceMultiplier;
        var relativeForce = transform.TransformVector(scaledForce);

        rigidbody.AddForce(relativeForce, force.Mode);
        rigidbody.transform.position = transform.position;
        rigidbody.transform.rotation = transform.rotation;
    }

    IEnumerator Procedure()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f / prediction.Rate);
            Predict();
        }
    }

    void Predict()
    {
        if (Input.GetKey(KeyCode.Mouse1) && isAiming)
        {
            PredictionSystem.Simulate(prediction.Iterations);
            TrajectoryPredictionDrawer.ShowAll();
            prediction.Line.positionCount = timeline.Count;

            for (int i = 0; i < timeline.Count; i++)
                prediction.Line.SetPosition(i, timeline[i].Position);
        }
    }
}