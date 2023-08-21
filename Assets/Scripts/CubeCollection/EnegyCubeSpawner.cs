using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnegyCubeSpawner : MonoBehaviour
{
    public GameObject prefab;
    public int numCubes; 
    public GameObject[] waypoints;
    public GameObject[] fixedpoints;

    void Start()
    {
        for (int i = 0 ; i < numCubes; i++)
        {
            GameObject cube = GameObject.Instantiate(prefab);
            cube.GetComponentInChildren<EnegyCubeMovement>().initWaypoints(waypoints);
            cube.transform.localPosition = waypoints[Random.Range(0, waypoints.Length)].transform.position;
            cube.transform.parent = this.transform;
        }
        
        // for (int i = 0; i < fixedpoints.Length; i++)
        // {
        //     GameObject cube = GameObject.Instantiate(prefab);
        //     cube.GetComponentInChildren<EnegyCubeMovement>().enabled = false;
        //     cube.GetComponent<NavMeshAgent>().enabled = false;
        //     cube.transform.localPosition = fixedpoints[i].transform.position;
        //     cube.transform.SetParent(transform.parent, false);
        // }
      
    }
}
