using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnegyCubeMovement : MonoBehaviour
{
    
    public NavMeshAgent navMeshAgent;
    public GameObject[] waypoints;
    public float flyRadius = 15f;

    private int currWayPoint = 0;
    private float time;
    
    

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (navMeshAgent.remainingDistance < .5 && !navMeshAgent.pathPending)
        {
            if (waypoints.Length == 0)
            {
                setRandomWaypoint();
            }
            else
            {
                int randomInt = Random.Range(0, 10);
                if (randomInt < 3)
                    setNextWaypoint();
                else
                    setRandomWaypoint();
            }

        }

        //bounce();
    }

    void setNextWaypoint()
    {

        currWayPoint = (currWayPoint + 1) % waypoints.Length;
        navMeshAgent.SetDestination(waypoints[currWayPoint].transform.position);
    }

    void setRandomWaypoint()
    {
        Debug.Log("Setting random waypoint");
        Vector3 randomDirection = Random.insideUnitSphere * flyRadius;

        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, flyRadius, 1);
        Vector3 finalPosition = hit.position;

        //currWayPoint = (currWayPoint + 1) % waypoints.Length;
        //navMeshAgent.SetDestination(waypoints[currWayPoint].transform.position);
        navMeshAgent.SetDestination(finalPosition);
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("touched the enegy cube");
            var audio = GetComponent<AudioSource>();
            audio.Play();
            
            // update score
            Destroy(gameObject);
        }
    }

}
