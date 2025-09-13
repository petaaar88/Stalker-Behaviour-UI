using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class PathFindingDebugConsole : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Stalker stalker;
    [SerializeField] private Text headerText;
    [SerializeField] private Text isUsingAStarText;
    [SerializeField] private Text speedText;
    [SerializeField] private Text pathSizeText;
    [SerializeField] private Text currentIndexInPathText;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        headerText.text = $"{stalker.gameObject.name} PathFinding:";

        isUsingAStarText.text = "Using A* : " + (stalker.agentMovement.isUsingAStarDebug ? "True" : "False");
        speedText.text = "Speed: " + stalker.agentMovement.speed;
        pathSizeText.text = "Path Size: " + stalker.agentMovement.pathSolver.path.Count;
        currentIndexInPathText.text = "Current waypoint: " + stalker.agentMovement.currentNodeIndex;

    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawSphere(transform.position, 0.3f);

        if(stalker && stalker.agentMovement)
        if (stalker.agentMovement.target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(stalker.agentMovement.target.position, 0.3f);
                Gizmos.color = Color.blue;
            Gizmos.DrawSphere(stalker.agentMovement.pathSolver.grid.NodeFromWorldPoint(stalker.agentMovement.target.position).worldPosition, 0.3f);



            for (int i = 0; i < stalker.agentMovement.pathSolver.path.Count; i++)
            {
                Gizmos.color = Color.green;
                if (stalker.agentMovement.currentNodeIndex == i)
                    Gizmos.color = Color.magenta;
                Gizmos.DrawSphere(stalker.agentMovement.pathSolver.path[i].worldPosition, 0.17f);
                    
            }

            }

    }
}
