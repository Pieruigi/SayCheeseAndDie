using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace SCAD
{
    public enum RatState { MoveToDest, DestinationReached }

    public class RatController : MonoBehaviour
    {

        NavMeshAgent agent;

        RatState state = RatState.MoveToDest;

        [SerializeField]
        float sightRange = 8;

        [SerializeField]
        float avoidanceRadius = 3;
        Vector3 startingPosition;
        
       

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            startingPosition = transform.position;
        }

        // Start is called before the first frame update
        void Start()
        {
            // Compute initial path
            var tmpList = LevelController.Instance.Holes.Where(obj => Vector3.Distance(obj.transform.position, transform.position) > 1).OrderByDescending(obj => Vector3.Distance(obj.transform.position, transform.position)).Take(5).ToList();
            var target = tmpList[Random.Range(0, tmpList.Count)];
            target = LevelController.Instance.Holes[2]; // TEST - Remove
            agent.SetDestination(target.transform.position);
            Debug.Log($"TEST - Agent destination:{agent.destination}");
        }

        void FixedUpdate()
        {
            switch (state)
            {
                case RatState.MoveToDest:
                    MoveToTargetHole();
                    break;
            }
        }

        void MoveToTargetHole()
        {
            if (IsDestinationReached())
            {
                state = RatState.DestinationReached;
                LevelController.Instance.NotifyRatReachedTargetHole(gameObject);
                return;
            }

            if (agent.pathPending)
                return;

            if (!agent.hasPath && !agent.pathPending)
            {
                Debug.Log("TEST - No path for the agent");
                // Choose a destination
                return;
            }

            if (agent.pathStatus == NavMeshPathStatus.PathPartial || agent.pathStatus == NavMeshPathStatus.PathInvalid)
            {
                Debug.Log("TEST - Partial or invalid path");
                // Choose a destination
                return;
            }


            if (agent.hasPath)
            {
                bool isSafe = PathIsSafe();
                Debug.Log($"TEST - Path is safe:{isSafe}");
            }

            

            // If for any reason the path is blocked choose another destination
            // if (agent.pathStatus == NavMeshPathStatus.PathPartial || agent.pathStatus == NavMeshPathStatus.PathInvalid)
            // {
            //     // Reset the invalid path
            //     agent.ResetPath();

            //     var l = LevelController.Instance.Holes.Where(l => l != targetHole && Vector3.Distance(transform.position, l.transform.position) > 2).ToList();
            //     targetHole = l[Random.Range(0, l.Count)];
            // }

            // if (agent.hasPath)
            // {
            //     Debug.Log($"TEST - Path.Corners.Length:{agent.path.corners.Length}");
            //     foreach (var c in agent.path.corners)
            //         Debug.Log($"TEST - Path.Corners.Next:{c}");
            // }


            // if (agent.hasPath)
            // {
            //     // Check if any further position is too close to the player    
            //     bool interrupt = false;

            //     for (int i = 0; i < agent.path.corners.Length && !interrupt; i++)
            //     {
            //         var corner = agent.path.corners[agent.path.corners.Length - 1 - i]; // Let's start from the farthest
            //         if (Vector3.Distance(PlayerController.Instance.transform.position, corner) < avoidanceRadius)
            //         {
            //             Debug.Log($"TEST - corner too close:{corner}");
            //             // Stop
            //             interrupt = true;

            //             // Get current destination
            //             var destination = agent.destination;

            //             // Reset path
            //             agent.ResetPath();

            //             // Get a new destination 
            //             var l = LevelController.Instance.Holes.Where(l => Vector3.Distance(destination, l.transform.position) > 2).ToList();
            //             targetHole = l[Random.Range(0, l.Count)];

            //             // Move avoider
            //             avoiderHelper.transform.position = PlayerController.Instance.transform.position;

            //             // Activate the avoider
            //             avoiderHelper.SetActive(true);

            //             // Compute the new path
            //             NavMeshPath path = new NavMeshPath();
            //             if (NavMesh.CalculatePath(transform.position, targetHole.transform.position, 0, path))
            //                 agent.SetPath(path);

            //             avoiderHelper.SetActive(false);
            //         }

            //     }
            // }

            // // // Check if player is visible
            // // if (Vector3.Distance(PlayerController.Instance.transform.position, transform.position) < sightRange)
            // // {
            // //     var origin = transform.position;
            // //     origin.y = agent.height;
            // //     var dir = PlayerController.Instance.transform.position - transform.position;
            // //     dir.y = 0;
            // //     dir = dir.normalized;
            // //     RaycastHit hitInfo;
            // //     if (Physics.Raycast(origin, dir, out hitInfo, sightRange))
            // //     {
            // //         if (hitInfo.collider.CompareTag("Player"))
            // //         {
            // //             // Reset current path if any
            // //             agent.ResetPath();

            // //             // Choose
            // //         }
            // //     }
            // // }


            // if (agent.hasPath || agent.pathPending)
            //     return;

            // Debug.Log($"TEST - New path - Setting new destination:{targetHole}");

            // agent.SetDestination(targetHole.transform.position); 


        }

        bool PathIsSafe()
        {
            if (!agent.hasPath) return false;

            if (agent.path.corners.Length < 1) return true;

            if (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) > sightRange) return true;

            for (int i = 0; i < agent.path.corners.Length - 1; i++)
            {

                var cornerA = agent.path.corners[i];
                var cornerB = agent.path.corners[i + 1];

                var origin = cornerA;
                origin.y = agent.height;
                var direction = cornerB - cornerA;
                direction.y = 0;

                RaycastHit hitInfo;
                if (Physics.Raycast(origin, direction.normalized, out hitInfo, direction.magnitude))
                {
                    if (hitInfo.collider.CompareTag("Player"))
                        return false;
                }
            }

            return true;
        }

        bool IsDestinationReached()
        {
            return Vector3.Distance(transform.position, agent.destination) < 0.1f;
        }

      
    }
    
}
