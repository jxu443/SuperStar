using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Obi;

public class MazeController : MonoBehaviour
{
    [System.Serializable]
    public class ScoreChangedEvent : UnityEvent<int, int> { }

    //public ObiCollider finishLine;

    public float angularAcceleration = 5;

    [Range(0, 1)]
    public float angularDrag = 0.2f;
    
    //HashSet<int> finishedParticles = new HashSet<int>();

    float angularSpeed = 0;
    float angle = 0;


    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            //Debug.Log("A is pressed");
            angularSpeed += angularAcceleration * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            angularSpeed -= angularAcceleration * Time.deltaTime;
        }
        angularSpeed *= Mathf.Pow(1 - angularDrag, Time.deltaTime);
        angle += angularSpeed * Time.deltaTime;

        transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);
        
    }

}
