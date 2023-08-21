using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnegyCubeMovement : MonoBehaviour
{
    
    public NavMeshAgent navMeshAgent;
    public Rigidbody body;
    public GameObject[] waypoints;
    public float flyRadius = 15f;
    public ScoreUpdator scoreUpdator;

    private int currWayPoint;
    private float timeSinceLastbounce = 0;
    

    // Start is called before the first frame update
    void Start()
    {
        currWayPoint =  Random.Range(0, waypoints.Length);
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
    
    void bounce()
    {
        timeSinceLastbounce += Time.deltaTime;
        
        if (timeSinceLastbounce >= 1f)
        {
            var impulseVector = Vector3.up;
            var impulseForce = Random.Range(10f, 12f);

            body.AddForce(impulseVector * impulseForce, ForceMode.Impulse);
            //Debug.Log("bounce impulseForce is " + impulseForce);
            timeSinceLastbounce -= 1f;
        }
    }
    
    void setNextWaypoint()
    {
        currWayPoint = (currWayPoint + 1) % waypoints.Length;
        navMeshAgent.SetDestination(waypoints[currWayPoint].transform.position);
    }

    void setRandomWaypoint()
    {
        //Debug.Log("Setting random waypoint");
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
            //Debug.Log("OnTriggerEnter touched the enegy cube");
            scoreUpdator.CurScore += 1;


            GetComponent<AudioSource>().enabled = true;
            GetComponent<MeshRenderer>().enabled = false;

            // update score
            Invoke(nameof(myDestroy), .8f);
        }
    }
    
    void myDestroy()
    {
        Destroy(gameObject);
    }
    
    public void initWaypoints(GameObject[] waypoints)
    {
        this.waypoints = waypoints;
    }

}
