using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

namespace SCAD
{
    public enum RatState { MoveToDest, DestinationReached }

    public class RatController : MonoBehaviour
    {

        GameObject targetHole;
        GameObject startingHole;

        NavMeshAgent agent;

        RatState state = RatState.MoveToDest;

        
       

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            switch (state)
            {
                case RatState.MoveToDest:
                    UpdateMoveToDestState();
                    break;
            }
        }

        void UpdateMoveToDestState()
        {
            if (IsTargetHoleReached())
            {
                state = RatState.DestinationReached;
                LevelController.Instance.NotifyRatReachedTargetHole(gameObject);
                return;
            }

            // If for any reason the path is blocked choose another destination
            if (agent.pathStatus == NavMeshPathStatus.PathPartial || agent.pathStatus == NavMeshPathStatus.PathInvalid)
            {
                // Reset the invalid path
                agent.ResetPath();
                
                var l = LevelController.Instance.Holes.Where(l => l != targetHole && Vector3.Distance(transform.position, l.transform.position) > 2).ToList();
                targetHole = l[Random.Range(0, l.Count)];
            }

            if (agent.hasPath)
            {
                Debug.Log($"TEST - Path.Corners.Length:{agent.path.corners.Length}");
                foreach(var c in agent.path.corners)
                    Debug.Log($"TEST - Path.Corners.Next:{c}");
            }

            if (agent.hasPath || agent.pathPending)
                return;

            Debug.Log($"TEST - New path - Setting new destination:{targetHole}");
            agent.SetDestination(targetHole.transform.position); 


        }

        bool IsTargetHoleReached()
        {
            return Vector3.Distance(transform.position, targetHole.transform.position) < 0.5f;
        }

        public void Init(GameObject startingHole, GameObject targetHole)
        {
            this.targetHole = targetHole;
            this.startingHole = startingHole;
        }
    }
    
}
