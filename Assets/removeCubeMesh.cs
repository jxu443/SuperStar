using System.Collections;
using System.Collections.Generic;
using Obi;
using UnityEngine;


[ExecuteInEditMode]
public class removeCubeMesh : MonoBehaviour
{
    // Start is called before the first frame update
    private int count = 0;
    void Start()
    {
        helper(transform);
        Debug.Log("Removed " + count + " ObiColliders " + transform.childCount);

    }

    void helper(Transform obj)
    {
        // check if obj has child
        if (obj.childCount == 0) return;

        // if yes, check if obj has mesh renderer
        if (obj.GetComponent<ObiCollider>())
        {
            Destroy(obj.GetComponent<ObiCollider>());
            count++;
        }
        
        for(int i = 0; i < obj.childCount; i++)
        {
            Transform xx = obj.GetChild(i);
            helper(xx);
        }
        
    }
    
}
