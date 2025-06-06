using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;
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
        float avoidanceRadius = 8;
        Vector3 startingPosition;

        NavMeshObstacle playerObstacle;

        NavMeshPath cachedPath;

        Vector3 destination;

        float pathUpdateRate = .5f;


        float pathUpdateElapsed = 0;


        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            startingPosition = transform.position;
            agent.isStopped = true;
    
        }

        // Start is called before the first frame update
        void Start()
        {

            var tmpList = LevelController.Instance.Holes.Where(obj => Vector3.Distance(obj.transform.position, transform.position) > 1).OrderByDescending(obj => Vector3.Distance(obj.transform.position, transform.position)).Take(5).ToList();
            var targetObj = tmpList[Random.Range(0, tmpList.Count)];
            targetObj = LevelController.Instance.Holes[1]; // TEST - Remove

            agent.SetDestination(targetObj.transform.position);

            // NavMesh.CalculatePath(transform.position, target.transform.position, NavMesh.AllAreas, cachedPath);

            cachedPath = new NavMeshPath();
            // Debug.Log($"TEST - Path:{cachedPath.status}");
            // //agent.SetDestination(target.transform.position);
            // Debug.Log($"TEST - Agent destination:{agent.destination}");
            // playerObstacle = PlayerController.Instance.GetComponent<NavMeshObstacle>();
        }

        void Update()
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

            if (agent.pathPending)
            {
                Debug.Log($"TEST - Path pending");
                return;
            }

            if (agent.hasPath)
            {
                Debug.Log($"TEST - Path completed");
                var closest = GetClosestCornerId();
                Debug.Log($"TEST - closest:{closest}");
                Vector3 target = Vector3.zero;
                bool hasTarget = false;
                if (closest < agent.path.corners.Length - 1)
                {
                    hasTarget = true;
                    target = agent.path.corners[closest + 1];
                }

                if (hasTarget)
                {
                    var dir = target - transform.position;
                    dir.y = 0;
                    transform.position += dir.normalized * agent.speed * Time.deltaTime;
                }

                if (IsDestinationReached())
                {
                    state = RatState.DestinationReached;
                    LevelController.Instance.NotifyRatReachedTargetHole(gameObject);
                }

                return;
            }

            return;
          

          

            if (IsDestinationReached())
            {
                state = RatState.DestinationReached;
                LevelController.Instance.NotifyRatReachedTargetHole(gameObject);
                return;
            }

            return;
            if (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) < playerObstacle.radius)
            {
                // Disable agent to avoid exiting the nav mesh

            }

            if (!agent.isOnNavMesh)
            {
                Debug.Log("TEST - Agent is out of navmesh");
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
                // Choose a new destination
                var destination = agent.destination;

                var l = LevelController.Instance.Holes.Where(obj => Vector3.Distance(obj.transform.position, destination) > 1).OrderByDescending(obj => Vector3.Distance(obj.transform.position, destination)).Take(5).ToList();

                agent.ResetPath();

                destination = l[Random.Range(0, l.Count)].transform.position;
                agent.SetDestination(destination);
                return;
            }

            return;
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

            if (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) > avoidanceRadius) return true;

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

        int GetClosestCornerId()
        {
            int cosest = -1;
            float minDist = 0;
            for (int i = 0; i < agent.path.corners.Length; i++)
            {
                float dist = Vector3.Distance(transform.position, agent.path.corners[i]);
                if (cosest < 0 || dist < minDist)
                {
                    cosest = i;
                    minDist = dist;
                }
            }

            return cosest;
        }

        bool IsDestinationReached()
        {
            return Vector3.Distance(transform.position, agent.destination) < 0.1f;
        }


    }
    
}
