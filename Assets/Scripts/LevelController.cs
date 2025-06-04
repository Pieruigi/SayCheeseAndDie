using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SCAD
{
    public class LevelController : Singleton<LevelController>
    {
        [SerializeField]
        List<GameObject> holes;
        public IList<GameObject> Holes => holes.AsReadOnly();


        [SerializeField]
        GameObject ratPrefab;

        // [SerializeField]
        // int minRats = 8;

        [SerializeField]
        int maxRats = 1;

        [SerializeField]
        float ratSpawnRate = 1.5f;

        List<GameObject> rats = new List<GameObject>();

        float ratSpawnElapsed = 0;

        [SerializeField]
        float minRatSpawnDistanceFromPlayer = 10;
        public float MinRatSpawnDistanceFromPlayer { get { return minRatSpawnDistanceFromPlayer; } }
        

        // Start is called before the first frame update
        void Start()
        {

        }

        void FixedUpdate()
        {
            SpawnRats();
        }

        void SpawnRats()
        {
            ratSpawnElapsed += Time.deltaTime;
            if (ratSpawnElapsed < ratSpawnRate)
                return;

            ratSpawnElapsed -= ratSpawnRate;

            if (rats.Count < maxRats)
            {
                // Get a random starting hole
                var tmpList = holes.Where(h => Vector3.Distance(PlayerController.Instance.transform.position, h.transform.position) > minRatSpawnDistanceFromPlayer).ToList();
                var hole = tmpList[Random.Range(0, tmpList.Count)];

                // Get the target hole
                tmpList = holes.OrderByDescending(obj => Vector3.Distance(obj.transform.position, hole.transform.position)).Take(5).ToList();
                var target = tmpList[Random.Range(0, tmpList.Count)];

                // Create a new rat
                var rat = Instantiate(ratPrefab, hole.transform.position, hole.transform.rotation);
                rats.Add(rat);

                // Initialize rat
                rat.GetComponent<RatController>().Init(hole, target);
            }


        }

        void RemoveRat(GameObject rat)
        {
            rats.Remove(rat);
            Destroy(rat);
        }

        public void NotifyRatReachedTargetHole(GameObject rat)
        {
            // Tell the player they miss a rat

            // Remove rat
            RemoveRat(rat);

        }

        
        
    }
    
}
