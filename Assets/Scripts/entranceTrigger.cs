using System;
using System.Collections;
using System.Collections.Generic;
using Obi;
using UnityEditor;
using UnityEngine;

public class entranceTrigger : MonoBehaviour
{
    
    // TODO: set collision matrix
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("trigger enter " + other.name); // ObiSoftbody
        if (other.gameObject.CompareTag("Player")) 
        {
            Debug.Log(other.name+ " has enter the entrance");
            var cmp = other.transform.GetComponentInParent<SwitchMultipleActor>();
            if (cmp != null)
            {
                other.transform.GetComponent<MovementController>().enabled = false;
                
                cmp.SoftbodyToFluid();
                Invoke(nameof(closeEntrance), 0.5f);
                
            }
            else
            {
                Debug.Log("no cmp SwitchMultipleActor found on collider");
            }
            
        }
    }

    void closeEntrance()
    {
        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<ObiCollider>().enabled = true;
        
        transform.GetComponentInParent<MazeController>().enabled = true;
    }
    
}
